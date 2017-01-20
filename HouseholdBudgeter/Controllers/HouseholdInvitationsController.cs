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
            var user = db.Users.Find(User.Identity.GetUserId());

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
                var user = db.Users.Find(User.Identity.GetUserId());
                var existingUser = db.Users.Where(u => u.Email == invitation.ToEmail).FirstOrDefault();
                Household household = db.Households.Find(user.HouseholdId);

                invitation.JoinCode = Guid.NewGuid();
                invitation.HouseholdId = household.Id;
                db.Invitations.Add(invitation);
                await db.SaveChangesAsync();
                try
                {
                    //Build Email Message
                    MailMessage inviteMessasge = new MailMessage();
                    inviteMessasge.To.Add(new MailAddress(invitation.ToEmail, invitation.ToEmail));
                    inviteMessasge.From = new MailAddress(user.Email, "From");
                    inviteMessasge.Subject = "Household-Budget: Invitation to Join a Household";

                    //if receiving user is registered, send join code
                    if (existingUser != null)
                    {
                        var callbackUrlForExistingUser = Url.Action("JoinHousehold", "Account", new { inviteHouseholdId = invitation.HouseholdId }, protocol: Request.Url.Scheme);

                        string bodytext = String.Concat("<p>I would like to invite you to join my household <mark>", household.Name,
                                    "</mark> in the Household-Budget app budgeting system", "</p> <p><a href='"
                                    , callbackUrlForExistingUser, "'>Join</a></p>");
                        inviteMessasge.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodytext, null, MediaTypeNames.Text.Html));

                    }
                    //if receiving user is not registered, register user and send join code
                    else
                    {
                        var callbackUrl = Url.Action("RegisterToJoinHousehold", 
                            "Account", new { inviteHouseholdId = invitation.HouseholdId,
                                invitationId = invitation.Id,
                                guid = invitation.JoinCode }, 
                            protocol: Request.Url.Scheme);

                        string html = String.Concat("<p>I would like to invite you to join my household <mark>", household.Name,
                                        "</mark> in the Household-Budget app budgeting system.</p> <p><a href='", callbackUrl, "'>Join</a></p>");


                        inviteMessasge.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));
                        var ems = new EmailService();
                        await ems.SendAsync(inviteMessasge);

                    }
                    //Initialise SmtpClient and send
                    //SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                    //var SendGridCredentials = db.SendGridCredentials.First();
                    //NetworkCredential credentials = new NetworkCredential(SendGridCredentials.UserName, SendGridCredentials.Password);
                    //smtpClient.Credentials = credentials;
                    //smtpClient.Send(inviteMessasge);

                    return RedirectToAction("Index", "HouseholdInvitations");

                }
                catch (Exception ex)
                {
                    ViewBag.Exception = ex.Message;
                    return View(invitation);
                }
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", invitation.HouseholdId);
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
            //Invitation invitation = db.Invitations.Find(id);
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


