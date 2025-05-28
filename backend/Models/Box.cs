using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class Box
{
    [Key]
    public string Code { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? Password { get; set; }

    public ICollection<Models.File> Files { get; set; } = new List<Models.File>();

    public Box(string code)
    {
        Code = code;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.AddMinutes(10);
    }
}
