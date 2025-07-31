using System.ComponentModel.DataAnnotations;

namespace ViewModel.Home
{
    public class ProfileViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string UserName { get; set; }
    }
}
