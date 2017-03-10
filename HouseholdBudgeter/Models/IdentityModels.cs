using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Collections;

namespace HouseholdBudgeter.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Transactions = new HashSet<Transaction>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? HouseholdId { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual Household Households { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<BankAccount> BankAccounts { get; set; }
        public virtual DbSet<Household> Households { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<BudgetCategory> BudegetCategory { get; set; }
        public virtual DbSet<Budget> Budgets { get; set; }
        public virtual DbSet<BudgetItem> BudgetItems { get; set; }
        public virtual DbSet<SendGridCredential> SendGridCredentials { get; set; }
        public virtual DbSet<DemoLogin> Login { get; set; }
        public virtual DbSet<HouseholdInvitation> Invitations { get; set; }
    }
}