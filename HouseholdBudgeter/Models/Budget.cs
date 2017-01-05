using System.Collections.Generic;

namespace HouseholdBudgeter.Models
{
    public class Budget
    {
        public Budget()
        {
            //this.Transactions = new HashSet<Transaction>();
            this.BudgetItems = new HashSet<BudgetItem>();
            this.Category = new HashSet<BudgetCategory>();
            this.Household = new HashSet<Household>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int HouseHoldId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int AnnualFrequency { get; set; }
        public string Income { get; set; }
        public string Expense { get; set; }

        public virtual ICollection<BudgetItem> BudgetItems { get; set; }
        public virtual ICollection<BudgetCategory> Category { get; set; }
        public virtual ICollection<Household> Household { get; set; }
    }

}