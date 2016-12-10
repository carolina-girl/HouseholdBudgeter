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
    public class BudgetCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetCategories
        public ActionResult Index()
        {
            return View(db.BudgetCategory.ToList());
        }

        // GET: BudgetCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetCategory budgetCategory = db.BudgetCategory.Find(id);
            if (budgetCategory == null)
            {
                return HttpNotFound();
            }
            return View(budgetCategory);
        }

        // GET: BudgetCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BudgetCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Category")] BudgetCategory budgetCategory)
        {
            if (ModelState.IsValid)
            {
                db.BudgetCategory.Add(budgetCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(budgetCategory);
        }

        // GET: BudgetCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetCategory budgetCategory = db.BudgetCategory.Find(id);
            if (budgetCategory == null)
            {
                return HttpNotFound();
            }
            return View(budgetCategory);
        }

        // POST: BudgetCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Category")] BudgetCategory budgetCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budgetCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(budgetCategory);
        }

        // GET: BudgetCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BudgetCategory budgetCategory = db.BudgetCategory.Find(id);
            if (budgetCategory == null)
            {
                return HttpNotFound();
            }
            return View(budgetCategory);
        }

        // POST: BudgetCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id)
        {
            var category = db.BudgetCategory.Find(Id);
            var item = db.BudgetItem.Find(Id);
            UpdateBudgetAmount(false, item.Amount, item.Frequency, item.BudgetId);
            BudgetCategory budgetCategory = db.BudgetCategory.Find(Id);
            db.BudgetCategory.Remove(budgetCategory);
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
