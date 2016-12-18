using HouseholdBudgeter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HouseholdBudgeter.Helpers
{
        public class AccountUserHelper
        {
            private ApplicationDbContext db;

            public AccountUserHelper()
            {
                db = new ApplicationDbContext();
            }

            public bool CanUserAccessAccount(string userId, int accountId)
            {
                bool canAccess = false;
                var user = db.Users.Find(userId);
                var hId = user.HouseholdId;
                var accountsInHousehold = db.BankAccount.Where(x => x.HouseholdId == hId).ToList();
                if (accountsInHousehold.Where(x => x.Id == accountId).ToList().Count > 0)
                {
                    canAccess = true;
                }
                return canAccess;
            }
        }
    }
