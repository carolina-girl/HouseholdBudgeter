using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
            this.BudgetCategories = new HashSet<BudgetCategory>();
            this.Budgets = new HashSet<Budget>();
            Users = new HashSet<ApplicationUser>();
            BudgetItems = new HashSet<BudgetItem>();
        }
        public int Id { get; set; }
        public int? BudgetId { get; set; }
        public string Budget { get; set; }
        public string UserId { get; set; }
        [NotMapped]
        public string FullName { get { return FirstName + " " + LastName; } }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HouseholdName { get; set; }

        public string Code { get; set; }
        public string EmailAddress { get; set; }
        public DateTimeOffset Date { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<BudgetCategory> BudgetCategories { get; set; }
        public virtual ICollection<HouseholdInvitations> HouseholdInvitations { get; set; } 
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }

    }
}
