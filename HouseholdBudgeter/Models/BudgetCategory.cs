using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models
{
    public class BudgetCategory
    {
        public BudgetCategory()
        {
            this.Transactions = new HashSet<Transaction>();
            this.BudgetItems = new HashSet<BudgetItem>();
        }
        public int Id { get; set; }
        public string Category { get; set; }
        public virtual ICollection<Transaction> Transactions {get;set;}
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
    }
}