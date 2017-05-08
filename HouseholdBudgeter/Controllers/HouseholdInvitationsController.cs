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
            //get user and invitations of this household from db
            var user = db.Users.Find(User.Identity.GetUserId());

            var invitations = db.Invitations.Include(i => i.Households).Where(u => user.HouseholdId == u.HouseholdId).ToList();
            //check to see that this user has a household, and list the invitations from it
            Household household = db.Households.Find(user.HouseholdId);

            if (household == null)
            {
                return RedirectToAction("Create", "Households");
            }

            return View(invitations);
        }

        public PartialViewResult _CreateInv()
        {
            //get user and household, create select list of this users households
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

                var user = db.Users.Find(User.Identity.GetUserId());

                var existingUser = db.Users.Where(u => u.Email == invitation.ToEmail).FirstOrDefault();

                Household household = db.Households.FirstOrDefault(h => h.Id == householdId);

                HouseholdInvitation invitations = new HouseholdInvitation();

                invitation.JoinCode = Guid.NewGuid();
                invitation.ToEmail = invitation.ToEmail;
                invitation.HouseholdId = invitation.HouseholdId;
                db.Invitations.Add(invitation);
                db.SaveChanges();
    
                var es = new EmailService();
                var msg = new IdentityMessage();
                if (existingUser != null)
                {
                    var callbackUrlForExitingUser = Url.Action("JoinHousehold", "Account", new { inviteHouseholdId = invitation.HouseholdId }, protocol: Request.Url.Scheme);
                    msg.Destination = invitation.ToEmail;
                    msg.Subject = user.FirstName + "" + user.LastName + " has invited you to join their Money Manager household.";
                    msg.Body = user.FirstName + "" + user.LastName + " invites you to join the " + household.Name + " household on the Money Manager household-budgeter application. Click <a href=\"" + callbackUrlForExitingUser + "\" target=\"_blank\">here</a> to join.";
                    await es.SendAsync(msg);
                    TempData["Message"] = "Your invitation has been sent!";
                }
                else
                {
                    var callbackUrl = Url.Action("Register", "Account", new { inviteHouseholdId = invitation.HouseholdId, invitationId = invitation.Id, guid = invitation.JoinCode }, protocol: Request.Url.Scheme);
                    msg.Destination = invitation.ToEmail;
                    msg.Subject = user.FirstName + "" + user.LastName + " has invited you to join their Money Manager household.";
                    msg.Body = user.FirstName + "" + user.LastName + " invites you to join the " + household.Name + " household on the Money Manager household-budgeter application. Click <a href=\"" + callbackUrl + "\" target=\"_blank\">here</a> to join. Enter the code " + invitation.JoinCode + ".";
                    await es.SendAsync(msg);
                    TempData["Message"] = "Your invitation has been sent!";
                }
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
            //get user, invitation, and household from db
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
           //HouseholdInvitation invitation = db.Invitations.Find(id);
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
            //get user, invitation, and household from db
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


