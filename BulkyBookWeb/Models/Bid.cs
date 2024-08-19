using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WorkBid.Models
{
    public class Bid
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Bids { get; set; }

        [Required]
        public int Price { get; set; }
    }
}
