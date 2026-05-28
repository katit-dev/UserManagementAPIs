using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Models
{
    public class User
    {
       [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Age { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool Deleted { get; set; }
    }
}