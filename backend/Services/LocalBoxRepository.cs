using backend.util;

namespace backend.Repositories;

public class LocalBoxRepository : IBoxRepository
{
    private readonly ILogger<LocalBoxRepository> _logger;
    private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "Boxes");
    private string GetBoxPath(string code)
    {
        return Path.Combine(_basePath, code);
    }

    public LocalBoxRepository(ILogger<LocalBoxRepository> logger)
    {
        _logger = logger;
    }

    public bool BoxExists(string code)
    {
        return Directory.Exists(Path.Combine(_basePath, code));
    }

    public List<FileDTO>? GetFiles(string code)
    {
        if (!Directory.Exists(Path.Combine(_basePath, code)))
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

    public string CreateBox()
    {
        string code = Code.GenerateBoxCode(12);

        while (Directory.Exists(Path.Combine(_basePath, code)))
            code = Code.GenerateBoxCode(12);

        // var expiryDate = DateTime.Now.AddMinutes(10);
        Directory.CreateDirectory(Path.Combine(_basePath, code));
        return code;
    }

    public async Task<List<FileDTO>?> UploadFiles(string code, List<IFormFile> files)
    {
        try
        {
            var boxPath = GetBoxPath(code);

            foreach (var file in files)
            {
                var filePath = Path.Combine(boxPath, file.FileName);
                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
            }

            return GetFiles(code); // Returns updated files list
        }
        catch (Exception err)
        {
            _logger.LogError(err, "Error occurred while uploading files.");
            return null;
        }
    }
    public bool DeleteBox(string code)
    {
        try
        {
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