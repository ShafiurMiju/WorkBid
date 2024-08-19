using System.ComponentModel.DataAnnotations;

namespace WorkBid.Models
{
    public class Notice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
