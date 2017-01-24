namespace HouseholdBudgeter.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            string[] Category =
            {
                "Automobile", "Bank charges", "Childcare", "Clothing", "Credit Card Fees", "Education",
                "Events", "Food", "Flowers", "Gifts", "Household", "Healthcare", "Insurance", "Job expenses", "Leisure (daily/non-vacation)",
                "Household", "Hobbies", "Loans", "Pet Care", "Savings", "Taxes", "Utilities", "Vacation"
            };

            string[] demoName =
            {
                "mahburns@gmail.com", "Admin@HouseholdBudget.com"
            };
            string demoPassword = "Password-1";

            foreach (var c in Category)
            {
                context.BudegetCategory.Add(new BudgetCategory { Name = c });
            }

            foreach (var d in demoName)
            {
                context.DemoLogins.Add(new DemoLogin { UserName = d, Password = demoPassword });
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var household = context.Households.Add(new Household { Id = 1, Name = "Demo Household" });

            var Saving = context.BankAccounts.Add(new BankAccount
            {
                HouseholdId = household.Id,
                Name = "Saving",
                Created = new DateTimeOffset(DateTime.Now),
                Balance = 100,
            });
            var Checking = context.BankAccounts.Add(new BankAccount
            {
                HouseholdId = household.Id,
                Name = "Checking",
                Created = new DateTimeOffset(DateTime.Now),
                Balance = 1000,
            });
            var uStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(uStore);

            if (!userManager.Users.Any(u => u.Email == "mahburns@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "mahburns@gmail.com",
                    Email = "mahburns@gmail.com",
                    FirstName = "Mary",
                    LastName = "Burns",
                    HouseholdId = household.Id,
                    EmailConfirmed = true
                }, demoPassword);
            }
            if (!context.Users.Any(u => u.Email == "Admin@HouseholdBudget.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    Email = "Admin@HouseholdBudget.com",
                    UserName = "Admin@HouseholdBudget.com",
                    FirstName = "Demo-Admin-User",
                    LastName = "Household-Budgeter-Application",
                    HouseholdId = household.Id,
                    EmailConfirmed = true
                }, demoPassword);
            }
        }
    }
}
