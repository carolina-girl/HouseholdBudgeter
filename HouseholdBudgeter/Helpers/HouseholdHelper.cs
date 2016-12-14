using HouseholdBudgeter.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Helpers
{
    public class HouseholdHelper
    {
        private UserManager<ApplicationUser> userManager =
            new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private ApplicationDbContext db = new ApplicationDbContext();

        public enum TransactionType
        {
            //[Display(Name = "Expense")]
            Expense,
            //[Display(Name = "Income")]
            Income
        }
        //Send invitation
        //get email to be sent
        //removing someone from account (leave)

    }
}