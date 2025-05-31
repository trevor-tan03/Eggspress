namespace backend.Services;

public interface IUploadService
{
    Task<bool> TryEnforceBoxSizeLimitAsync(string boxCode, string fileId, long chunkSize);
    Task SaveChunkAsync(IFormFile chunk, string path);
    Task<bool> CombineIfComplete(string boxCode, string fileId, int totalChunks, string outputPath);
}