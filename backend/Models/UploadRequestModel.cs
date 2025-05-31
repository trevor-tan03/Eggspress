namespace backend.Models;

public class UploadRequestModel
{
    public IFormFile Chunk { get; set; } = default!;
    public int ChunkNumber { get; set; }
    public int TotalChunks { get; set; }
    public string FileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;

    public static async Task<UploadRequestModel?> FromFormAsync(HttpRequest request)
    {
        if (!request.HasFormContentType)
            return null;

        var form = await request.ReadFormAsync();

        if (!int.TryParse(form["chunkNumber"], out var chunkNum) ||
            !int.TryParse(form["totalChunks"], out var totalChunks) ||
            !form.TryGetValue("fileId", out var fileId) ||
            !form.TryGetValue("fileName", out var fileName) ||
            form.Files["file"] is not { } chunkFile)
            return null;

        return new UploadRequestModel
        {
            Chunk = chunkFile,
            ChunkNumber = chunkNum,
            TotalChunks = totalChunks,
            FileId = fileId!,
            FileName = fileName!
        };
    }
}
