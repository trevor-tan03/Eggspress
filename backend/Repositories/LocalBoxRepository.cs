using backend.Data;
using backend.Models;
using backend.util;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace backend.Repositories;

public class LocalBoxRepository : IBoxRepository
{
    private readonly ILogger<LocalBoxRepository> _logger;
    private readonly string _basePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "Boxes");
    private readonly BoxDbContext _context;
    private readonly IFileRepository _fileRepository;

    public LocalBoxRepository(ILogger<LocalBoxRepository> logger, BoxDbContext context, IFileRepository fileRepository)
    {
        _logger = logger;
        _context = context;
        _fileRepository = fileRepository;
    }

    public async Task<Box?> GetBox(string code)
    {
        var box = await _context.Boxes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code);

        if (box == null || DateTime.UtcNow > box.ExpiresAt || !Directory.Exists(GetBoxPath(code)))
            return null;

        return box;
    }

    public async Task<(BoxOperationResult, Box? createdBox)> CreateBox(string? password = null)
    {
        try
        {
            string code = Code.GenerateBoxCode(12);
            while (Directory.Exists(GetBoxPath(code)))
                code = Code.GenerateBoxCode(12);

            var box = new Box(code);

            if (!string.IsNullOrWhiteSpace(password))
            {
                var hasher = new PasswordHasher<Box>();
                box.Password = hasher.HashPassword(box, password);
            }

            await _context.Boxes.AddAsync(box);
            await _context.SaveChangesAsync();

            Directory.CreateDirectory(GetBoxPath(code));
            return (BoxOperationResult.Success, box);
        }
        catch (Exception err)
        {
            _logger.LogError(err, "Error occurred while creating box.");
            return (BoxOperationResult.Error, null);
        }
    }

    public async Task StreamFile(string code, string fileName, Stream stream)
    {
        var boxPath = GetBoxPath(code);
        var filePath = Path.Combine(boxPath, fileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await stream.CopyToAsync(fileStream);
        }
    }

    public async Task<BoxOperationResult> DeleteBox(string code)
    {
        try
        {
            var box = await _context.Boxes.FirstOrDefaultAsync(x => x.Code == code);

            if (box == null) return BoxOperationResult.NotFound;

            // Marks as expired. Expired boxes will be removed periodically.
            box.ExpiresAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return BoxOperationResult.Success;
        }
        catch (Exception err)
        {
            _logger.LogError(err, "Error occurred while removing box.");
            return BoxOperationResult.Error;
        }
    }

    public string GetBoxPath(string code)
    {
        return Path.Combine(_basePath, code);
    }

    public async Task<List<FileDTO>?> GetFiles(string code)
    {
        try
        {
            var boxPath = Path.Combine(_basePath, code);
            var dirInfo = new DirectoryInfo(boxPath);
            var files = dirInfo.GetFiles();
            var filesDTO = ConvertToDTO.Files(files);

            for (int i = 0; i < filesDTO.Count; i++)
            {
                var originalName = await _fileRepository.GetOriginalFileName(code, filesDTO[i].Name);
                if (originalName != null)
                    filesDTO[i].Name = originalName;
            }

            return filesDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get files for box {code}");
            return null;
        }
    }
}