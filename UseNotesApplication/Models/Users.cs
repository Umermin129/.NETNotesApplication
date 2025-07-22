using System.ComponentModel.DataAnnotations;

namespace UseNotesApplication.Models
{
    public class Users
    {
        public int Id{get; set;}
        [Required,MaxLength(50)]
        public String UserName { get; set;}
        [Required,MaxLength(100)]
        public string Name{get; set;}
        [Required,EmailAddress]
        public string Email {  get; set;}
        public ICollection<UserPictures> Pictures { get; set;}
        public ICollection<Notes> Notes { get; set;}
    }
}
