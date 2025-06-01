using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class File
    {
        [Key]
        public string? Id { get; set; }
        // Display file with OriginalFileName
        public string? OriginalFileName { get; set; }
        // Store file with RandomFileName
        public string? RandomFileName { get; set; }
        [Required]
        public int TotalChunks { get; set; }
        [Required]
        public int ChunkCount { get; set; }

        public string BoxCode { get; set; } = default!;
        public long SizeBytes { get; set; }
        public Box? Box { get; set; }
    }
}
