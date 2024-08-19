using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WorkBid.Models
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Member")]
        [DisplayName("Member ID")]
        public int mId { get; set; }
        public Member Member { get; set; }

        [Required]  
        [DisplayName("Wallet Name")]
        public string WalletName { get; set; }        
        
        [Required]  
        [DisplayName("Wallet Number")]
        public string WalletNumber { get; set; }
    }
}
