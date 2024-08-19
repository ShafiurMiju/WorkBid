using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WorkBid.Models
{
    public class Deposite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Member")]
        [DisplayName("Member ID")]
        public int mId { get; set; }
        public Member Member { get; set; }

        [Required]
        [DisplayName("Deposite Type")]
        public string DepositeType { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(50, 10000,ErrorMessage = "Amount must be between 50 to 10000")]
        public int Amount { get; set; }

        [Required]
        [DisplayName("Transection Id")]
        public string TId {  get; set; }

        [Required]
        [DisplayName("Status")]
        public bool IsStatus { get; set; } = false;

        public string comment {  get; set; }

        [Required]
        [DisplayName("Status")]
        public bool IsRejected { get; set; } = false;

        public string Admincomment { get; set; } = "";

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

    }
}
