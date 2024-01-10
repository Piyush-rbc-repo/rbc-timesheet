using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.MVC.Filters;
using Timesheet.MVC.Models;
namespace Timesheet.MVC.Controllers
{
    //[Authorize(Users = "INFRASEEPZ\\judi.stephen,INFRASEEPZ\\kaveri.naik,INFRASEEPZ\\Diptee.Rasal,INFRASEEPZ\\Subba.Rao")]
    [AuthorizeRole(UserRole.Admin)]
    public class MasterController : Controller
    {
        #region Constructor and Global Variable
        private Timesheet.Service.ServiceInterface.ITimesheetEntryService _ITimesheetEntryService = new Timesheet.Service.ServiceLibrary.TimesheetEntryService();
        private Timesheet.Service.ServiceInterface.IMasterService _IMasterService = new Timesheet.Service.ServiceLibrary.MasterService();
        //public TimesheetController(Timesheet.Service.ServiceInterface.ITimesheetEntryService ITimesheetEntryService)
        //{
        //    this._ITimesheetEntryService = ITimesheetEntryService;

        //}

        #endregion

        #region Public Action Methods
        public ActionResult Index()
        {
           
            FillDropDowns();
            return View();
        }

        public JsonResult Search(LookupMasterSearchModal modal)
        {
            List<Modal.LookupMasterModal> result = GetMasterData(modal.MasterName);
            return Json(result, JsonRequestBehavior.AllowGet);



        }
        public ContentResult Master(int parentid)
        {

            var Query = _IMasterService.GetAll().Where(x => x.n_ParentId == parentid);
            string s = "<select>" + string.Join(",", Query.Select(x => "<Option value='" + x.n_Id.ToString() + "'>" + x.s_MasterName + "</option>").ToList()) + "</select>";


            return Content(s);


        }
        /// <summary>
        /// Function is used to display only active records of project and Cr Type in dropdowno of CR Number Master 
        /// for Parent_Id = 4 CR types will be displayed
        /// for Parent_Id = 3 Projects will be displayed
        /// </summary>
        /// <param name="parentid"></param>
        /// <param name="IsActive"></param>
        /// <returns></returns>
        public ContentResult GetDropDownValues(int parentid,bool IsActive)
        {

            var Query = _IMasterService.GetAll().Where(x => x.n_ParentId == parentid && x.b_IsActive == IsActive );
            string s = "<select>" + string.Join(",", Query.Select(x => "<Option value='" + x.n_Id.ToString() + "'>" + x.s_MasterName + "</option>").ToList()) + "</select>";


            return Content(s);

            

        }

        public ActionResult AddUpdateMaster(Modal.LookupMasterModal frm)
        {
            Modal.CommoSaveResult result;
            if(frm.Oper == "add")
            {
                result=_IMasterService.Add(frm);

            }
            else if (frm.Oper == "del")
            {
                frm.Oper = "D";
                result = _IMasterService.Add(frm);

            }
            else
            {
                result=_IMasterService.Update(frm);

            }
            if(result.pn_Error)
            {
                //return Content("0");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
              
                return Json(result,JsonRequestBehavior.AllowGet);

            }
            
        }

        public ContentResult GetLocations()
        {
             
            var locations = System.Configuration.ConfigurationManager.AppSettings["Locations"].ToString().Split(';');
            string s = "<select>" + string.Join(",", locations.Select(x => "<Option>" + x+ "</option>").ToList()) + "</select>";
            return Content(s);

        }
        #endregion

        #region private methods
        private void FillDropDowns()
        {
            var Query = _IMasterService.GetAll();

            ViewBag.Master = new SelectList(
                                                   Query.Where(x => x.n_ParentId == null  && x.b_IsActive == true).Select(x => new SelectListItem()
                                                   {
                                                       Text = x.s_MasterName,
                                                       Value = x.n_Id.ToString()


                                                   }).OrderBy(x=>x.Text).ToList() , "Value", "Text");

        }
        private List<Modal.LookupMasterModal> GetMasterData(string MasterName)
        {

            return _IMasterService.GetAll().Where(x => x.n_ParentId.ToString() == MasterName).ToList();
            
        }


        
        #endregion


    }
}
