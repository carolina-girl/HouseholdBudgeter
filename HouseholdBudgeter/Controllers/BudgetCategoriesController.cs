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
    public class BudgetCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BudgetCategories
        public ActionResult Index(int? id)
        {
            //get user, household of this user
            var user = db.Users.Find(User.Identity.GetUserId());

            Household household = db.Households.Find(user.HouseholdId);

            if (household == null)
            {
                return RedirectToAction("Create", "Households");
            }

            return View(db.BudgetCategory);
        }

        // GET: BudgetCategories/Details/5
        public ActionResult Details(int? id)
        {
            //get user, category, household
            var user = db.Users.Find(User.Identity.GetUserId());

            BudgetCategory category = db.BudgetCategory.FirstOrDefault(b => b.Id == id);

            Household household = db.Households.FirstOrDefault(h => h.Id == category.Id);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: BudgetCategories/Create
        public PartialViewResult Create()
        {
            return PartialView();
        }

        // POST: BudgetCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] BudgetCategory category)
        {
            if (ModelState.IsValid)
            {
                db.BudgetCategory.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: BudgetCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            //get use, category, household owner of category
            var user = db.Users.Find(User.Identity.GetUserId());

            BudgetCategory category = db.BudgetCategory.FirstOrDefault(b => b.Id == id);

            Household household = db.Households.FirstOrDefault(h => h.Id == category.Id);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: BudgetCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] BudgetCategory category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: BudgetCategories/Delete/5$
        public ActionResult Delete(int? id)
        {
            //get user, category, household owner
            var user = db.Users.Find(User.Identity.GetUserId());

            BudgetCategory category = db.BudgetCategory.FirstOrDefault(b => b.Id == id);

            Household household = db.Households.FirstOrDefault(h => h.Id == category.Id);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: BudgetCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //get user, category, household owner
            var user = db.Users.Find(User.Identity.GetUserId());

            BudgetCategory category = db.BudgetCategory.FirstOrDefault(b => b.Id == id);

            Household household = db.Households.FirstOrDefault(h => h.Id == category.Id);

            db.BudgetCategory.Remove(category);
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
