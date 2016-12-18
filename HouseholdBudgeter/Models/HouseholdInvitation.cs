using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models
{
    public class HouseholdInvitations
    {
        public HouseholdInvitations()
       {
        this.Users = new HashSet<ApplicationUser>();
       }
        public int Id { get; set; }
        public int? HouseholdId { get; set; }
        public DateTimeOffset Date { get; set; }
        public Guid Invitecode { get; set; }
        public string Email { get; set; }
        public bool Expired { get; set; }
        public bool IsAccepted { get; set; }
        public virtual Household Household { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

    }
}
