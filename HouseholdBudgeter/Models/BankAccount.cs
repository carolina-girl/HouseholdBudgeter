using System;
using System.Collections.Generic;

namespace HouseholdBudgeter.Models
{
    public class BankAccount
    {
        public BankAccount()
        {
            this.Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }

        public decimal Balance { get; set; }
        public decimal InitialBalance { get; set; }
        public decimal ReconcileBalance { get; set; }
        public decimal WarningBalance { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}

