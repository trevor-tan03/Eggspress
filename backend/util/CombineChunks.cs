namespace backend.util;

public static class Chunks
{
    public static async Task Combine(string chunkFolder, string finalPath)
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
}