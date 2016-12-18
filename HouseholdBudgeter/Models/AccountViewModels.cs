using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HouseholdBudgeter.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "EmailAddress")]
        public string EmailAddress { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "EmailAddress")]
        public string EmailAddress { get; set; }
    }

    public class LoginViewModel
    {
        public string Name { get; set; }

        [Required]
        [Display(Name = "EmailAddress")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }

    public class RegisterViewModel
    {


        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Display(Name = "Mobile Phone")]
        public string MobilePhone { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "EmailAddress")]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        //[Required]
        //[EmailAddress]
        //[Display(Name = "First Name")]
        //public string FirstName { get; set; }

        //[Required]
        //[EmailAddress]
        //[Display(Name = "Last Name")]
        //public string LastName { get; set; }

        //[Required]
        //[EmailAddress]
        //[Display(Name = "Mobile Phone")]
        //public string MobilePhone { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "EmailAddress")]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "EmailAddress")]
        public string EmailAddress { get; set; }
    }

    public class UserProfileViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Mobile Phone")]
        public string MobilePhone { get; set; }

        public int? HouseholdId { get; set; }
        public string HouseholdName { get; set; }

        public bool HasPassword { get; set; }
    }

}

