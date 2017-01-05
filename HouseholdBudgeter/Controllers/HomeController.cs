using HouseholdBudgeter.Helpers;
using HouseholdBudgeter.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HouseholdBudgeter.Controllers
{
    //[RequireHttps]
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
            var CurrentBudget = db.Budgets.First(a => a.HouseHoldId == model.Households.Id);

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


        [HttpGet]
        public ActionResult GetChart()
        {
            //var s = new [] { new { year= "2008", value= 20 },
            //    new { year= "2009", value= 5 },
            //    new { year= "2010", value= 7 },
            //    new { year= "2011", value= 10 },
            //    new { year= "2012", value= 20 }};

            var house = db.Households.Find(User.Identity.GetHouseholdId());
            var tod = DateTimeOffset.Now;
            decimal totalExpense = 0;
            decimal totalBudget = 0;
            var totalAcc = (from a in house.BankAccounts
                            select a.Balance).DefaultIfEmpty().Sum();

            //var totalAcc = house.Accounts.Select(a => a.Balance).DefaultIfEmpty().Sum();
            var bar = (from c in house.Categories
                       where c.CategoryType == "Expense"
                       let aSum = (from t in c.Transactions
                                   where t.Date.Year == tod.Year && t.Date.Month == tod.Month
                                   select t.Amount).DefaultIfEmpty().Sum()


                       let bSum = (from b in c.BudgetItems
                                   select b.Amount).DefaultIfEmpty().Sum()


                       let _ = totalExpense += aSum
                       let __ = totalBudget += bSum

                       select new
                       {
                           Name = c.Name,
                           Actual = aSum,
                           Budgeted = bSum
                       }).ToArray();

            var donut = (from c in house.Categories
                         where c.CategoryType == "Expense"
                         let aSum = (from t in c.Transactions
                                     where t.Date.Year == tod.Year && t.Date.Month == tod.Month
                                     select t.Amount).DefaultIfEmpty().Sum()
                         select new
                         {
                             label = c.Name,
                             value = aSum
                         }).ToArray();

            var result = new
            {
                totalAcc = totalAcc,
                totalBudget = totalBudget,
                totalExpense = totalExpense,
                bar = bar,
                donut = donut
            };

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
        public ActionResult GetMonthly()
        {
            var household = db.Households.Find(User.Identity.GetHouseholdId());
            var monthsToDate = Enumerable.Range(1, DateTime.Today.Month)
                            .Select(m => new DateTime(DateTime.Today.Year, m, 1))
                            .ToList();

            var sums = from month in monthsToDate
                       select new
                       {
                           month = month.ToString("MMM"),


                           income = (from account in household.BankAccounts
                                     from transaction in account.Transactions
                                     where transaction.Category.CategoryType == "Income" &&
                                           transaction.Date.Month == month.Month
                                     select transaction.Amount).DefaultIfEmpty().Sum(),
                           //income = household.Accounts.SelectMany(t => t.Transactions).Where(c => 
                           //c.Category.CategoryType.Name == "Income" && c.TransDate.Month ==
                           //month.Month).Select(t => t.Amount).DefaultIfEmpty().Sum(),


                           expense = (from account in household.BankAccounts
                                      from transaction in account.Transactions
                                      where transaction.Category.CategoryType == "Expense"
                                            && transaction.Date.Month == month.Month
                                      select transaction.Amount).DefaultIfEmpty().Sum(),
                           //expenses = household.Accounts.SelectMany(a => a.Transactions).Where(t => 
                           // t.Category.CategoryType.Name == "Expense" && t.TransDate.Month ==
                           // month.Month).Select(t => t.Amount).DefaultIfEmpty().Sum(),

                           budget = household.BudgetItems.Select(b => b.Amount).DefaultIfEmpty().Sum(),
                       };

            return Content(JsonConvert.SerializeObject(sums), "application/json");

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
    








