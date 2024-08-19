using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WorkBid.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
