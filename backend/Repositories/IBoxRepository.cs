using backend.Models;
using backend.util;

namespace backend.Repositories;

public interface IBoxRepository
{
    public Task<Box?> GetBox(string code);
    public Task<(BoxOperationResult, Box? createdBox)> CreateBox(string? password = null);
    public List<FileDTO>? GetFiles(string code);
    public Task<(BoxOperationResult, List<FileDTO>?)> UploadFiles(string code, List<IFormFile> files);
    public Task<BoxOperationResult> DeleteBox(string code);
}

public enum BoxOperationResult
{
    NotFound,
    Error,
    Success
}