using System.ComponentModel.DataAnnotations;

namespace UseNotesApplication.ViewModels
{
    public class LoginImage()
    {
        public int Id { get; set; }
        public string ImageURI { get; set; }
    }
    public class LoginViewModel
    {
        [Required]
        public String UserName { get; set; }

        public List<LoginImage> GridImages { get; set; } 
        public List<int> SelectedImageIds { get; set; } = new();
    }
}
