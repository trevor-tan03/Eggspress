using backend.Data;
using backend.Models;
using backend.util;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class LocalBoxRepository : IBoxRepository
{
    private readonly ILogger<LocalBoxRepository> _logger;
    private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "Boxes");
    private readonly BoxDbContext _context;

    private string GetBoxPath(string code)
    {
        return Path.Combine(_basePath, code);
    }

    public LocalBoxRepository(ILogger<LocalBoxRepository> logger, BoxDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> BoxExists(string code)
    {
        var box = await _context.Boxes.FirstOrDefaultAsync(x => x.Code == code);

        if (box == null || DateTime.UtcNow > box.ExpiresAt)
            return false;

        return Directory.Exists(Path.Combine(_basePath, code));
    }

    public async Task<List<FileDTO>?> GetFiles(string code)
    {
        var boxExists = await BoxExists(code);
        if (!boxExists)
        {
            _logger.LogError($"Box '{code}' not found when trying to retrieve files");
            return null;
        }

        try
        {
            var boxPath = Path.Combine(_basePath, code);
            var dirInfo = new DirectoryInfo(boxPath);
            var files = dirInfo.GetFiles();
            var filesDTO = ConvertToDTO.GetFilesList(files);
            return filesDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get files for box {code}");
            return null;
        }
    }

    public async Task<string> CreateBox()
    {
        string code = Code.GenerateBoxCode(12);

        while (Directory.Exists(Path.Combine(_basePath, code)))
            code = Code.GenerateBoxCode(12);


        var box = new Box(code);

        await _context.Boxes.AddAsync(box);
        await _context.SaveChangesAsync();
        Directory.CreateDirectory(Path.Combine(_basePath, code));
        return code;
    }

    public async Task<List<FileDTO>?> UploadFiles(string code, List<IFormFile> files)
    {
        try
        {
            var boxPath = GetBoxPath(code);
            if (!Directory.Exists(boxPath))
            {
                _logger.LogWarning("Tried to upload files to non-existent box: {Code}", code);
                return null;
            }

            foreach (var file in files)
            {
                var filePath = Path.Combine(boxPath, file.FileName);
                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
            }

            return await GetFiles(code); // Returns updated files list
        }
        catch (Exception err)
        {
            _logger.LogError(err, "Error occurred while uploading files.");
            return null;
        }
    }
    public async Task<bool> DeleteBox(string code)
    {
        try
        {
            var box = await _context.Boxes.FirstOrDefaultAsync(x => x.Code == code);
            if (box == null) return false;

            _context.Boxes.Remove(box);
            await _context.SaveChangesAsync();

            var boxPath = GetBoxPath(code);
            Directory.Delete(boxPath, recursive: true);
            return !Directory.Exists(boxPath);
        }
        catch (Exception err)
        {
            _logger.LogError(err, "Error occurred while removing box.");
            return false;
        }
    }
}