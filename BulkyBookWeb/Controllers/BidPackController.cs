using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkBid.Authorization;
using WorkBid.Models;

namespace WorkBid.Controllers
{
    public class BidPackController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BidPackController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var obj = _db.Members.FirstOrDefault(m => m.Id == id);

            if (obj == null)
            {
                return RedirectToAction("Index", "Login");
            }

            IEnumerable<Bid> objBidList = _db.Bids;

            return View(objBidList);
        }

        [HttpPost]
        public IActionResult Buy(int id)
        {
            var bidPackage = _db.Bids.Find(id);

            if (bidPackage == null)
            {
                return Json(new { success = false, message = "Package Not Found" });
            }

            int memberId = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));
            var member = _db.Members.Find(memberId);

            if (member == null)
            {
                return Json(new { success = false, message = "User Not Found" });
            }

            if (member.Balance < bidPackage.Price)
            {
                return Json(new { success = false, message = "Insufficient funds to buy this bid package." });
            }

            member.Bids += bidPackage.Bids;
            member.Balance -= bidPackage.Price;

            _db.SaveChanges();

            return Json(new { success = true, message = "You have successfully purchased the bid package!" });
        }

    }
}
