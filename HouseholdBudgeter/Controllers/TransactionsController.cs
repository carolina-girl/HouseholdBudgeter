using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HouseholdBudgeter.Models;
using Microsoft.AspNet.Identity;
using HouseholdBudgeter.Helpers;
using static HouseholdBudgeter.Helpers.HouseholdHelper;

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
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "FirstName", transaction.UserId);
            return View(transaction);
        }

        // GET: Add Transaction
        public ActionResult _AddTransaction(int? id)
        {
            try
            {
                ViewBag.AccountId = id;
                var householdId = User.Identity.GetHouseholdId();
                var household = db.Household.Find(householdId);
                //var categories = household.Budgets.BudgetItems.Select(b => b.BudgetCategory).Distinct().ToList();
                //ViewBag.BudgetCategories = categories;
                return PartialView();
            }
            catch
            {
                return PartialView("_Error");
            }
        }

        // GET: Add Transaction
        public ActionResult _ReconcileAccount(int? id)
        {
            try
            {
                ViewBag.AccountId = id;
                return PartialView();
            }
            catch
            {
                return PartialView("_Error");
            }
        }


        public bool SetIsReconciled(bool IsReconciled, int? AccountId)
        {
            var account = db.BankAccount.Find(AccountId);
            if (IsReconciled)
            {
                foreach (var transaction in account.Transactions)
                {
                    transaction.IsReconciled = IsReconciled;
                    db.Transactions.Attach(transaction);
                    db.Entry(transaction).Property("IsReconciled").IsModified = true;
                }
            }
            account.ReconcileAmount = account.ReconcileAmount;
            db.BankAccount.Attach(account);
            db.Entry(account).Property("IsReconciled").IsModified = true;
            db.SaveChanges();

            return true;
        }

        //POST: Transaction/AddTransaction
        [HttpPost]
        public ActionResult AddTransaction([Bind(Include = "Amount, Description, TransactionType")] Transaction transaction, int? AccountId, int? BudgetCategoryId)
        {
            if (ModelState.IsValid)
            {
                transaction.Date = DateTimeOffset.Now;
                var userId = User.Identity.GetUserId();
                transaction.UserId = userId;
                if (transaction.TransactionType == TransactionType.Expense)
                {
                    transaction.CategoryId = transaction.CategoryId;
                    UpdateAccountBalance(false, false, transaction.Amount, AccountId);
                }
                else
                {
                    UpdateAccountBalance(true, false, transaction.Amount, AccountId);
                }
                db.Transactions.Add(transaction);
                db.SaveChanges();

                var theTransaction = db.Transactions.Find(transaction.Id);
                var account = db.BankAccount.Find(AccountId);
                account.Transactions.Add(theTransaction);
                db.SaveChanges();

                SetIsReconciled(false, AccountId);
            }
            return RedirectToAction("Details", new { id = transaction.AccountId });
        }


        //POST: Transaction/ReconcileAccount
        [HttpPost]
        public ActionResult ReconcileAccount(decimal ActualBalance, int AccountId)
        {
            if (ModelState.IsValid)
            {
                var account = db.BankAccount.Find(AccountId);
                if (ActualBalance > account.Balance)
                {
                    var balanceDifference = ActualBalance - account.Balance;
                    UpdateAccountBalance(true, true, balanceDifference, AccountId);
                }
                else
                {
                    var balanceDifference = account.Balance - ActualBalance;
                    UpdateAccountBalance(false, true, balanceDifference, AccountId);
                }
                SetIsReconciled(true, AccountId);
            }
            return RedirectToAction("Details", new { id = AccountId });
        }

        public ActionResult _EditTransaction(int? id)
        {
            try
            {
                var householdId = User.Identity.GetHouseholdId();
                var household = db.Household.Find(householdId);
                //var categories = household.Budgets.BudgetItem.Select(b => b.BudgetCategory).Distinct().ToList();
                //ViewBag.BudgetCategories = categories;
                var transaction = db.Transactions.Find(id);
                return PartialView(transaction);
            }
            catch
            {
                return PartialView("_Error");
            }
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
        public ActionResult Edit([Bind(Include = "Id,CategoryId,AccountId,Date,ReconcileDate,Description,Name,UserId,Amount,ReconcileAmount,IsExpense,IsReconciled")] Transaction transaction, int? CategoryId)
        { 
            if (ModelState.IsValid)
            {
                transaction.Date = DateTimeOffset.Now;
                var userId = User.Identity.GetUserId();
                transaction.UserId = userId;
                var originalTransaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == transaction.Id);
                decimal AccountAmount = transaction.Amount - originalTransaction.Amount;
                if (transaction.TransactionType == TransactionType.Expense)
                {
                    transaction.CategoryId = transaction.CategoryId;
                    UpdateAccountBalance(false, false, AccountAmount, transaction.AccountId);
                }
                else
                {
                    //transaction.CategoryId = null;
                    UpdateAccountBalance(true, false, AccountAmount, transaction.AccountId);
                  }
                db.Transactions.Attach(transaction);
                db.Entry(transaction).Property("Amount").IsModified = true;
                db.Entry(transaction).Property("Description").IsModified = true;
                db.Entry(transaction).Property("BudgetCategoryId").IsModified = true;
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = transaction.AccountId });
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
            //bool AddBalance;
            //AddBalance = (transaction.TransactionType == Type.Expense) ? true : false;
            //UpdateAccountBalance(AddBalance, false, transaction.Amount, transaction.AccountId);
            transaction = db.Transactions.Find(Id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = AccountId });
        }

        //[HttpPost, ActionName("Void")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Void(int Id, int AccountId)
        //{


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
