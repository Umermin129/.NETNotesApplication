using System.ComponentModel.DataAnnotations;

namespace UseNotesApplication.ViewModels
{
    public class RegistrationViewModel
    {
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
    }
}
