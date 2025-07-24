using System.ComponentModel.DataAnnotations;

namespace UseNotesApplication.ViewModels
{
    public class TaskEdit
    {
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime LastUpdated { get; set; }
    }
    public class HomeViewModel
    {
        public string email { get; set; }
        public string Name { get; set; }
        public List<TaskEdit> NoteVersions { get; set; }
    }
}
