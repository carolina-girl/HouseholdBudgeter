using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models
{
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
