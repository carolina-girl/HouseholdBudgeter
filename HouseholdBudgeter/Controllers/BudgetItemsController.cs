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

        // GET: Add BudgetItems
        public ActionResult _AddBudgetItems(int? Id)
        {
            try
            {
                return PartialView();
            }
            catch
            {
                return PartialView("_Error");
            }
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




        // GET: Edit BudgetItem
        public ActionResult _EditBudgetItem(int? id)
        {
            try
            {
                var model = db.BudgetItem.Find(id);
                return PartialView(model);
            }
            catch
            {
                return PartialView("_Error");
            }
        }

        //POST: Edit Budget Item
        [HttpPost]
        public ActionResult EditBudgetItem([Bind(Include = "Id, BudgetId, Created, Amount, Frequency, BudgetCategory.Id")] BudgetItem item, string CategoryName)
        {
            if (ModelState.IsValid)
            {
                var householdId = User.Identity.GetHouseholdId();
                var budget = db.Budget.FirstOrDefault(b => b.HouseholdId == householdId);
                var oldItem = db.BudgetItem.AsNoTracking().FirstOrDefault(m => m.Id == item.Id);
                item.Date = DateTimeOffset.Now;

                budget.Amount -= oldItem.Amount * oldItem.Frequency / 12;
                budget.Amount += item.Amount * item.Frequency / 12;
                budget.Household = budget.Household;

                db.BudgetItem.Attach(item);
                db.Entry(item).Property("Amount").IsModified = true;
                db.Entry(item).Property("Frequency").IsModified = true;
                db.Entry(item).Property("Modified").IsModified = true;
                db.Budget.Attach(budget);
                db.Entry(budget).Property("Amount").IsModified = true;
                db.SaveChanges();

                var category = db.BudgetCategory.Find(oldItem.BudgetCategory.Id);
                var oldCategoryName = oldItem.BudgetCategory.Name;
                if (oldCategoryName != CategoryName)
                {
                    var budgetCategories = from i in budget.BudgetItems
                                           from c in db.BudgetCategory
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
                db.BudgetCategory.Attach(category);
                db.Entry(category).Property("Name").IsModified = true;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Edit BudgetItem
        public ActionResult _DeleteBudgetItem(int? Id)
        {
            try
            {
                var model = db.BudgetItem.Find(Id);
                return PartialView(model);
            }
            catch
            {
                return PartialView("_Error");
            }
        }

        //POST: Transaction/DeleteTransaction
        [HttpPost]
        public ActionResult DeleteBudgetItem(int Id)
        {
            var category = db.BudgetCategory.Find(Id);
            var item = db.BudgetItem.Find(Id);
            //UpdateBudgetAmount(false, item.Amount, item.Frequency, item.BudgetId);
            db.BudgetCategory.Remove(category);
            db.BudgetItem.Remove(item);
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
