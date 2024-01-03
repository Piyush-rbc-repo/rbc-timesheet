using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Timesheet.Database;
namespace Timesheet.MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
      

       protected void Session_Abandon(Object sender, EventArgs e)
        {

            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }
        }

       protected void Session_End(Object sender, EventArgs e)
       {
            FormsAuthentication.SignOut();
       }

        //protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        //{


        //    //WindowsIdentity ident = WindowsIdentity.GetCurrent();
        //    //WindowsPrincipal wind_princ = new WindowsPrincipal(ident);
        //    //if (ident.IsAuthenticated)
        //    //{

        //    //    string roleData = "user";
        //    //    FormsAuthenticationTicket ticket =
        //    //      new FormsAuthenticationTicket(1,
        //    //        "InfraTimesheet", DateTime.Now,
        //    //        DateTime.Now.AddMinutes(30), false, roleData, "/");
        //    //    string encTicket = FormsAuthentication.Encrypt(ticket);
        //    //    FormsIdentity formsId = new FormsIdentity(ticket);
        //    //    GenericPrincipal princ = new GenericPrincipal(ident, null);
        //    //    HttpContext.Current.User = princ;
        //    //}




        ////    bool cookieFound = false;

        ////    HttpCookie authCookie = null;
        ////    HttpCookie cookie;

        ////    for (int i = 0; i < Request.Cookies.Count; i++)
        ////    {
        ////        cookie = Request.Cookies[i];

        ////        if (cookie.Name == FormsAuthentication.FormsCookieName)
        ////        {
        ////            cookieFound = true;
        ////            authCookie = cookie;
        ////            break;
        ////        }
        ////    }

        ////    // If the cookie has been found, it means it has been issued from either 
        ////    // the windows authorisation site, is this forms auth site.
        ////    if (cookieFound)
        ////    {
        ////        // Extract the roles from the cookie, and assign to our current principal, 
        ////        // which is attached to the HttpContext.
        ////        FormsAuthenticationTicket winAuthTicket =
        ////          FormsAuthentication.Decrypt(authCookie.Value);
        ////        //string[] roles = winAuthTicket.UserData.Split(';');
        ////        string[] roles =new string[1] {"Project Manager"};
        ////        FormsIdentity formsId = new FormsIdentity(winAuthTicket);
        ////        GenericPrincipal princ = new GenericPrincipal(formsId, roles);
        ////        HttpContext.Current.User = princ;
        ////    }
        ////    else
        ////    {
        ////        // No cookie found, we can redirect to the Windows auth site if we want, 
        ////        // or let it pass through so that the forms auth system redirects to 
        ////        // the logon page for us.
        ////    }
        //}


        
    }
}