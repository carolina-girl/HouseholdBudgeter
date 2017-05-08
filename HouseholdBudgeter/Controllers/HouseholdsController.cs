using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HouseholdBudgeter.Models;
using Microsoft.AspNet.Identity;
using static HouseholdBudgeter.Helpers.AuthorizeHousehold;
using HouseholdBudgeter.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HouseholdBudgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            //get user and household by id
            var user = db.Users.Find(User.Identity.GetUserId());

            Household household = db.Households.Find(user.HouseholdId);

            if (household == null)
            {
                return RedirectToAction("Create", "Households");
            }
            //list this users households
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
                // get user and create new household if this user doesnt have one, save changes
                var user = db.Users.Find(User.Identity.GetUserId());

                if (user.HouseholdId == null)
                {
                    Household household2 = household;
                    db.Households.Add(household2 = new Household{ Name = household.Name });
                    db.SaveChanges();
                    user.HouseholdId = household2.Id;
                    //go ahead and give new household two accounts
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

        
        [AuthorizeHouseholdRequired]
        public async System.Threading.Tasks.Task<ActionResult> Leave()
        {
            //get user, authenticate, find household, and remove user from this household, save
            //call the refresh authentication method
            var user = db.Users.Find(User.Identity.GetUserId());
            await ControllerContext.HttpContext.RefreshAuthentication(user);
            Household household = db.Households.Find(user.HouseholdId);
            //set user's household to null
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
            //get user and bankaccounts from db and household, remove household
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

        // GET: Households/Errors
        public ActionResult Errors(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View();
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

