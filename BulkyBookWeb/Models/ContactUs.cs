using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WorkBid.Models
{
    public class ContactUs
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Enter your Name.")]
        [DisplayName("Your Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter Your Email Address.")]
        [EmailAddress(ErrorMessage = "Please Enter a Valid Email Address.")]
        [StringLength(100, ErrorMessage = "Email Can Not Exceed 100 Characters.")]
        [DisplayName("Your Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter a Subject.")]
        [StringLength(200, ErrorMessage = "Subject Can Not Exceed 200 Characters.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please Enter Your Message.")]
        [StringLength(1000, ErrorMessage = "Message Can Not Exceed 1000 Characters.")]
        [DisplayName("Your Message")]
        public string Message { get; set; }
    }
}
