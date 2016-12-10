using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }
        public int BudgetCategoryId { get; set; }
        public int BudgetId { get; set; }
        public int Frequency { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public virtual Budget Budget { get; set; }
        public virtual BudgetCategory BudgetCategory { get; set; }

    }
}