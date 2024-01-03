using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Timesheet.Service.ServiceInterface;

namespace Timesheet.MVC.Filters
{
    public sealed class UserRole
    {
        private UserRole() { }

        public const string Manager = "Project Manager";
        public const string Admin = "Admin";
        public const string Developer = "Senior Developer";
    }
    public class AuthorizeRole : AuthorizeAttribute
    {
        string[] UserRoles;
        private IUserService _IUserService;

        public AuthorizeRole(params string[] roles)
        {
            UserRoles = roles;
            _IUserService = new Timesheet.Service.ServiceLibrary.UserService();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext))
            {
                //if (!UserRoles.Any())
                //    return true;

                if (Array.Exists(UserRoles, E => E == httpContext.Session["IRole"].ToString()))
                {
                    return true;
                }

            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(
          AuthorizationContext filterContext)
        {
         
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary 
                    { 
                        { "Controller", "Error" }, 
                        { "Action", "UnAuthorized" } 
                    });

        }
    }
}