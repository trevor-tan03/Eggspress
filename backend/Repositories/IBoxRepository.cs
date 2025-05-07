using backend.util;

namespace backend.Repositories;

public interface IBoxRepository
{
    public Task<bool> BoxExists(string code);
    public Task<List<FileDTO>?> GetFiles(string code);
    public Task<string> CreateBox();
    public Task<List<FileDTO>?> UploadFiles(string code, List<IFormFile> files);
    public Task<bool> DeleteBox(string code);
}