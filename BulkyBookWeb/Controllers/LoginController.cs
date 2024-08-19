using BulkyBookWeb.Data;
using Microsoft.AspNetCore.Mvc;
using BulkyBookWeb.Models;
using WorkBid.Models;
using Microsoft.AspNetCore.Authentication;


namespace WorkBid.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LoginController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            int memberId = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var obj = _db.Members.FirstOrDefault(m => m.Id == memberId);

            if (obj != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Member obj)
        {
            int memberId = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var loggedMember = _db.Members.FirstOrDefault(m => m.Id == memberId);

            if (loggedMember != null)
            {
                return RedirectToAction("Index", "Home");
            }


            if (obj.PhoneNumber!=null && obj.Password!=null)
            {
                var member = _db.Members.FirstOrDefault(m => m.PhoneNumber == obj.PhoneNumber && m.Password == obj.Password);

                if (member != null)
                {
                    HttpContext.Session.SetString("LoggedInUser", member.Id.ToString());
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("password", "Account not found.");
            }

            return View(obj);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("LoggedInUser");
            return RedirectToAction("Index", "Login");
        }
    }
}
