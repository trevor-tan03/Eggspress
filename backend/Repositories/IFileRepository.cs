namespace backend.Repositories;

public interface IFileRepository
{
    public Task AddFile(string boxCode, string id, string originalFileName, string randomFileName);
    public Task<Models.File?> GetFileById(string boxCode, string fileId);
    public Task<string?> GetOriginalFileName(string boxCode, string randomFileName);
}