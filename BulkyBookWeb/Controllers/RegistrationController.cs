using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;
using WorkBid.Models;

namespace WorkBid.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RegistrationController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Member obj)
        {
            if (ModelState.IsValid)
            {
                var MemberFromDb = _db.Members.FirstOrDefault(m => m.PhoneNumber == obj.PhoneNumber);

                if (MemberFromDb == null)
                {
                    _db.Members.Add(obj);
                    _db.SaveChanges();

                    return RedirectToAction("Index", "Login");
                }

                ModelState.AddModelError("phonenumber", "A member with this phone number already exists.");

            }

            return View(obj);
        }
    }
}
