using BulkyBookWeb.Models;
using Microsoft.EntityFrameworkCore;
using WorkBid.Models;

namespace BulkyBookWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Category> Categories {  get; set; }
        public DbSet<Member> Members {  get; set; }
        public DbSet<Job> Jobs {  get; set; }
        public DbSet<Bid> Bids {  get; set; }
        public DbSet<ContactUs> ContactUs {  get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Deposite> Deposites { get; set; }
        public DbSet<Withdraw> Withdraws { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Notice> Notices { get; set; }

    }

}
