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

namespace HouseholdBudgeter.Controllers
{
    public class HouseholdInvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Guid invitecode;

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
        public ActionResult Create([Bind(Include = "Id,HouseholdId,Date,Email,Expired,IsAccepted,Invitecode")] HouseholdInvitations householdInvitation)
        {
            if (ModelState.IsValid)
            {
                var invite = db.HouseholdInvitation.FirstOrDefault(i => i.Invitecode == invitecode);
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                if (invite != null && invite.Expired == false)
                {
                    invite.Expired = true;
                    db.HouseholdInvitation.Attach(invite);
                    db.Entry(invite).Property("Expired").IsModified = true;
                    db.SaveChanges();
                    var household = db.Household.Find(invite.HouseholdId);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

                ViewBag.HouseholdId = new SelectList(db.Household, "Id", "Name", householdInvitation.HouseholdId);
                return View(householdInvitation);
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
