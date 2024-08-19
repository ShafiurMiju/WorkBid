using BulkyBookWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkBid.Models;

namespace WorkBid.Controllers
{
    public class MyJob : Controller
    {
        private readonly ApplicationDbContext _db;

        public MyJob(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult RunningJob()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var obj = _db.Members.FirstOrDefault(m => m.Id == id);

            if (obj == null)
            {
                return RedirectToAction("Index", "Login");
            }


            var jobs = _db.Jobs
                .Include(j => j.Winner)
                .Where(j => j.wId == id && j.IsCompleted == false)
                .ToList();

            return View(jobs);
        }

        [HttpGet]
        public IActionResult StartJob(int? id)
        {
            int memberId = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var job = _db.Jobs.FirstOrDefault(j => j.Id == id);

            if (job == null || job.wId != memberId)
            {
                return RedirectToAction("RunningJob");
            }

            if (job.IsCompleted == true)
            {
                TempData["ErrorMessage"] = "Job Already Completed.";
                return RedirectToAction("RunningJob");
            }

            // Generate random strings for the captcha-like task
            var randomStrings = new List<string>();
            for (int i = 0; i < job.JobQuantity; i++)
            {
                randomStrings.Add(Guid.NewGuid().ToString().Substring(0, 5));
            }

            ViewBag.RandomStrings = randomStrings;

            return View(job);
        }

        public IActionResult CompletedJobs()
        {
            int memberId = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var member = _db.Members.FirstOrDefault(m => m.Id == memberId);

            if (member == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var completedJobs = _db.Jobs
                .Include(j => j.Winner)
                .Where(j => j.wId == memberId && j.IsCompleted)
                .ToList();

            return View(completedJobs);
        }


        [HttpPost]
        public IActionResult CompleteJob(int jobId, List<string> userInputs)
        {
            int memberId = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var job = _db.Jobs.FirstOrDefault(j => j.Id == jobId);

            if (job == null || job.wId != memberId)
            {
                return RedirectToAction("RunningJob");
            }

            if (job.IsCompleted == true)
            {
                TempData["ErrorMessage"] = "Job Already Completed.";
                return RedirectToAction("RunningJob");
            }

            // Validate the user inputs
            bool isValid = true; // Replace with actual validation logic
            if (!isValid)
            {
                TempData["ErrorMessage"] = "Some inputs are incorrect. Please try again.";
                return View("StartJob", job);
            }

            // Mark the job as completed
            job.IsCompleted = true;
            _db.SaveChanges();

            // Update member's balance
            var member = _db.Members.FirstOrDefault(m => m.Id == job.wId);
            if (member != null)
            {
                member.WithdrawalBalance += job.Price;
                _db.SaveChanges();
            }

            return RedirectToAction("CompletedJobs");
        }


    }
}
