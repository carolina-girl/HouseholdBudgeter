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
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Budgets
        public ActionResult Index(int? id)
        {
            //find user in the household
            var UserId = User.Identity.GetUserId();
            var user = db.Users.Find(UserId);

            Household household = db.Households.Find(user.HouseholdId);


            var budgets = db.Budgets.Where(u => u.HouseHoldId == user.HouseholdId).Include(b => b.Household);

            //if user does not have household, create one
            if (household == null)
            {
                return RedirectToAction("Create", "households");
            }
            return View(budgets);
        }

        // GET: Budgets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // GET: Budgets/Create
        public PartialViewResult _CreateBudget()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            var getHouse = db.Households.Where(u => user.HouseholdId == u.Id).ToList();

            ViewBag.HouseHoldId = new SelectList(getHouse, "Id", "Name");
            return PartialView();
        }

        public PartialViewResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            var getHouse = db.Households.Where(u => user.HouseholdId == u.Id).ToList();

            ViewBag.HouseHoldId = new SelectList(getHouse, "Id", "Name");
            return PartialView();
        }
        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,HouseHoldId")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.Budgets.Add(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HouseHoldId = new SelectList(db.Households, "Id", "Name", budget.HouseHoldId);
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public ActionResult Edit(int? id)
        {
            //heirarchy where we find the user
            var user = db.Users.Find(User.Identity.GetUserId());

            //then the user's budget
            Budget budget = db.Budgets.FirstOrDefault(b => b.Id == id);

            BudgetItem budgetItem = db.BudgetItems.FirstOrDefault(b => b.Id == id);

            //then the budget's household owner
            Household household = db.Households.FirstOrDefault(h => h.Id == budget.Id);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (budget == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseHoldId = new SelectList(db.Households, "Id", "Name", budget.HouseHoldId);
            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,HouseHoldId")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                budget.HouseHoldId = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).HouseholdId.Value;

                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseHoldId = new SelectList(db.Households, "Id", "Name", budget.HouseHoldId);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public ActionResult Delete(int? id)
        {
            //heirarchy where we find the user
            var user = db.Users.Find(User.Identity.GetUserId());

            //then the user's budget
            Budget budget = db.Budgets.FirstOrDefault(b => b.Id == id);

            //then the budget's household owner
            Household household = db.Households.FirstOrDefault(h => h.Id == budget.Id);

            //if (!household.Members.Contains(user))
            //{
            //    return RedirectToAction("Unauthorized", "Error");
            //}
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //heirarchy where we find the user
            var user = db.Users.Find(User.Identity.GetUserId());

            //then the user's budget
            Budget budget = db.Budgets.FirstOrDefault(b => b.Id == id);

            //then the budget's household owner
            Household household = db.Households.FirstOrDefault(h => h.Id == budget.Id);

            db.Budgets.Remove(budget);
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
