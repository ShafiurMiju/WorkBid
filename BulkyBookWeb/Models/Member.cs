using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WorkBid.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LName { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int Bids { get; set; } = 0;

        [Required]
        public int Balance { get; set; } = 0;

        [Required]
        public int WithdrawalBalance { get; set; } = 0;

        [Required]
        public int TotalWin { get; set; } = 0;

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
