namespace backend.util;

public record FileDTO(string name, long size);

public class ConvertToDTO()
{
    public static List<FileDTO> GetFilesList(FileInfo[] files)
    {
        var list = new List<FileDTO>();

        foreach (FileInfo file in files)
            list.Add(new FileDTO(file.Name, file.Length));

        return list;
    }
}