using System.ComponentModel.DataAnnotations;
namespace ViewModel.Notes
{
    public class TaskEditViewModel
    {
       
            public int Id { get; set; }
            [Required(ErrorMessage = "Please Enter Title ")]
            public string Title { get; set; }
            public string? Description { get; set; }
            public string Status { get; set; } = "Pending";
            public DateTime LastUpdated { get; set; } 
    }
}
