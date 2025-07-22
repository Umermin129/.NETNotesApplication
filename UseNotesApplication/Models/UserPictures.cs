using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UseNotesApplication.Models
{
    public class UserPictures
    {
        public int Id { get; set; }
        [Required]
        public string ImageURI { get; set; }
        [Required]
        [Range(1,5)]
        public int Sequence { get; set; }
        [Required]
        
        public int UsersId { get; set; }
        [ForeignKey("UsersId")]
        public Users Users { get; set; }

    }
}
