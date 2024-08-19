using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WorkBid.Models
{
    public class Withdraw
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Member")]
        [DisplayName("Member ID")]
        public int mId { get; set; }
        public Member Member { get; set; }

        [Required]
        [DisplayName("Withdraw Type")]
        public string WithdrawType { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(1000, 100000, ErrorMessage = "Amount must be between 1000 to 100000")]
        public int Amount { get; set; }

        [Required]
        [DisplayName("Transaction ID")]
        public string TId { get; set; } = "";

        [Required]
        [DisplayName("Status")]
        public bool IsStatus { get; set; } = false;

        [Required]
        [DisplayName("Rejected")]
        public bool IsRejected { get; set; } = false;

        public string Comment { get; set; } = ""; // Optional comment from the user

        public string AdminComment { get; set; } = ""; // Comment from admin

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
