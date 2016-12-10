using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models
{
    public class Household
    {
        public Household()
        {
            this.BankAccounts = new HashSet<BankAccount>();
            this.HouseholdInvitations = new HashSet<HouseholdInvitations>();
            this.Budgets = new HashSet<Budget>();
            this.Users = new HashSet<ApplicationUser>();
        }
        public int Id { get; set; }
        public int? BudgetId { get; set; }
        public string Name {get;set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public DateTimeOffset Date { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<HouseholdInvitations> HouseholdInvitations { get; set; } 
        public virtual ICollection<Budget> Budgets { get; set; }

    }
}