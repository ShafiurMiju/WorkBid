namespace WorkBid.Models
{
    public class JobListViewModel
    {
        public IEnumerable<Job> Jobs { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
