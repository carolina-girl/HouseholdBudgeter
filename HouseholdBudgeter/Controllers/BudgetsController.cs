using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HouseholdBudgeter.Models;
using HouseholdBudgeter.Helpers;
using static HouseholdBudgeter.Helpers.AuthorizeHousehold;

namespace HouseholdBudgeter.Controllers
{ 

    [RequireHttps]
[AuthorizeHouseholdRequired]
public class BudgetsController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budgets
        public ActionResult Index()
        {
            var householdId = User.Identity.GetHouseholdId();
            var household = db.Household.Find(householdId);
            var model = household.Budget;
            var budget = db.Budget.Include(b => b.Household);
            return View(budget.ToList());
        }

        //POST: Add Budget Item
        [HttpPost]
        public ActionResult AddBudgetItem(int Frequency, decimal Amount, string CategoryName)
        {
            if (ModelState.IsValid)
            {
                var householdId = User.Identity.GetHouseholdId();
                var budget = db.Budget.FirstOrDefault(b => b.HouseholdId == householdId);
                var item = new BudgetItem();
                item.Date = DateTimeOffset.Now;
                item.Frequency = Frequency;
                item.Amount = Amount;
                db.BudgetItem.Add(item);
                budget.BudgetItems.Add(item);

                BudgetCategory category = new BudgetCategory();
                var budgetCategories = from i in budget.BudgetItems
                                       from c in db.BudgetCategory
                                       where i.BudgetCategoryId == c.Id
                                       select c;
                if (budgetCategories.Any(b => b.Name.Standardize() == CategoryName.Standardize()))
                {
                    TempData["Error"] = "Category already exists. Please enter a different category name.";
                    return RedirectToAction("Index");
                }
                else
                {
                    category.Name = CategoryName;
                }
                //category.BudgetItems = BudgetItem;
                db.BudgetCategory.Add(category);
                db.SaveChanges();
                UpdateBudgetAmount(true, Amount, Frequency, budget.Id);
                item.BudgetCategoryId = category.Id;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }

            //Helper function: Update account balance
            public bool UpdateBudgetAmount(bool AddAmount, decimal Amount, int Frequency, int? BudgetId)
            {
                var budget = db.Budget.Find(BudgetId);
                budget.Amount = (AddAmount) ? budget.Amount + Amount * Frequency / 12 : budget.Amount - Amount * Frequency / 12;
                budget.Household = budget.Household;
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChangesWithErrors();

                return true;
            }

            // GET: Budgets/Details/5
            public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Budget budget = db.Budget.Find(id);
                if (budget == null)
                {
                    return HttpNotFound();
                }
                return View(budget);
            }

            // GET: Budgets/Create
            public ActionResult Create()
            {
                ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name");
                return View();
            }

            // POST: Budgets/Create
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create([Bind(Include = "Id,HouseholdId,Name")] Budget budget)
            {
                if (ModelState.IsValid)
                {
                    db.Budget.Add(budget);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name", budget.HouseholdId);
                return View(budget);
            }

            // GET: Budgets/Edit/5
            public ActionResult Edit(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Budget budget = db.Budget.Find(id);
                if (budget == null)
                {
                    return HttpNotFound();
                }
                ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name", budget.HouseholdId);
                return View(budget);
            }

            // POST: Budgets/Edit/5
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit([Bind(Include = "Id,HouseholdId,Name")] Budget budget)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(budget).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name", budget.HouseholdId);
                return View(budget);
            }

            // GET: Budgets/Delete/5
            public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Budget budget = db.Budget.Find(id);
                if (budget == null)
                {
                    return HttpNotFound();
                }
                return View(budget);
            }

            // POST: Budgets/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                Budget budget = db.Budget.Find(id);
                db.Budget.Remove(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
        }
    }
