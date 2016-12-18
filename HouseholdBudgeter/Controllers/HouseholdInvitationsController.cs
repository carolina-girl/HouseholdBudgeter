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
using Microsoft.AspNet.Identity;
using HouseholdBudgeter.Helpers;

namespace HouseholdBudgeter.Controllers
{
    public class HouseholdInvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HouseholdInvitations
        public ActionResult Index()
        {
            var invitation = db.HouseholdInvitation.Include(h => h.Household);
            return View(invitation.ToList());
        }

        // GET: HouseholdInvitations/JoinHousehold
        public ActionResult JoinHousehold()
        {
            return View();
        }

        // POST: HouseholdInvitations/JoinHousehold
        [HttpPost]
        public async Task<ActionResult> JoinHousehold(Guid? inviteCode)
        {
            var invite = db.HouseholdInvitation.FirstOrDefault(i => i.Invitecode == inviteCode);
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            if (invite != null && invite.Expired == false)
            {
                invite.Expired = true;
                db.HouseholdInvitation.Attach(invite);
                db.Entry(invite).Property("Expired").IsModified = true;
                db.SaveChanges();
                var household = db.Household.Find(invite.HouseholdId);
                if (household != null)
                {
                    household.Users.Add(user);
                    db.SaveChanges();
                    //await ControllerContext.HttpContext.RefreshAuthentication(user);
                    return RedirectToAction("Details", "Household");
                }
            }
            ViewBag.Message = "That invitation is invalid or has expired.";
            return View();
        }



        // GET: HouseholdInvitations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseholdInvitations householdInvitation = db.HouseholdInvitation.Find(id);
            if (householdInvitation == null)
            {
                return HttpNotFound();
            }
            return View(householdInvitation);
        }

        // GET: HouseholdInvitations/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name");
            return View();
        }

        // POST: HouseholdInvitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HouseholdId,Date,Expired,IsAccepted,Invitecode")] HouseholdInvitations householdInvitation, string Email)
        {

            return View();
        }

        // POST: HouseholdInvitations/InviteUser/5
        [HttpPost]
        public async Task<ActionResult> InviteUser(string inviteEmail)
        {
            if (!string.IsNullOrWhiteSpace(inviteEmail))
            {
                int? householdId = User.Identity.GetHouseholdId();
                string userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                HouseholdInvitations invitation = new HouseholdInvitations();
                invitation.Invitecode = Guid.NewGuid();
                invitation.Email = inviteEmail;
                invitation.HouseholdId = householdId;
                db.HouseholdInvitation.Add(invitation);
                db.SaveChanges();

                var svc = new EmailService();
                var msg = new IdentityMessage();
                msg.Destination = inviteEmail;
                msg.Subject = user.FullName + " has invited you to join the Money Manager";
                msg.Body = user.FullName + " has invited you to join their household in the Money Manager using the following invitation code: " + invitation.Invitecode;
                await svc.SendAsync(msg);
                TempData["Message"] = "Your invitation has been sent!";
            }

            return RedirectToAction("Index");
        }


        // GET: HouseholdInvitations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseholdInvitations householdInvitation = db.HouseholdInvitation.Find(id);
            if (householdInvitation == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name", householdInvitation.HouseholdId);
            return View(householdInvitation);
        }

        // POST: HouseholdInvitations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Date,Invitecode,Email,Expired,IsAccepted")] HouseholdInvitations householdInvitation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(householdInvitation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name", householdInvitation.HouseholdId);
            return View(householdInvitation);
        }

        // GET: HouseholdInvitations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseholdInvitations householdInvitation = db.HouseholdInvitation.Find(id);
            if (householdInvitation == null)
            {
                return HttpNotFound();
            }
            return View(householdInvitation);
        }

        // POST: HouseholdInvitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HouseholdInvitations householdInvitation = db.HouseholdInvitation.Find(id);
            db.HouseholdInvitation.Remove(householdInvitation);
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
