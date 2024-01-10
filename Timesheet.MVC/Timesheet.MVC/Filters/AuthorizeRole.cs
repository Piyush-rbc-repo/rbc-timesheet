using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        public const string Default = "Default";
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
            if (httpContext.Session["IRole"] == null)
                return false;
            var pageAccess = CheckPageAccess(UserRoles, httpContext);
            if (base.AuthorizeCore(httpContext) && pageAccess)
                return true;
            return false;
        }

        private bool CheckPageAccess(string[] UserRoles, HttpContextBase httpContext)
        {
            return UserRoles.Any(x => x == httpContext.Session["IRole"].ToString())
                || UserRoles.Any(x => x == UserRole.Default);
        }

        protected override void HandleUnauthorizedRequest(
          AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "Controller", "Account" },
                        { "Action", "Logout" },
                        {"DisplayMessage","Session Timed out! Please Login Again!" }
                    });//()
        }
    }
}