using HouseholdBudgeter.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HouseholdBudgeter.Helpers
{
    public class AuthorizeHousehold
    {

        public class AuthorizeHouseholdRequired : AuthorizeAttribute
        {
            //creates custom Authorization attribute by creating a new class that inherits from System.Web.Mvc.AuthorizeAttribute
            //overrrides 2 methods of this class: AuthorizeCore() and HandleUnauthorizedRequest()
            //checks to see if we are logged in and part of household
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                var isAuthorized = base.AuthorizeCore(httpContext);
                if (!isAuthorized)
                {
                    return false;
                }
                return httpContext.User.Identity.IsInHousehold();
            }

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "JoinHousehold" }));

                }
            }
        }
    }
}