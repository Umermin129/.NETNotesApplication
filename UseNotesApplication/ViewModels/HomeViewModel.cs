using System.ComponentModel.DataAnnotations;

namespace UseNotesApplication.ViewModels
{
    public class TaskEdit
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Please Enter Title ")]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime LastUpdated { get; set; }
    }
    public class HomeViewModel
    {
        public string UserName { get; set; }
        public string email { get; set; }
        public string Name { get; set; }
        public List<TaskEdit> TaskLists { get; set; }
    }
}
