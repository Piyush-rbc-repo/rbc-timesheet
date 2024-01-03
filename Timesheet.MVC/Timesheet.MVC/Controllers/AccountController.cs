using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using System.Web.Security;
using Timesheet.Service.ServiceLibrary;
using Timesheet.Service.ServiceInterface;
using System.Text;

namespace Timesheet.MVC.Controllers
{
    public class AccountController : Controller
    {

        private Timesheet.Service.ServiceInterface.IMasterService _IMasterService;
        private IUserService _IUserService;
        private string Errormsg = "";
        public AccountController()
        {

            _IMasterService = new Timesheet.Service.ServiceLibrary.MasterService();
            _IUserService = new Timesheet.Service.ServiceLibrary.UserService();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated )
            {
                var username = User.Identity.Name;
               // var username = "INFRASEEPZ\\Ajit.Maid";
                PrincipalContext pc = new PrincipalContext(ContextType.Domain, "Kiya"); //Infraseepz //  Domain change 28-10-2022
                UserPrincipal principal = UserPrincipal.FindByIdentity(pc, username);
                
                //To get resource ID --Mitesh Patil
                string userid = principal.SamAccountName;
                var result = _IMasterService.GetById(5);
                var id = Convert.ToInt32(result.Where(x => x.s_MasterCode.ToLower() == userid.ToLower()).Select(x => x.n_Id).FirstOrDefault());
                Session["IUsecode"] = id;
                
                Session["IUserName"] = principal.DisplayName;
                Session["IRole"]=_IUserService.UserRolesGet(principal.DisplayName);
                return RedirectToAction("Index", "Timesheet");
            }
            else
            {
                PrincipalContext pc = null;
                UserPrincipal principal = null;
                var username = HttpContext.User.Identity.Name;

                //pc = new PrincipalContext(ContextType.Domain, "Infraseepz");
                //principal = UserPrincipal.FindByIdentity(pc, username);


                Modal.Login modal = new Modal.Login();
                modal.userid = username;
                //modal.UseWindowsAuth = true;
                return View();
            }


        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(Modal.Login modal)
        {
            try
            {

                if (modal.UseWindowsAuth)
                {



                    WindowsIdentity ident = WindowsIdentity.GetCurrent();

                    if (WindowsIdentity.GetCurrent().IsAuthenticated)
                    {

                        if (login(WindowsIdentity.GetCurrent(),modal))
                        {
                            return RedirectToAction("Index", "Timesheet");
                        }
                        else
                        {
                            ModelState.AddModelError("Login", Errormsg);
                            return View(modal);
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("Not Authenticated", "Can not login with current windows user.");
                        return View(modal);

                    }

                }
                else
                {
                    PrincipalContext pc = new PrincipalContext(ContextType.Domain, "Kiya");  //Infraseepz //  Domain change 28-10-2022
                    UserPrincipal principal;

                    // Sasmita : TODO : // comment the below function ValidateCredentials(). Instead of that 
                    // call another function which will check username and password in [tbl_LookupMaster] table with usernane,password and parentid 5
                    // change in SP [usp_AddUpdateLookupMaster] , pass @ps_Mode= something which will return LookupMasterModal<> based on username,password and parentd id=5

                    if (pc.ValidateCredentials(modal.userid, modal.password) || Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["NotRequiredPassword"]))
                    {
                        principal = UserPrincipal.FindByIdentity(pc, modal.userid);
                        WindowsIdentity ident = new WindowsIdentity(modal.userid, "Windows");
                        
                        if (login(ident,modal))
                        {
                            return RedirectToAction("Index", "Timesheet");
                        }
                        else
                        {
                            ModelState.AddModelError("Login", Errormsg);
                            return View(modal);
                        }


                    }
                    else
                    {

                        ModelState.AddModelError("Not Authenticated", "Invalid user id and password combination.");

                        return View(modal);

                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message);

                return View(modal);

            }
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Account", null);
        }


        #region private Methods
        private bool login(WindowsIdentity ident,Modal.Login modal)
        {
            var result = _IMasterService.GetById(5);
            var username = ident.Name;





            PrincipalContext pc = new PrincipalContext(ContextType.Domain, "Kiya");  //Infraseepz //  Domain change 28-10-2022
            UserPrincipal principal = UserPrincipal.FindByIdentity(pc, username);


            if (!(result.Where(x => x.s_MasterCode.ToLower() == modal.userid.ToLower()).ToList().Count > 0))
            {
                Errormsg = "Invalid User...";
                return false;

            }
      //      var count = result.Where(x => x.s_MasterCode.ToLower() == modal.userid.ToLower()).ToList().Count;
            string roleData = "user";
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, "InfraTimesheet", DateTime.Now, DateTime.Now.AddMinutes(20), false, roleData, "/");
            string encTicket = FormsAuthentication.Encrypt(ticket);
            FormsIdentity formsId = new FormsIdentity(ticket);
            GenericPrincipal princ = new GenericPrincipal(ident, new[] { "users" });
            HttpContext.User = princ;

            FormsAuthentication.SetAuthCookie(username, false);
            //New added to fix login issue - Mitesh Patil 
            var id = Convert.ToInt32( result.Where(x => x.s_MasterCode.ToLower() == modal.userid.ToLower()).Select(x =>x.n_Id).FirstOrDefault());
            Session["IUsecode"] = id;
            
            Session["IUserName"] = principal.Name;
            Session["IRole"] = _IUserService.UserRolesGet(principal.Name);
            return true;
        }

        private bool cookieCreation(UserPrincipal principal)
        {
            var cookieText = Encoding.UTF8.GetBytes("UserAuthSession");
            var encryptedValue = Convert.ToBase64String(MachineKey.Protect(cookieText, "ProtectCookie"));
            //--- Create cookie object and pass name of the cookie and value to be stored.
            HttpCookie cookieObject = new HttpCookie("UserDetailsCookie", principal.SamAccountName);

            //---- Set expiry time of cookie.
            cookieObject.Expires.AddDays(5);

            //---- Add cookie to cookie collection.
            Response.Cookies.Add(cookieObject);
            return true;
        }
        #endregion
    }
}

