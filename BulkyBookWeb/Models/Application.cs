using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WorkBid.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Member")]
        [DisplayName("Member ID")]
        public int mId { get; set; }
        public Member Member { get; set; }

        [Required]
        [ForeignKey("Job")]
        [DisplayName("Job ID")]
        public int jId { get; set; }
        public Job Job { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

    }
}
