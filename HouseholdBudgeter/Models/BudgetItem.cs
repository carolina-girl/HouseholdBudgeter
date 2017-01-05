using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public int CategoryId { get; set; }

        public decimal Amount { get; set; }
        public virtual Budget Budget { get; set; }
        public virtual BudgetCategory Category { get; set; }
    }
}