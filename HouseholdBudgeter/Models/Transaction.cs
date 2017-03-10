using HouseholdBudgeter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HouseholdBudgeter.Helpers.HouseholdHelper;

namespace HouseholdBudgeter.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int BankAccountsId { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public DateTimeOffset Date { get; set; }

        [Required]
        public bool Types { get; set; }
        public bool Void { get; set; }

        public decimal Amount { get; set; }
        public decimal? ReconciledAmount { get; set; }
        public int CategoryId { get; set; }
        public bool Reconciled { get; set; }
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual BudgetCategory Category { get; set; }
        public virtual BankAccount BankAccounts { get; set; }
    }
}