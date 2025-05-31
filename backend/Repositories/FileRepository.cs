using backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace backend.Repositories;

public class FileRepository : IFileRepository
{
    private readonly BoxDbContext _context;

    public FileRepository(BoxDbContext context)
    {
        _context = context;
    }
    public async Task AddFile(string boxCode, string id, string originalFileName, string randomFileName)
    {
        var file = new Models.File
        {
            Id = id,
            OriginalFileName = originalFileName,
            RandomFileName = randomFileName,
            BoxCode = boxCode
        };
        await _context.Files.AddAsync(file);
        await _context.SaveChangesAsync();
    }

    public async Task<Models.File?> GetFileById(string boxCode, string fileId)
    {
        var file = await _context.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == fileId && f.BoxCode == boxCode);
        if (file == null) return null;
        return file;
    }

    public async Task<string?> GetOriginalFileName(string boxCode, string randomFileName)
    {
        var file = await _context.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.BoxCode == boxCode && f.RandomFileName == randomFileName);

        if (file == null)
            throw new FileNotFoundException("No file with the name " + randomFileName);
        return file.OriginalFileName;
    }

    public async Task<string?> GetRandomFileName(string boxCode, string originalFileName)
    {
        var file = await _context.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.BoxCode == boxCode && f.OriginalFileName == originalFileName);

        if (file == null)
            throw new FileNotFoundException("No file with the name " + originalFileName);
        return file.RandomFileName;
    }

    public async Task IncreaseBytesSize(string fileId, long bytesSize)
    {
        var file = await _context.Files
            .FirstOrDefaultAsync(f => f.Id == fileId);

        if (file == null)
            throw new FileNotFoundException("No file with the id " + fileId);

        file.SizeBytes += bytesSize;
        await _context.SaveChangesAsync();
    }
}