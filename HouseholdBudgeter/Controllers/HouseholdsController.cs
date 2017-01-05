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
    //[Authorize]
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            Household household = db.Households.Find(user.HouseholdId);
            if (household == null)
            {
                return RedirectToAction("Create", "Households");
            }

            return View(db.Households.Find(user.HouseholdId));
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
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
        public ActionResult Create([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                //previously users could create a matching name and be added to existing household
                //now, new house is created
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user.HouseholdId == null)
                {
                    Household household2 = household;
                    db.Households.Add(household2 = new Household
                    {
                        Name = household.Name
                    });
                    db.SaveChanges();

                    user.HouseholdId = household2.Id;

                    household2.BankAccounts.Add(new BankAccount
                    {
                        Name = "Checkings",
                        Created = new DateTimeOffset(DateTime.Now),
                        Balance = 0,
                        InitialBalance = 0,
                        ReconcileBalance = 0,
                        WarningBalance = 0
                    });
                    household2.BankAccounts.Add(new BankAccount
                    {
                        Name = "Savings",
                        Created = new DateTimeOffset(DateTime.Now),
                        Balance = 0,
                        InitialBalance = 0,
                        ReconcileBalance = 0,
                        WarningBalance = 0
                    });

                    db.SaveChanges();

                    return RedirectToAction("Index", new { id = household2.Id });
                }
                else
                {
                    return RedirectToAction("Index", new { id = user.Id });
                }
            }
            return View(household);
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
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
        public ActionResult Edit([Bind(Include = "Id,Name")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        public ActionResult Leave()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            Household household = db.Households.Find(user.HouseholdId);

            //set user's house to null
            //remove the user from the household list
            user.HouseholdId = null;
            household.Members.Remove(user);

            db.SaveChanges();
            return RedirectToAction("Create", "Households");
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            BankAccount bankaccount = db.BankAccounts.FirstOrDefault(x => x.Id == id);

            Household household = db.Households.Find(id);

            if (!household.Members.Contains(user))
            {
                return RedirectToAction("Unauthorized", "Error");
            }

            db.Households.Remove(household);
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

