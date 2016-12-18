using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

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
        public DateTimeOffset Date { get; set; }
        public decimal Amount { get; set; }
        public decimal? ReconcileAmount { get; set; }
        public bool IsReconciled { get; set; }
        public virtual Household Household { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public bool IsDeleted { get; set; }
        public string Type { get; set; }
        public decimal Balance { get; set; }
        public Decimal StartingBalance { get; set; }
        public bool IsActive { get; set; }
        public decimal ReconciledBalance
        {
            get
            {
                return Transactions.Where(t => t.IsDeleted == false && t.IsVoid == false && t.IsReconciled == true).Sum(y => y.Amount);
            }

        }
    }
}


//    public void UpdateAccountBalance()
//    {
//        //find the transactions for this account
//        var transactions = db.Transactions.Where(x => x.AccountId == this.Id).ToList();
//        var account = db.BankAccount.Find(this.Id);
//        Decimal total = 0;
//        total += StartingBalance;
//        foreach (var t in transactions)
//        {
//            if (t.Type.Equals("Income"))
//            {
//                total += t.Amount;
//            }
//            else if (t.Type.Equals("Expense"))
//            {
//                total -= t.Amount;
//            }
//        }
//        account.Balance = total;
//        db.Entry(account).State = EntityState.Modified;
//        db.SaveChanges();
//    }

//    public void UpdateReconciledAccountBalance()
//    {
//        //find the transactions for this account
//        var transactions = db.Transactions.Where(x => x.AccountId == this.Id).ToList();
//        var account = db.BankAccount.Find(this.Id);
//        //computing reconciled balance the same way we compute the regular balance
//        Decimal recTotal = 0;
//        recTotal += StartingBalance;
//        foreach (var t in transactions)
//        {
//            if (t.Type.Equals("Income"))
//            {
//                recTotal += t.ReconcileAmount;
//            }
//            else if (t.Type.Equals("Expense"))
//            {
//                recTotal -= t.ReconcileAmount;
//            }
//        }
//        account.ReconciledBalance = recTotal;
//        db.Entry(account).State = EntityState.Modified;
//        db.SaveChanges();
//    }
//}
//}      