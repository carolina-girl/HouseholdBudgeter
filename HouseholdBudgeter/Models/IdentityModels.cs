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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string MobilePhone { get; set; }
        public string TheEmailConfirmed { get; set; }
        public int? HouseholdId { get; set; }
        public virtual Household Household { get; set; }


        public ApplicationUser()
        {
            this.Transactions = new HashSet<Transaction>();
        }

        public virtual ICollection<Transaction> Transactions { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
{
    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("HouseholdId", HouseholdId.ToString()));

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
        public DbSet<BankAccount> BankAccount { get; set; }
        public DbSet<Budget> Budget { get; set; }
        public DbSet<BudgetCategory> BudgetCategory { get; set; }
        public DbSet<BudgetItem> BudgetItem { get; set; }
        public DbSet<Household> Household { get; set; }
        public DbSet<HouseholdInvitations> HouseholdInvitation { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

    }
}