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
        public async Task<ActionResult> Create([Bind(Include = "Id,HouseholdId,Date,Expired,IsAccepted,Invitecode")] HouseholdInvitations householdInvitation, string Email)
           { 
            if (!string.IsNullOrWhiteSpace(Email))
               {
                int? householdId = User.Identity.GetHouseholdId();
                string userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                HouseholdInvitations invitation = new HouseholdInvitations();
                invitation.Invitecode = Guid.NewGuid();
                invitation.Email = invitation.Email;
                invitation.HouseholdId = invitation.HouseholdId;
                db.HouseholdInvitation.Add(invitation);
                db.SaveChanges();

                var svc = new EmailService();
                var msg = new IdentityMessage();
                msg.Destination = invitation.Email;
                msg.Subject = user.FirstName + "" + user.LastName + " has invited you to join their Money Manager household.";
                msg.Body = user.FirstName + "" + user.LastName + " has invited you to join their household in the Money Manager! To join, go to budgeter.azurewebsites.net and enter the following invitation code: " + invitation.Invitecode;
                await svc.SendAsync(msg);
                TempData["Message"] = "Your invitation has been sent!";
            
            return RedirectToAction("InvitationSent", "HouseholdInvitations");
              }
        ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name", householdInvitation.HouseholdId);
                return View();
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
