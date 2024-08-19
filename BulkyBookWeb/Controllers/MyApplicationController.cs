using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkBid.Models;

namespace WorkBid.Controllers
{
    public class MyApplicationController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MyApplicationController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            ViewData["LoggedInUserId"] = id;

            var obj = _db.Members.FirstOrDefault(m => m.Id == id);

            if (obj == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var application = _db.Applications
                .Include(a => a.Member)
                .Include(a => a.Job)
                .ThenInclude(j => j.Winner)
                .Where(a => a.mId == id)
                .ToList();

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        

    }
}
