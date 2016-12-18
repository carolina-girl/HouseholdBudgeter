using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HouseholdBudgeter.Models;
using System.Threading.Tasks;
using HouseholdBudgeter.Helpers;
using static HouseholdBudgeter.Helpers.AuthorizeHousehold;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Policy;

namespace HouseholdBudgeter.Controllers
{
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Households
        public ActionResult Index()
        {

            return View(db.Household.ToList());
        }

        // GET: Households/Details/5
        public ActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Household.Find(Id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Users,BudgetId,FirstName,LastName,EmailAddress,Date")] Household household)
        {
            if (ModelState.IsValid)
            {
                //Create household
                household.Date = System.DateTimeOffset.Now;
                db.Household.Add(household);
                db.SaveChanges();
                //Add user to household
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var thisHousehold = db.Household.Find(household.Id);
                thisHousehold.Users.Add(user);
                ////Create budget for household
                //Budget budget = new Budget();
                //budget.Created = System.DateTimeOffset.Now;
                //budget.Amount = 0;
                //budget.HouseholdId = thisHousehold.Id;
                //budget.Household = thisHousehold;
                //db.Budgets.Add(budget);
                ////Update household to include budget
                //thisHousehold.Budget = budget;
                //thisHousehold.BudgetId = budget.Id;
                //db.Households.Attach(thisHousehold);
                //db.Entry(thisHousehold).Property("BudgetId").IsModified = true;
                //db.SaveChanges();
                ////Refresh cookies to add new household Id
                //await ControllerContext.HttpContext.RefreshAuthentication(user);
                return RedirectToAction("Index");
            }

            return View();
        }


        // GET: Households/Edit/5
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Household.Find(Id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Users,BudgetId,Name,Date,FirstName,LastName,EmailAddress")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }



        // POST: Households/LeaveHousehold/5
        [HttpPost]
        public async Task<ActionResult> LeaveHousehold(bool? confirmLeaveHousehold)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var householdId = User.Identity.GetHouseholdId();
            var household = db.Household.Find(householdId);

            if (confirmLeaveHousehold != null && household.Users.Contains(user))
            {
                household.Users.Remove(user);
                db.SaveChanges();
                //await ControllerContext.HttpContext.RefreshAuthentication(user);
                return RedirectToAction("Dashboard", "Home");
            }

            TempData["Error"] = "Please confirm you want to leave this household.";
            return RedirectToAction("Dashboard", "Home");
        }
    




// GET: Households/Delete/5
public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Household.Find(Id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id)
        {
            Household household = db.Household.Find(Id);
            db.Household.Remove(household);
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

