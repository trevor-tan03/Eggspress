using backend.util;

namespace backend.Repositories;

public interface IBoxRepository
{
    public bool BoxExists(string code);
    public List<FileDTO>? GetFiles(string code);
    public string CreateBox();
    public Task<List<FileDTO>?> UploadFiles(string code, List<IFormFile> files);
    public bool DeleteBox(string code);
}