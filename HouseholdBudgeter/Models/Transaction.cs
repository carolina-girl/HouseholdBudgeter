using HouseholdBudgeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models
{
    public class Transaction
    { 
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int AccountId { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset? ReconcileDate { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public decimal ReconcileAmount { get; set; }
        public bool IsExpense { get; set; }
        public bool IsReconciled { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual BudgetCategory BudgetCategory { get; set; }
        public virtual Type TransactionType { get; set; }
        public virtual  ApplicationUser User { get; set; }

    }
}