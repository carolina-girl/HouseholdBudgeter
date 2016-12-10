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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var householdId = User.Identity.GetHouseholdId();
            var model = db.BankAccount.Where(a => a.HouseholdId == householdId).ToList();
            var transactions = db.Transactions.Include(t => t.User);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        //Helper function: Update account balance
        public bool UpdateAccountBalance(bool IsIncome, bool IsReconciled, decimal Amount, int? AccountId)
        {
            var account = db.BankAccount.Find(AccountId);
            account.Balance = (IsIncome) ? account.Balance + Amount : account.Balance - Amount;
            if (IsReconciled)
            {
                account.ReconcileAmount = (IsIncome) ? account.ReconcileAmount + Amount : account.ReconcileAmount - Amount;
            }
            else
            {
                account.ReconcileAmount = account.ReconcileAmount;
            }
            db.BankAccount.Attach(account);
            db.Entry(account).Property("Balance").IsModified = true;
            db.Entry(account).Property("ReconciledAmount").IsModified = true;
            db.SaveChanges();

            return true;
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            //ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "FirstName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CategoryId,AccountId,Date,ReconcileDate,Description,Name,UserId,Amount,ReconcileAmount,IsExpense,IsReconciled")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                ViewBag.AccountId = Id;
                var householdId = User.Identity.GetHouseholdId();
                var household = db.Household.Find(householdId);
                var categories = household.Budget.BudgetItems.Select(b => b.BudgetCategory).Distinct().ToList();
                ViewBag.BudgetCategories = categories;
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "FirstName", transaction.UserId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            //ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "FirstName", transaction.UserId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryId,AccountId,Date,ReconcileDate,Description,Name,UserId,Amount,ReconcileAmount,IsExpense,IsReconciled")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.Date = DateTimeOffset.Now;
                var userId = User.Identity.GetUserId();
                transaction.UserId = userId;
                var originalTransaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == transaction.Id);
                decimal AccountAmount = transaction.Amount - originalTransaction.Amount;
                if (transaction.TransactionType == Type.Expense)
                {
                    transaction.BudgetCategoryId = BudgetCategoryId;
                    UpdateAccountBalance(false, false, AccountAmount, transaction.AccountId);
                }
                else
                {
                    transaction.BudgetCategoryId = null;
                    UpdateAccountBalance(true, false, AccountAmount, transaction.AccountId);
                }
                db.Transactions.Attach(transaction);
                db.Entry(transaction).Property("Amount").IsModified = true;
                db.Entry(transaction).Property("Description").IsModified = true;
                db.Entry(transaction).Property("BudgetCategoryId").IsModified = true;
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new {id = Transaction.AccountId);
            }
            //ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "FirstName", transaction.UserId);
            return View(transaction);
        }
        
        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id, int AccountId)
        {
            var transaction = db.Transactions.Find(Id);
            bool AddBalance;
            AddBalance = (transaction.TransactionType == Type.Expense) ? true : false;
            UpdateAccountBalance(AddBalance, false, transaction.Amount, transaction.AccountId);
            transaction = db.Transactions.Find(Id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = AccountId });
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
