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
                var budget = db.Budget.FirstOrDefault(b => b.HouseholdId == householdId);
                var item = new BudgetItem();
                item.Date = DateTimeOffset.Now;
                item.Frequency = Frequency;
                item.Amount = Amount;
                db.BudgetItem.Add(item);
                budget.BudgetItems.Add(item);

                //BudgetCategory category = new BudgetCategory();
                //var budgetCategories = from i in budget.BudgetItems
                //                       from c in db.BudgetCategory
                //                       where i.BudgetCategoryId == c.Id
                //                       select c;
                //if (budgetCategories.Any(b => b.Category.Standardize() == CategoryName.Standardize()))
                //{
                //    TempData["Error"] = "Category already exists. Please enter a different category name.";
                //    return RedirectToAction("Index");
                //}
                //else
                //{
                //    category.Category = CategoryName;
                //}
                //category.BudgetItems = item;
                //db.BudgetCategory.Add(category);
                //db.SaveChanges();
                //UpdateBudgetAmount(true, Amount, Frequency, budget.Id);
                //item.BudgetCategoryId = category.Id;
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
                var budget = db.Budget.FirstOrDefault(b => b.HouseholdId == householdId);
                var oldItem = db.BudgetItem.AsNoTracking().FirstOrDefault(m => m.Id == budgetItem.Id);
                budgetItem.Date = DateTimeOffset.Now;

                budgetItem.Amount -= oldItem.Amount * oldItem.Frequency / 12;
                budgetItem.Amount += budgetItem.Amount * budgetItem.Frequency / 12;
                budget.Household = budget.Household;
          

                db.BudgetItem.Attach(budgetItem);
                db.Entry(budgetItem).Property("Amount").IsModified = true;
                db.Entry(budgetItem).Property("Frequency").IsModified = true;
                db.Entry(budgetItem).Property("Date").IsModified = true;
                db.Entry(budget).Property("Amount").IsModified = true;
                db.Budget.Attach(budget);
                db.Entry(budgetItem).State = EntityState.Modified;
                db.SaveChanges();

                //var newCategory = db.BudgetCategory.Find(oldItem.BudgetCategoryId);
                //var oldCategoryName = oldItem.Catagory.Category;
                //if (oldCategoryName != newCategory)
                //{
                //    var budgetCategories = from i in budget.BudgetItems
                //                           from c in db.BudgetCategory
                //                           where i.BudgetCategoryId == c.Id
                //                           select c;
                //    if (budgetCategories.Any(b => b.Category.Standardize() == newCategory.Standardize()))
                //    {
                //        TempData["EditError"] = "That category already exists. Please enter a different category name.";
                //        TempData["Id"] = budgetItem.Id;
                //        return RedirectToAction("Index");
                //    }
                //}
                //else
                //{
                //    newCategory.Category = Category;
                //}
                //newCategory.Category = Category;
                //db.BudgetCategory.Attach(newCategory);
                //db.Entry(newCategory).Property("Name").IsModified = true;
                //db.Entry(budgetItem).State = EntityState.Modified;
                //db.SaveChanges();
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
        public ActionResult DeleteConfirmed(int Id)
        {
     
            BudgetItem budgetItem = db.BudgetItem.Find(Id);
            db.BudgetItem.Remove(budgetItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //POST: Edit Budget Amount
        [HttpPost]
        public ActionResult EditBudgetAmount(int Id)
        {
            var category = db.BudgetCategory.Find(Id);
            var item = db.BudgetItem.Find(Id);
            //EditBudgetAmount(false, item.Amount, item.Frequency, item.BudgetId);
            BudgetCategory budgetCategory = db.BudgetCategory.Find(Id);
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
