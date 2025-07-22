using System.ComponentModel.DataAnnotations;

namespace UseNotesApplication.ViewModels
{
    public class RegistrationViewModel
    {
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please upload exactly 5 images.")]
        public List<IFormFile> Images { get; set; }

        [Required(ErrorMessage = "Provide a sequence number for each image.")]
        public List<int> Sequence { get; set; }

    }
}
