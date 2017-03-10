using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HouseholdBudgeter.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Net.Mime;
using HouseholdBudgeter.Helpers;
using System.Web.Security;
using Microsoft.Ajax.Utilities;

namespace HouseholdBudgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class HouseholdInvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HouseholdInvitations
        [Authorize]
        public ActionResult Index()
        {
            var UserId = User.Identity.GetUserId();
            var user = db.Users.Find(UserId);

            var invitations = db.Invitations.Include(i => i.Households).Where(u => user.HouseholdId == u.HouseholdId).ToList();

            Household household = db.Households.Find(user.HouseholdId);
            if (household == null)
            {
                return RedirectToAction("Create", "Households");
            }

            return View(invitations);
        }

        public PartialViewResult _CreateInv()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            var userHousehold = db.Households.AsNoTracking().Where(u => user.HouseholdId == u.Id).ToList();

            ViewBag.HouseholdId = new SelectList(userHousehold, "Id", "Name");

            return PartialView();
        }

        // GET: HouseholdInvitations/Create
        [HttpGet]
        [Authorize]
        public PartialViewResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            var userHousehold = db.Households.AsNoTracking().Where(u => user.HouseholdId == u.Id).ToList();

            ViewBag.HouseholdId = new SelectList(userHousehold, "Id", "Name");

            return PartialView();
        }

        // POST: HouseholdInvitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "Id,HouseholdId,JoinCode,ToEmail,Joined")] HouseholdInvitation invitation)
        {
            if (ModelState.IsValid)
            {
                int? householdId = User.Identity.GetHouseholdId();
                string userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                HouseholdInvitation invitations = new HouseholdInvitation();
                invitation.JoinCode = Guid.NewGuid();
                invitation.ToEmail = invitation.ToEmail;
                invitation.HouseholdId = invitation.HouseholdId;
                db.Invitations.Add(invitation);
                db.SaveChanges();

                var es = new EmailService();
                var msg = new IdentityMessage();
                msg.Destination = invitation.ToEmail;
                msg.Subject = user.FirstName + "" + user.LastName + " has invited you to join their Money Manager household.";
                msg.Body = user.FirstName + "" + user.LastName + " has invited you to join their household in the Money Manager! To join, go to mburns-budgeter.azurewebsites.net to register as a new user, and enter the following invitation code: " + invitation.JoinCode;
                await es.SendAsync(msg);
                TempData["Message"] = "Your invitation has been sent!";

                return RedirectToAction("Index", (new
                {
                    id = user.HouseholdId
                }));
            }

            return View(invitation);
        }



        // GET: HouseholdInvitations/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            HouseholdInvitation invitation = db.Invitations.FirstOrDefault(x => x.Id == id);
            Household household = db.Households.FirstOrDefault(x => x.Id == invitation.HouseholdId);

            if (!household.Members.Contains(user))
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

           // HouseholdInvitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // POST: HouseholdInvitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            HouseholdInvitation invitation = db.Invitations.FirstOrDefault(x => x.Id == id);
            Household household = db.Households.FirstOrDefault(x => x.Id == invitation.HouseholdId);

            if (!household.Members.Contains(user))
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            //Invitation invitation = db.Invitations.Find(id);
            db.Invitations.Remove(invitation);
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


