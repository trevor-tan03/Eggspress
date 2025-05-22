using backend.Models;

namespace backend.util;

public record FileDTO(string Name, long Size, DateTime UploadedAt);

public class ConvertToDTO()
{
    public static List<FileDTO> Files(FileInfo[] files)
    {
        var list = new List<FileDTO>();

        foreach (FileInfo file in files)
            list.Add(new FileDTO(file.Name, file.Length, file.CreationTime));

        return list;
    }

    public static BoxDTO Box(Box box, List<FileDTO> files)
    {
        return new BoxDTO(box.Code, box.ExpiresAt, files);
    }
}