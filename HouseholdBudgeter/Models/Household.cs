using HouseholdBudgeter.Models;
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
            this.Members = new HashSet<ApplicationUser>();
            this.BankAccounts = new HashSet<BankAccount>();
            this.Budgets = new HashSet<Budget>();
            this.Categories = new HashSet<BudgetCategory>();
            this.Invitations = new HashSet<HouseholdInvitation>();
            this.BudgetItems = new HashSet<BudgetItem>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string JoinCode { get; set; }

        public virtual ICollection<ApplicationUser> Members { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        //already in BANKACCOUNTS, dont need to ask twice
        //public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<BudgetCategory> Categories { get; set; }
        public virtual ICollection<HouseholdInvitation> Invitations { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
    }
}
