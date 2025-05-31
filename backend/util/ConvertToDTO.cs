using backend.Models;

namespace backend.util;

public class FileDTO
{
    public string Name { get; set; }
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class ConvertToDTO()
{
    public static List<FileDTO> Files(FileInfo[] files)
    {
        var list = new List<FileDTO>();

        foreach (FileInfo file in files)
            list.Add(new FileDTO
            {
                Name = file.Name,
                Size = file.Length,
                UploadedAt = file.CreationTime
            });

        return list;
    }

    public static BoxDTO Box(Box box, List<FileDTO> files)
    {
        var boxSize = files.Sum(f => f.Size);
        return new BoxDTO(box.Code, box.ExpiresAt, boxSize, files);
    }
}