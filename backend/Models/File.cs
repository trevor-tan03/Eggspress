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

        public string BoxCode { get; set; } = default!;
        public Box? Box { get; set; }
    }
}
