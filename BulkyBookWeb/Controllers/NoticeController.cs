using BulkyBookWeb.Data;
using Microsoft.AspNetCore.Mvc;

namespace WorkBid.Controllers
{
    public class NoticeController : Controller
    {

        private readonly ApplicationDbContext _db;

        public NoticeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var obj = _db.Members.FirstOrDefault(m => m.Id == id);

            if (obj == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var notices = _db.Notices
                .Where(n => n.IsActive)
                .OrderByDescending(n => n.DatePosted)
                .ToList();

            return View(notices);
        }
    }
}
