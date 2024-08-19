using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkBid.Authorization;
using WorkBid.Models;

namespace WorkBid.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            int admin = Convert.ToInt32(HttpContext.Session.GetString("LoggedAdmin"));

            var obj = _db.Admins.FirstOrDefault(m => m.Id == admin);

            if (obj != null)
            {
                return RedirectToAction("AllJob", "Admin");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Admin obj)
        {
            int admin = Convert.ToInt32(HttpContext.Session.GetString("LoggedAdmin"));

            var loggedAdmin = _db.Admins.FirstOrDefault(m => m.Id == admin);

            if (loggedAdmin != null)
            {
                return RedirectToAction("AllJob", "Admin");
            }


            if (obj.UserName != null && obj.Password != null)
            {
                var adminObj = _db.Admins.FirstOrDefault(m => m.UserName == obj.UserName && m.Password == obj.Password);

                if (adminObj != null)
                {
                    HttpContext.Session.SetString("LoggedAdmin", adminObj.Id.ToString());
                    return RedirectToAction("AllJob", "Admin");
                }

                ModelState.AddModelError("password", "Account not found.");
            }

            return View(obj);
        }

        [HttpGet]
        [AdminLogged]
        public IActionResult AllJob()
        {
            IEnumerable<Job> objJobList = _db.Jobs;

            return View(objJobList);
        }

        [HttpGet]
        [AdminLogged]
        public IActionResult CreateJob()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminLogged]
        public IActionResult CreateJob(Job obj)
        {
            if (obj.JobTitle!=null && obj.JobDescription!=null && obj.Price!=null && obj.JobQuantity!=null)
            {
                _db.Jobs.Add(obj);
                _db.SaveChanges();

                return RedirectToAction("alljob", "Admin");
            }

            return View(obj);
        }

        [HttpGet]
        [AdminLogged]
        public IActionResult SelectWinner(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var job = _db.Jobs
                .Include(j => j.Applications)
                .ThenInclude(a => a.Member)
                .FirstOrDefault(j => j.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminLogged]
        public IActionResult winnerSelect(Job obj)
        {
            var job = _db.Jobs
                .Include(j => j.Applications)
                .ThenInclude(a => a.Member)
                .FirstOrDefault(j => j.Id == obj.Id);

            if (job == null)
            {
                return NotFound();
            }

            var winner = job.Applications.FirstOrDefault(a => a.Member.Id == obj.wId)?.Member;

            if (winner == null)
            {
                return BadRequest("Winner not found.");
            }

            job.wId = obj.wId;
            winner.TotalWin += 1;

            _db.SaveChanges();

            return RedirectToAction("alljob", "Admin");
        }

        [HttpGet]
        [AdminLogged]
        public IActionResult DepositRequests()
        {
            var pendingDeposits = _db.Deposites
                .Include(d => d.Member)
                .Where(d => !d.IsStatus)
                .ToList();

            return View(pendingDeposits);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminLogged]
        public IActionResult ApproveDeposit(int id)
        {
            var deposit = _db.Deposites.Include(d => d.Member).FirstOrDefault(d => d.Id == id);
            if (deposit == null)
            {
                return NotFound();
            }

            deposit.IsStatus = true;
            deposit.Member.Balance += deposit.Amount;

            _db.SaveChanges();

            TempData["SuccessMessage"] = "Deposit approved successfully!";
            return RedirectToAction("DepositRequests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminLogged]
        public IActionResult RejectDeposit(Deposite obj)
        {
            var deposit = _db.Deposites.Find(obj.Id);

            if (deposit == null)
            {
                return NotFound();
            }

            deposit.IsStatus = true;
            deposit.IsRejected = true;
            if (obj.Admincomment == "")
            {
                deposit.Admincomment = "";
            }
            else
            {
                deposit.Admincomment = obj.Admincomment;
            }

            _db.SaveChanges();

            TempData["SuccessMessage"] = "Deposit rejected successfully!";
            return RedirectToAction("DepositRequests");
        }

        [HttpGet]
        [AdminLogged]
        public IActionResult AdminWithdrawals()
        {
            var withdrawRequests = _db.Withdraws
                .Include(w => w.Member)
                .OrderByDescending(w => w.CreatedDateTime)
                .ToList();

            var withdrawNumber = _db.Wallets
                .Include(w => w.Member)
                .ToList();


            if (withdrawNumber==null)
            {
                ViewBag.withdrawNumber = null;
            }
            else
            {
                ViewBag.withdrawNumber = withdrawNumber;
            }

            return View(withdrawRequests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminLogged]
        public IActionResult ProcessWithdraw(Withdraw obj, bool approve)
        {
            var withdrawRequest = _db.Withdraws.Find(obj.Id);

            if (withdrawRequest == null)
            {
                return NotFound();
            }

            var member = _db.Members.Find(withdrawRequest.mId);

            if (member == null)
            {
                return NotFound();
            }

            if (approve)
            {
                if (member.WithdrawalBalance >= withdrawRequest.Amount)
                {
                    if (obj.TId==null)
                    {
                        return RedirectToAction("AdminWithdrawals");
                    }
                    withdrawRequest.TId = obj.TId;
                    member.WithdrawalBalance -= withdrawRequest.Amount;
                    withdrawRequest.IsStatus = true;
                    withdrawRequest.AdminComment = "Approved";
                }
                else
                {
                    withdrawRequest.IsRejected = true;
                    withdrawRequest.AdminComment = "Insufficient balance for withdrawal";
                }
            }
            else
            {
                withdrawRequest.IsRejected = true;
                if (obj.AdminComment != null)
                {
                    withdrawRequest.AdminComment = obj.AdminComment;
                }
                else
                {
                    withdrawRequest.AdminComment = "Rejected by admin";
                }
            }

            _db.Members.Update(member);
            _db.Withdraws.Update(withdrawRequest);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Withdrawal processed successfully!";
            return RedirectToAction("AdminWithdrawals");
        }

        [HttpGet]
        [AdminLogged]
        public IActionResult Notice()
        {

            var notices = _db.Notices
                .OrderByDescending(n => n.DatePosted)
                .ToList();

            ViewBag.notices = notices;

            return View();
        }

        [HttpPost]
        [AdminLogged]

        public IActionResult Notice(Notice model)
        {

            if (ModelState.IsValid)
            {
                model.DatePosted = DateTime.Now;
                _db.Notices.Add(model);
                _db.SaveChanges();
                TempData["SuccessMessage"] = "Notice created successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create notice. Please check the input.";
            }

            var notices = _db.Notices
                .OrderByDescending(n => n.DatePosted)
                .ToList();

            ViewBag.notices = notices;

            return View(model);
        }

        [HttpPost]
        [AdminLogged]

        public IActionResult DeleteNotice(int id)
        {
            var notice = _db.Notices.FirstOrDefault(n => n.Id == id);
            if (notice != null)
            {
                _db.Notices.Remove(notice);
                _db.SaveChanges();
                TempData["SuccessMessage"] = "Notice deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Notice not found!";
            }

            return RedirectToAction("Notice");
        }

        [HttpPost]
        [AdminLogged]
        public IActionResult ActivateNotice(int id)
        {
            var notice = _db.Notices.FirstOrDefault(n => n.Id == id);
            if (notice != null)
            {
                notice.IsActive = true;
                _db.SaveChanges();
                TempData["SuccessMessage"] = "Notice activated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Notice not found!";
            }

            return RedirectToAction("Notice");
        }

        [HttpPost]
        [AdminLogged]
        public IActionResult DeactivateNotice(int id)
        {
            var notice = _db.Notices.FirstOrDefault(n => n.Id == id);
            if (notice != null)
            {
                notice.IsActive = false;
                _db.SaveChanges();
                TempData["SuccessMessage"] = "Notice deactivated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Notice not found!";
            }

            return RedirectToAction("Notice");
        }

        [HttpPost]
        [AdminLogged]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("LoggedAdmin");
            return RedirectToAction("Index", "Admin");
        }
    }
}
