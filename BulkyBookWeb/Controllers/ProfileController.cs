using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkBid.Models;

namespace WorkBid.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProfileController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            int memberId = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var obj = _db.Members.FirstOrDefault(m => m.Id == memberId);

            if (obj==null)
            {
                return RedirectToAction("Index", "Login");   
            }

            return View(obj);
        }

        public IActionResult PaymentMethod()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var obj = _db.Members.FirstOrDefault(m => m.Id == id);

            if (obj == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var application = _db.Wallets
                .Include(a => a.Member)
                .Where(a => a.mId == id)
                .ToList();

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        [HttpGet]
        public IActionResult AddPaymentMethod()
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
        [ValidateAntiForgeryToken]
        public IActionResult AddPaymentMethod(Wallet obj)
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));

            var memberLogged = _db.Members.FirstOrDefault(m => m.Id == id);

            if (memberLogged == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (obj.WalletName!=null && obj.WalletNumber!=null)
            {
                var existingWallet = _db.Wallets
                                    .FirstOrDefault(w => w.WalletName == obj.WalletName && w.mId == id);

                if (existingWallet != null)
                {
                    TempData["ErrorMessage"] = obj.WalletName + " already exists.";
                    return RedirectToAction("PaymentMethod", "Profile");
                }

                obj.mId = id;

                _db.Wallets.Add(obj);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Wallet added successfully.";

                return RedirectToAction("PaymentMethod", "Profile");
            }

            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePaymentMethod(int? id)
        {
            var walletFromDb = _db.Wallets.Find(id);

            if (walletFromDb == null)
            {
                TempData["ErrorMessage"] = "Wallet not found or you do not have permission to delete this wallet.";
                return RedirectToAction("PaymentMethod", "Profile");
            }

            _db.Wallets.Remove(walletFromDb);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Wallet deleted successfully.";

            return RedirectToAction("PaymentMethod", "Profile");
        }

        public IActionResult Deposit()
        {
            int memberId = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));
            var member = _db.Members.FirstOrDefault(m => m.Id == memberId);

            if (member == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var depositHistory = _db.Deposites
                .Where(d => d.mId == memberId)
                .OrderByDescending(d => d.CreatedDateTime)
                .ToList();

            if (depositHistory==null)
            {
                ViewBag.DepositHistory = null;
            }
            else
            {
                ViewBag.DepositHistory = depositHistory;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deposit(Deposite model)
        {
            if (model.DepositeType != null && model.TId != null && model.Amount != 0)
            {
                // Check if TId is unique
                bool isTIdUnique = !_db.Deposites.Any(d => d.TId == model.TId);

                if (!isTIdUnique)
                {
                    TempData["ErrorMessage"] = "The Transaction ID is already used. Please use a unique Transaction ID.";
                    return View(model);
                }

                int memberId = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));
                var member = _db.Members.FirstOrDefault(m => m.Id == memberId);

                if (member != null)
                {
                    model.mId = memberId;
                    model.comment = model.comment ?? "";

                    // Proceed to add the deposit as TId is unique
                    _db.Deposites.Add(model);
                    _db.SaveChanges();

                    TempData["SuccessMessage"] = "Deposit request submitted successfully!";
                    return RedirectToAction("Deposit", "Profile");
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid input. Please check your data and try again.";
            }

            var depositHistory = _db.Deposites
                .Where(d => d.mId == Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser")))
                .OrderByDescending(d => d.CreatedDateTime)
                .ToList();

            ViewBag.DepositHistory = depositHistory;

            return View(model);
        }


        public IActionResult Withdraw()
        {
            int memberId = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));
            var member = _db.Members.FirstOrDefault(m => m.Id == memberId);

            if (member == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var withdrawHistory = _db.Withdraws
                .Where(w => w.mId == memberId)
                .OrderByDescending(w => w.CreatedDateTime)
                .ToList();

            if (withdrawHistory==null)
            {
                ViewBag.WithdrawHistory = null;
            }
            else
            {
                ViewBag.WithdrawHistory = withdrawHistory;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Withdraw(Withdraw model)
        {
            if (model.WithdrawType!= null && model.Amount >= 1000)
            {
                int memberId = Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser"));
                var member = _db.Members.FirstOrDefault(m => m.Id == memberId);

                var hasSelectedWallet = _db.Wallets.Any(w => w.mId == memberId && w.WalletName == model.WithdrawType);

                if (!hasSelectedWallet)
                {
                    TempData["ErrorMessage"] = $"You need to add a {model.WithdrawType} wallet to your profile before requesting a withdrawal.";
                    return RedirectToAction("Withdraw");
                }

                var hasPendingWithdraw = _db.Withdraws.Any(w => w.mId == memberId && !w.IsStatus && !w.IsRejected);

                if (hasPendingWithdraw)
                {
                    TempData["ErrorMessage"] = "You already have a pending withdrawal request. Please wait until it is processed before submitting a new request.";
                    return RedirectToAction("Withdraw");
                }

                if (member != null && model.Amount >= 1000)
                {
                    if (member.WithdrawalBalance >= model.Amount)
                    {
                        model.mId = memberId;
                        model.Comment = model.Comment ?? "";

                        _db.Withdraws.Add(model);
                        _db.SaveChanges();

                        TempData["SuccessMessage"] = "Withdraw request submitted successfully!";
                        return RedirectToAction("Withdraw");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Insufficient balance.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid withdrawal amount.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid input. Please check your data and try again.";
            }

            var withdrawHistory = _db.Withdraws
                .Where(w => w.mId == Convert.ToInt32(HttpContext.Session.GetString("LoggedInUser")))
                .OrderByDescending(w => w.CreatedDateTime)
                .ToList();

            ViewBag.WithdrawHistory = withdrawHistory;

            return View(model);
        }
    }
}


