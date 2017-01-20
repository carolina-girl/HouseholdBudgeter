using System;

namespace HouseholdBudgeter.Models
{
    //need an inviation object that has householdid and userid for current logged user
    //use sendgrid system, set up the message, destination,
    //guid built inside the body, build callbackurl that is going to provide a link to the destination
    //CallBackUrl = Url.Action("Joined", "Household", new { joincode = invitation.JoinCode}, protocol:Request.Url.Scheme);
    public class HouseholdInvitation
    {
        public int Id { get; set; }
        public Guid JoinCode { get; set; }
        public string ToEmail { get; set; }
        public bool Joined { get; set; }

        public int HouseholdId { get; set; }
        public virtual Household Households { get; set; }

        public string userId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
