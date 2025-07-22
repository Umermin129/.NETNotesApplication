using System.ComponentModel.DataAnnotations;

namespace UseNotesApplication.Models
{
    public class Notes
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "Pending";
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public int UsersId { get; set; }
        public Users Users { get; set; }
        public ICollection<NoteVersion> Versions { get; set; }

    }
}
