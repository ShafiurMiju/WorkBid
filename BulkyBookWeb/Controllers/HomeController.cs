using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;
using WorkBid.Models;

namespace BulkyBookWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider)
        {
            _logger = logger;
            _db = db;
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
        }

        [HttpPost]
        public IActionResult ApplyForJob(int jobId)
        {
            var job = _db.Jobs.Find(jobId);
            if (job == null)
            {
                return Json(new { success = false, message = "Job not found." });
            }

            var userIdStr = HttpContext.Session.GetString("LoggedInUser");
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var user = _db.Members.Find(userId);
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (user.Bids < 1)
            {
                return Json(new { success = false, message = "You do not have enough bids to apply for this job." });
            }

            var existingApplication = _db.Applications.Any(a => a.mId == userId && a.jId == jobId);
            if (existingApplication)
            {
                return Json(new { success = false, message = "You have already applied for this job." });
            }

            var application = new Application
            {
                mId = userId,
                jId = jobId
            };

            _db.Applications.Add(application);
            user.Bids -= 1;
            job.Bidder += 1;
            _db.SaveChanges();

            return Json(new { success = true, message = "You have successfully applied for the job." });
        }

        [HttpGet]
        public IActionResult Index(int page, int pageSize = 10)
        {
            if (page==0)
            {
                page = 1;
            }
            int skip = (page - 1) * pageSize;
            int id = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            if (id == 0)
            {
                return RedirectToAction("Index", "Login");
            }

            var appliedJobIds = _db.Applications
                .Where(a => a.mId == id)
                .Select(a => a.jId)
                .ToList();

            var jobs = _db.Jobs
                .Where(job => !appliedJobIds.Contains(job.Id) && job.wId == null)
                .OrderByDescending(job => job.CreatedDateTime)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            var totalJobs = _db.Jobs.Count(job => !appliedJobIds.Contains(job.Id) && job.wId == null);

            ViewBag.TotalPages = (int)Math.Ceiling(totalJobs / (double)pageSize);
            ViewBag.CurrentPage = page;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var html = RenderRazorViewToString("Index", jobs);
                return Content(html, "text/html");
            }

            return View(jobs);
        }

        private string RenderRazorViewToString(string viewName, object model = null)
        {
            var actionContext = new ActionContext
            {
                HttpContext = HttpContext,
                RouteData = RouteData,
                ActionDescriptor = ControllerContext.ActionDescriptor
            };

            using var stringWriter = new StringWriter();
            var viewResult = _razorViewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: false);

            if (!viewResult.Success)
            {
                var findViewResult = _razorViewEngine.FindView(actionContext, viewName, false);
                if (!findViewResult.Success)
                {
                    throw new InvalidOperationException($"Couldn't find view {viewName}");
                }

                viewResult = findViewResult;
            }

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                ViewData,
                TempData,
                stringWriter,
                new HtmlHelperOptions()
            );

            if (model != null)
            {
                ViewData.Model = model;
            }

            viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();

            return stringWriter.GetStringBuilder().ToString();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var obj = _db.Members.FirstOrDefault(m => m.Id == id);

            if (obj == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public IActionResult ContactUs(ContactUs model)
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var obj = _db.Members.FirstOrDefault(m => m.Id == id);

            if (obj == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                _db.ContactUs.Add(model);
                _db.SaveChanges();

                TempData["Message"] = "Your message has been sent successfully!";
                return RedirectToAction("ContactUs");
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
