using System.ComponentModel.DataAnnotations;

namespace UseNotesApplication.ViewModels.Login
{
   
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }

        public List<LoginImage> GridImages { get; set; }
        public List<int> SelectedImageIds { get; set; } = new List<int>();
    }
}
