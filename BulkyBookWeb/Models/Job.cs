using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkBid.Models
{
    public class Job
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Job Title")]
        public string JobTitle { get; set; }

        [Required]
        [DisplayName("Job Description")]
        public string JobDescription { get; set; }

        [Required]
        [DisplayName("Job Quantity")]
        public int JobQuantity { get; set; }

        public int Bidder { get; set; } = 0;

        [Required]
        public int Price { get; set; }

        [DisplayName("Is Completed")]
        public bool IsCompleted { get; set; } = false;

        public int? wId { get; set; } = null;
        [ForeignKey("wId")]
        public Member Winner { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        public ICollection<Application> Applications { get; set; }

    }
}
