using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HouseholdBudgeter.Models;

namespace HouseholdBudgeter.Controllers
{
    public class BudgetItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetItems
        public ActionResult Index()
        {
            var budgetItem = db.BudgetItem.Include(b => b.Budget).Include(b => b.BudgetCategory);
            return View(budgetItem.ToList());
        }

        // GET: BudgetItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItem.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // GET: BudgetItems/Create
        public ActionResult Create()
        {
            ViewBag.BudgetId = new SelectList(db.Budget, "Id", "Name");
            ViewBag.BudgetCategoryId = new SelectList(db.BudgetCategory, "Id", "Category");
            return View();
        }

        // POST: BudgetItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BudgetCategoryId,BudgetId,Frequency,Date,Amount,Name")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                db.BudgetItem.Add(budgetItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BudgetId = new SelectList(db.Budget, "Id", "Name", budgetItem.BudgetId);
            ViewBag.BudgetCategoryId = new SelectList(db.BudgetCategory, "Id", "Category", budgetItem.BudgetCategoryId);
            return View(budgetItem);
        }

        //POST: Add Budget Item
        [HttpPost]
        public ActionResult AddBudgetItem(int Frequency, decimal Amount, string CategoryName)
        {
            if (ModelState.IsValid)
            {
                var householdId = User.Identity.GetHouseholdId();
                var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == householdId);
                var item = new BudgetItem();
                item.Created = DateTimeOffset.Now;
                item.Frequency = Frequency;
                item.Amount = Amount;
                db.BudgetItems.Add(item);
                budget.BudgetItems.Add(item);

                BudgetCategory category = new BudgetCategory();
                var budgetCategories = from i in budget.BudgetItems
                                       from c in db.BudgetCategories
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
                category.BudgetItem = item;
                db.BudgetCategories.Add(category);
                db.SaveChanges();
                UpdateBudgetAmount(true, Amount, Frequency, budget.Id);
                item.BudgetCategoryId = category.Id;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }


        // GET: BudgetItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItem.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.BudgetId = new SelectList(db.Budget, "Id", "Name", budgetItem.BudgetId);
            ViewBag.BudgetCategoryId = new SelectList(db.BudgetCategory, "Id", "Category", budgetItem.BudgetCategoryId);
            return View(budgetItem);
        }

        // POST: BudgetItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BudgetCategoryId,BudgetId,Frequency,Date,Amount,Name")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                var householdId = User.Identity.GetHouseholdId();
                var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == householdId);
                var oldItem = db.BudgetItems.AsNoTracking().FirstOrDefault(m => m.Id == item.Id);
                item.Modified = DateTimeOffset.Now;

                budget.Amount -= oldItem.Amount * oldItem.Frequency / 12;
                budget.Amount += item.Amount * item.Frequency / 12;
                budget.Household = budget.Household;

                db.BudgetItems.Attach(item);
                db.Entry(item).Property("Amount").IsModified = true;
                db.Entry(item).Property("Frequency").IsModified = true;
                db.Entry(item).Property("Modified").IsModified = true;
                db.Budgets.Attach(budget);
                db.Entry(budget).Property("Amount").IsModified = true;
                db.SaveChanges();

                var category = db.BudgetCategories.Find(oldItem.BudgetCategory.Id);
                var oldCategoryName = oldItem.BudgetCategory.Name;
                if (oldCategoryName != CategoryName)
                {
                    var budgetCategories = from i in budget.BudgetItems
                                           from c in db.BudgetCategories
                                           where i.BudgetCategoryId == c.Id
                                           select c;
                    if (budgetCategories.Any(b => b.Name.Standardize() == CategoryName.Standardize()))
                    {
                        TempData["EditError"] = "That category already exists. Please enter a different category name.";
                        TempData["Id"] = item.Id;
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    category.Name = CategoryName;
                }
                category.Name = CategoryName;
                db.BudgetCategories.Attach(category);
                db.Entry(category).Property("Name").IsModified = true;
                db.Entry(budgetItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BudgetId = new SelectList(db.Budget, "Id", "Name", budgetItem.BudgetId);
            ViewBag.BudgetCategoryId = new SelectList(db.BudgetCategory, "Id", "Category", budgetItem.BudgetCategoryId);
            return View(budgetItem);
        }

        // GET: BudgetItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetItem budgetItem = db.BudgetItem.Find(id);
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // POST: BudgetItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BudgetItem budgetItem = db.BudgetItem.Find(id);
            db.BudgetItem.Remove(budgetItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //POST: Edit Budget Amount
        [HttpPost]
        public ActionResult EditBudgetAmount()
        {
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
