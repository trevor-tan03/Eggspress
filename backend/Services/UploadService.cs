using backend.Repositories;
using backend.util;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace backend.Services;

public class UploadService : IUploadService
{
    //private readonly ILogger _logger;
    private readonly IFileRepository _fileRepository;
    private readonly IBoxRepository _boxRepository;
    private readonly long MAX_BOX_SIZE;

    public UploadService(IFileRepository fileRepository, IBoxRepository boxRepository)
    {
        //_logger = logger;
        _fileRepository = fileRepository;
        _boxRepository = boxRepository;
        MAX_BOX_SIZE = 5L * 1024 * 1024 * 1024;
    }

    private async Task CombineChunks(string chunkFolder, string finalPath)
    {
        var chunkPaths = Directory.GetFiles(chunkFolder)
            .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)));

        await using (var finalStream = new FileStream(finalPath, FileMode.Create))
        {
            foreach (var chunkPath in chunkPaths)
            {
                await using (var chunkStream = File.OpenRead(chunkPath))
                {
                    await chunkStream.CopyToAsync(finalStream);
                }
                File.Delete(chunkPath);
            }
        }
    }

    public async Task<bool> TryEnforceBoxSizeLimitAsync(string boxCode, string fileId, long chunkSize)
    {
        var boxSize = await _boxRepository.GetBoxSize(boxCode);
        return await _fileRepository.TryAddChunkSize(boxCode, fileId, chunkSize, boxSize, MAX_BOX_SIZE);
    }

    public async Task SaveChunkAsync(IFormFile chunk, string path)
    {
        await using (var stream = new FileStream(path, FileMode.Create))
        {
            await chunk.CopyToAsync(stream);
        }
    }

    public async Task<bool> CombineIfComplete(string boxCode, string fileId, int totalChunks, string chunkPath)
    {
        var file = await _fileRepository.GetFileById(boxCode, fileId);
        if (file == null)
            return false;

        var chunkFiles = Directory.GetFiles(chunkPath);
        if (chunkFiles.Length != totalChunks)
            return false;

        try
        {
            var boxPath = _boxRepository.GetBoxPath(boxCode);
            var filePath = Path.Combine(boxPath, file.RandomFileName!);

            await CombineChunks(chunkPath, filePath);
            Directory.Delete(chunkPath, recursive: true); // Cleanup
            return true;
        }
        catch (Exception ex)
        {
            //_logger.LogError($"Error combining chunks: {ex.Message}");
            Console.WriteLine($"Error combining chunks: {ex.Message}");
            return false;
        }
    }
}