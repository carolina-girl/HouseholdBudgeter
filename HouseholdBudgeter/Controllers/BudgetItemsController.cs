using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HouseholdBudgeter.Models;
using Microsoft.AspNet.Identity;

namespace HouseholdBudgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class BudgetItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetItems
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var budgetItems = db.BudgetItems.Include(h => h.Budget.Household).Include(b => b.Budget).Include(b => b.Category);

            Household household = db.Households.Find(user.HouseholdId);


            if (household == null)
            {
                return RedirectToAction("Create", "Households");
            }

            ViewBag.HouseholdName = household.Name;

            return View(budgetItems);
        }

        // GET: BudgetItems/Details/5
        public ActionResult Details(int? id)
        {
            //heirarchy where we find the user
            var user = db.Users.Find(User.Identity.GetUserId());

            //then identify the user's budgetitems
            BudgetItem budgetItem = db.BudgetItems.FirstOrDefault(b => b.Id == id);

            //then the budgetitem's budget owner
            Budget budget = db.Budgets.FirstOrDefault(b => b.Id == budgetItem.Id);

            //then the budget's household owner
            Household household = db.Households.FirstOrDefault(h => h.Id == budget.Id);

            if (!household.Members.Contains(user))
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (budgetItem == null)
            {
                return HttpNotFound();
            }
            return View(budgetItem);
        }

        // GET: BudgetItems/Create
        public PartialViewResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            var getBudget = db.Budgets.Where(u => user.HouseholdId == u.HouseHoldId).ToList();

            ViewBag.BudgetId = new SelectList(getBudget, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.BudegetCategory, "Id", "Name");
            return PartialView();
        }

        // POST: BudgetItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BudgetId,CategoryId,Amount")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                db.BudgetItems.Add(budgetItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.BudegetCategory, "Id", "Name", budgetItem.CategoryId);
            return View(budgetItem);
        }

        // GET: BudgetItems/Edit/5
        public ActionResult Edit(int? id)
        {
            //heirarchy where we find the user
            var user = db.Users.Find(User.Identity.GetUserId());

            //then identify the user's budgetitems
            BudgetItem budgetItem = db.BudgetItems.FirstOrDefault(b => b.Id == id);

            //then the budget's household owner
            Household household = db.Households.FirstOrDefault(h => h.Id == budgetItem.Id);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (budgetItem == null)
            {
                return HttpNotFound();
            }

            var getBudget = db.Budgets.Where(u => u.HouseHoldId == user.HouseholdId).ToList();

            ViewBag.BudgetId = new SelectList(getBudget, "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.BudegetCategory, "Id", "Name", budgetItem.CategoryId);
            return View(budgetItem);
        }

        // POST: BudgetItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BudgetId,CategoryId,Amount")] BudgetItem budgetItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budgetItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BudgetId = new SelectList(db.Budgets, "Id", "Name", budgetItem.BudgetId);
            ViewBag.CategoryId = new SelectList(db.BudegetCategory, "Id", "Name", budgetItem.CategoryId);
            return View(budgetItem);
        }

        // GET: BudgetItems/Delete/5
        public ActionResult Delete(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            BudgetItem budgetItem = db.BudgetItems.FirstOrDefault(b => b.Id == id);
            Budget budget = db.Budgets.FirstOrDefault(b => b.Id == budgetItem.BudgetId);
            Household household = db.Households.FirstOrDefault(h => h.Id == budget.HouseHoldId);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
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
            //heirarchy where we find the user
            var user = db.Users.Find(User.Identity.GetUserId());

            //then identify the user's budgetitems
            BudgetItem budgetItem = db.BudgetItems.FirstOrDefault(b => b.Id == id);

            //then the budgetitem's budget owner
            Budget budget = db.Budgets.FirstOrDefault(b => b.Id == budgetItem.Id);

            //then the budget's household owner
            Household household = db.Households.FirstOrDefault(h => h.Id == budget.Id);

            //if (!household.Members.Contains(user))
            //{
            //    return RedirectToAction("Unauthorized", "Error");
            //}

            db.BudgetItems.Remove(budgetItem);
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
