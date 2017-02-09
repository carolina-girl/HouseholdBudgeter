using HouseholdBudgeter.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HouseholdBudgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class HomeController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }

        public ActionResult Dashboard(int? id, int? low)
        {
            //object of DashboardViewModel
            DashboardViewModel model = new DashboardViewModel();

            //get user Id
            var user = db.Users.Find(User.Identity.GetUserId());

            //get users of Household
            model.Households = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Households;

            if (model.Households == null)
            {
                return RedirectToAction("Create", "Households");
            }

            //get in descending order transactions
            model.Transactions = db.Transactions.Where(a => a.BankAccounts.HouseholdId == user.HouseholdId).OrderByDescending(a => a.Id).Take(6).ToList();
            //get all account for this household
            model.BankAccounts = db.BankAccounts.Where(a => a.HouseholdId == model.Households.Id).ToList();

            //get List of existing budgets in this household
            var getBudget = db.Budgets.Where(a => user.HouseholdId == a.HouseHoldId).ToList();

            //if (model.Households.Budgets.Count == 0)
            //{
            //    return RedirectToAction("Index", "Budgets");
            //}

            //Get currentbudget for this
            var CurrentBudget = db.Budgets.FirstOrDefault(a => a.HouseHoldId == model.Households.Id);

            if (id == null)
            {
                //Viewbag to get list of current existing budget
                ViewBag.BudgetId = new SelectList(getBudget, "Id", "Name", CurrentBudget.Id);
                //assign current budget id to existing var to hold the value
                model.GetBudgetId = CurrentBudget.Id;
                //assign the current budget to show on chart
                model.Budgets = CurrentBudget;
            }
            else
            {
                //dispaly current existing budget in household
                ViewBag.BudgetId = new SelectList(getBudget, "Id", "Name", id);
                //assign id to GetBudgetId
                model.GetBudgetId = (int)id;
                //get budget assign to id
                model.Budgets = db.Budgets.First(a => a.Id == id);

            }

            var currentDate = DateTime.Now;

            model.begin = new DateTime(currentDate.Year, currentDate.Month, 1);
            model.end = currentDate;

            return View(model);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateChart(int BudgetId)
        {
            //retrieve data back to dashboard view
            return RedirectToAction("Dashboard", new { id = BudgetId });
        }
    }
}
    








