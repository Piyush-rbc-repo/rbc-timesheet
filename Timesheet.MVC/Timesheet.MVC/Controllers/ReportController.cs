using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using Timesheet.Modal;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Globalization;
using System.Data;
using Timesheet.MVC.Filters;
using System.Reflection;
using ClosedXML.Excel;
using Newtonsoft.Json;
using System.Drawing;

namespace Timesheet.MVC.Controllers
{
    //[Authorize(Users = "INFRASEEPZ\\judi.stephen,INFRASEEPZ\\girish.punjabi,INFRASEEPZ\\sushil.wagh,INFRASEEPZ\\kanwalpreet.nirmal,INFRASEEPZ\\kaveri.naik")]
     [AuthorizeRole(Timesheet.MVC.Filters.UserRole.Manager,Timesheet.MVC.Filters.UserRole.Admin)]
    public class ReportController : Controller
    {
        #region Global variabled and constructor
        private Timesheet.Service.ServiceInterface.IDashboardService _IDashboardService;
        private Timesheet.Service.ServiceInterface.ITimesheetEntryService _ITimesheetEntryService;
        private Timesheet.Service.ServiceInterface.IMasterService _IMasterService;
        public ReportController()
        {
            _IDashboardService = new Timesheet.Service.ServiceLibrary.DashboardService();
            _ITimesheetEntryService = new Timesheet.Service.ServiceLibrary.TimesheetEntryService();
            _IMasterService = new Timesheet.Service.ServiceLibrary.MasterService();

        }
        #endregion
        #region Action Methods
        public ActionResult Index()
        {
            
            var monthyear = DateTime.Now.ToString("MMMM") + " - " + DateTime.Now.Year.ToString();
            
            ReportSearchModal modal = new ReportSearchModal();
            FillDropdowns();
            modal.MonthId = (from s in (SelectList)ViewBag.MonthLst
                             where (s.Text.ToLower() == monthyear.ToLower())
                             select Convert.ToInt32( s.Value.ToString())
                             ).FirstOrDefault();
            modal.WeekId = (from s in (SelectList)ViewBag.WeekIdLst
                            where (s.Text.ToLower()== "all")
                            select Convert.ToInt32(s.Value.ToString())
                             ).FirstOrDefault();
            return View(modal);
        }
        public ActionResult Search(ReportSearchModal modal)
        {
 
            List<TimesheetSearchResultModal> results = _IDashboardService.Search(modal);
            var result = results.Where(x => x.VisibleToUser == true).ToList();
            if(Session["IRole"].ToString() == "Project Manager")
            {

            var resourceid = (int)TempData.Peek("resourceid");
            results = results.Where(x => x.ManagerId == resourceid).ToList();

            }

            var summary = (from r in results
                           group r by new
                           {
                               CrNumber = r.CrNumber,
                               ProjectName = r.ProjectName,
                               CrNumberId = (string.IsNullOrEmpty(r.CrNumber) ? null : r.CrNumberId),
                               ProjectId =r.ProjectId 
                              // Cost = r.Approved_Cost

                           } into res
                           select new SummaryReport()
                           {
                               CrNumber = res.Key.CrNumber,
                               ProjectName = res.Key.ProjectName,
                               crId = res.Key.CrNumberId,
                               ProjectId = res.Key.ProjectId,
                               BilledHrs = (float)res.Sum(x => (x.Billable == "Yes" ? x.ActualEfforts : 0)),
                               BilledDays = (res.Sum(x => (x.Billable == "Yes" ? x.ActualEfforts /8  : 0))).ToString(),
                               UnBilledDays = (float)res.Sum(x => (x.Billable == "Yes" ? 0 : x.ActualEfforts / 8)),
                              // UnBilledDays = (float)res.Sum(x => ((x.Efforts - x.ActualEfforts < 0 ? 0 : x.Efforts - x.ActualEfforts) / 8)),
                               ApprovedCost = (float)res.Sum(x=>x.Approved_Cost) 
                               //* (float)(res.Sum(x => (x.Billable == "Yes" ? x.Efforts_Days : 0)))
                               
                               
                           });



            TempData["details"] = result;
            return new JsonResult()
            {

                Data = new { Summary = summary},
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public ActionResult GetDetails(int? CrNumberId, int? ProjectId,int? ResourceId ,int MonthId ,int WeekId,bool statusId)
        {
            //if (TempData.Peek("details") != null)
            //{

            var modal = new ReportSearchModal();
            modal.ResourceId = ResourceId;
            modal.MonthId = MonthId;
            modal.WeekId = WeekId;
            modal.statusId = statusId;
            List<TimesheetSearchResultModal> results = _IDashboardService.Search(modal);
            var result = results.Where(x => x.VisibleToUser == true).ToList();
            //List<TimesheetSearchResultModal> tm = (List<TimesheetSearchResultModal>)TempData.Peek("details");

            //Commented by Sasmita and added Conditon for CrNumberId to avoid issue in fetching the deatils after saving the records.Issue was with the projects which have NULL crId

            //var res = result.Where(x => x.ProjectId == (!ProjectId.HasValue ? x.ProjectId : (int)ProjectId) &&
            // x.CrNumberId == CrNumberId).ToList();

            var res = result.Where(x => x.ProjectId == (!ProjectId.HasValue ? x.ProjectId : (int)ProjectId) &&
                                        x.CrNumberId == (!CrNumberId.HasValue ? x.CrNumberId : (int)CrNumberId)).ToList();


            return Json(res , JsonRequestBehavior.AllowGet);
                
            //}
            //else
            //{
            //    return new HttpStatusCodeResult(400, "Please refetch the summary and try to get details of it!!!");

            //}
            
        }
        public ActionResult ApproveReject(TimesheetApproverModal modal)
        {

            if(TempData.Peek("details")==null)
            {

                return Json(new { pb_Error=true,ps_Msg="Invalid Session"}, JsonRequestBehavior.AllowGet);
            }
           

            int userid = (int)TempData.Peek("resourceid");
            List<TimesheetSearchResultModal> Data = (List<TimesheetSearchResultModal>)TempData.Peek("details");
            var insertRecords= true;
            if (string.IsNullOrWhiteSpace(modal.Ids))
            {
                modal.Ids = string.Join(",", Data.Select(x => x.n_id.ToString()).ToList<string>());
                insertRecords=false;
            }

            var result = _ITimesheetEntryService.ApproveReject(modal, userid, insertRecords);
           
           return Json(result,JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost] 
        public ActionResult ModifyData( List<TimesheetSearchResultModal> modal)
        {
           var result= _ITimesheetEntryService.UpdateRecordFromApproverDashboard(modal,0);
           return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult GetProjectCrLevelDashboard(ProjectCrBasedSearchModal modal)
        {
            var returndata =_ITimesheetEntryService.GetProjectCrBasedDashboard(modal);
            return Json(returndata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNoOfResource(ReportSearchModal modal)
        {
            List<TimesheetSearchResultModal> result = _IDashboardService.Search(modal);

            if (result.Count == 0)
            {
                 RedirectToAction("NoDataFound", "Error");
            }
          
            var groupedResult = result.GroupBy(x => x.ResoureId).Select(grp => grp.ToList()).ToList();
            var groupedResultCount = groupedResult.Count;
            return Json(groupedResultCount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Excel(ReportSearchModal modal,string noOfResource)
        {
            string offshoreOnshoreFlag = string.Empty;
            string strLocation = string.Empty;

            // Step 1 - get the data from database
            List<TimesheetSearchResultModal> result = _IDashboardService.Search(modal);
            
            if(result.Count==0)
            {
                return RedirectToAction("NoDataFound", "Error");
            }
            int noOfRes = int.Parse(noOfResource);
            var groupedResult= result.GroupBy(x => x.ResoureId).Select(grp => grp.ToList()).ToList();
            var groupedResultCount = groupedResult.Count;
            ViewBag.hdnFlag = groupedResultCount;

            //added by Sasmita to append Onshore/Offshore dynamically in excel filename
            strLocation = Convert.ToString(result.Select(x => x.Location).First());
            if(!string.IsNullOrEmpty(strLocation))
            offshoreOnshoreFlag = strLocation.ToLower() == "india" ? "Offshore" : "Onshore";

            
                // instantiate the GridView control from System.Web.UI.WebControls namespace
                // set the data source
            GridView gridview = new GridView();
                gridview.AutoGenerateColumns = false;
                gridview.Columns.Add(new BoundField() { DataField = "ActivityDate", HeaderText = "Date" });
                gridview.Columns.Add(new BoundField() { DataField = "ResourceName", HeaderText = "Resource Name" });
                gridview.Columns.Add(new BoundField() { DataField = "ProjectName", HeaderText = "Project" });
                gridview.Columns.Add(new BoundField() { DataField = "CrNumber", HeaderText = "CR Number" });
                gridview.Columns.Add(new BoundField() { DataField = "Activity", HeaderText = "Activity" });
                gridview.Columns.Add(new BoundField() { DataField = "SubActivity", HeaderText = "Sub Activity" });
                gridview.Columns.Add(new BoundField() { DataField = "ActualEfforts", HeaderText = "Efforts (Hrs)" });
                gridview.Columns.Add(new BoundField() { DataField = "Efforts_Days", HeaderText = "Efforts (Days)" });
                gridview.Columns.Add(new BoundField() { DataField = "Billable", HeaderText = "Billable Flag" });
                gridview.Columns.Add(new BoundField() { DataField = "Comments", HeaderText = "Comments" });
                gridview.DataSource = groupedResult[noOfRes];
                gridview.DataBind();
                gridview.Columns[0].HeaderStyle.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
                gridview.Columns[1].HeaderStyle.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
                gridview.Columns[2].HeaderStyle.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
                gridview.Columns[3].HeaderStyle.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
                gridview.Columns[4].HeaderStyle.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
                gridview.Columns[5].HeaderStyle.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
                gridview.Columns[6].HeaderStyle.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
                gridview.Columns[7].HeaderStyle.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
                gridview.Columns[8].HeaderStyle.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
                gridview.Columns[9].HeaderStyle.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            //gridview.Columns[0].ItemStyle.Width = 100;
            //gridview.HeaderRow.BackColor = System.Drawing.Color.FromArgb(192,192,192);

            // Clear all the content from the current response

           
            Response.ClearContent();               
            Response.Buffer = true; 
                DateTime dt = DateTime.ParseExact(groupedResult[noOfRes][0].ActivityDate, "dd/MM/yyyy",
                                  CultureInfo.InvariantCulture);
                string month = dt.ToString("MMMM");
            //string OutputFileName = (groupedResult[noOfRes][0].ResourceName.ToString()).Replace(' ', '_') + "_" +
            //                    DateTime.Parse(groupedResult[noOfRes][0].ActivityDate, new CultureInfo("en-CA")).ToString("MMMM") + "_" +
            //                    DateTime.Parse(groupedResult[noOfRes][0].ActivityDate, new CultureInfo("en-CA")).ToString("yyyy") + " _TIMESHEET_Offshore";


            //Commented By Sasmita to append Onshore/Offshore dynamically in excel filename
            //string OutputFileName = (groupedResult[noOfRes][0].ResourceName.ToString()).Replace(' ', '_') + "_" +
            //                      month + "_" +
            //                       dt.ToString("yyyy") + " _TIMESHEET_Offshore";


            string OutputFileName = (groupedResult[noOfRes][0].ResourceName.ToString()).Replace(' ', '_') + "_" +
                                      month + "_" +
                                       dt.ToString("yyyy") + " _TIMESHEET_"+ offshoreOnshoreFlag;


            // set the header
            Response.AddHeader("content-disposition", "attachment;filename=" + OutputFileName + ".xls");            
            Response.ContentType = "application/ms-excel";            
            Response.Charset = ""; 

                // create HtmlTextWriter object with StringWriter
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                    {
                        // render the GridView to the HtmlTextWriter
                        gridview.RenderControl(htw);
                        // Output the GridView content saved into StringWriter
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                        Response.End();
                    }
                }
            
            return View();
        }

        //Added by Sasmita to fix Excel Issues
        public ActionResult DownloadExcel(ReportSearchModal modal, string noOfResource)
        {
            string offshoreOnshoreFlag = string.Empty;
            string strLocation = string.Empty;

            MemoryStream MyMemoryStream = null;

            // Step 1 - get the data from database
            List<TimesheetSearchResultModal> result = _IDashboardService.Search(modal);

            if (result.Count == 0)
            {
                return RedirectToAction("NoDataFound", "Error");
            }
            int noOfRes = int.Parse(noOfResource);
            var groupedResult = result.GroupBy(x => x.ResoureId).Select(grp => grp.ToList()).ToList();
            var groupedResultCount = groupedResult.Count;
            ViewBag.hdnFlag = groupedResultCount;

            if(modal.ResourceId != null)
            strLocation = Convert.ToString(result.Select(x => x.Location).First());

            if (!string.IsNullOrEmpty(strLocation))
                offshoreOnshoreFlag = strLocation.ToLower() == "india" ? "_TIMESHEET_Offshore" : "_TIMESHEET_Onshore";

            
            DateTime dt = DateTime.ParseExact(groupedResult[noOfRes][0].ActivityDate, "dd/MM/yyyy",
                             CultureInfo.InvariantCulture);

            string month = dt.ToString("MMMM");

            DataTable dtTimesheet = ToDataTable<TimesheetSearchResultModal>(result);

            // Append filename with "AllResources" when no Resource  is selected else append with selected Resource Name

            string OutputFileName = (modal.ResourceId == null ? "AllResources" : groupedResult[noOfRes][0].ResourceName.ToString()).Replace(' ', '_') + "_" +
                                     month + "_" +                                     
                                     dt.ToString("yyyy") +  offshoreOnshoreFlag;


            string worksheetName = (modal.ResourceId == null ? "AllResources" : groupedResult[noOfRes][0].ResourceName.ToString()).Replace(' ', '_');
               // + "_" + month + "_" + dt.ToString("yyyy");

            //Remove the unwanted columns which are not required to display in excel
            dtTimesheet.Columns.Remove("n_id");
            dtTimesheet.Columns.Remove("CrProject");
            dtTimesheet.Columns.Remove("Status");
            dtTimesheet.Columns.Remove("CreatedOn");
            dtTimesheet.Columns.Remove("CrNumberId");
            dtTimesheet.Columns.Remove("ProjectId");
            dtTimesheet.Columns.Remove("ResoureId");
            dtTimesheet.Columns.Remove("n_status");
            dtTimesheet.Columns.Remove("Approved_Cost");
            dtTimesheet.Columns.Remove("ManagerId");
            dtTimesheet.Columns.Remove("RejectionReason");
            dtTimesheet.Columns.Remove("ActualEfforts");
            dtTimesheet.Columns.Remove("VisibleToUser");
            dtTimesheet.Columns.Remove("Location");
            dtTimesheet.Columns.Remove("ActivityId");
            dtTimesheet.Columns.Remove("CrLongName");
            dtTimesheet.Columns.Remove("OnsiteManagerName");
            dtTimesheet.Columns.Remove("ProjectType");

            //Rename the columns to show in Excel
            dtTimesheet.Columns["ActivityDate"].ColumnName = "Date";
            dtTimesheet.Columns["ResourceName"].ColumnName = "Resource Name";
            dtTimesheet.Columns["ProjectName"].ColumnName = "Project";
            dtTimesheet.Columns["CrNumber"].ColumnName = "CR Number";
            dtTimesheet.Columns["Activity"].ColumnName = "Activity";
            dtTimesheet.Columns["SubActivity"].ColumnName = "Sub Activity";
            dtTimesheet.Columns["Efforts"].ColumnName = "Efforts (Hrs)";
            dtTimesheet.Columns["Efforts_Days"].ColumnName = "Efforts (Days)";
            dtTimesheet.Columns["Billable"].ColumnName = "Billable Flag";
            dtTimesheet.Columns["Comments"].ColumnName = "Comments";


            using (XLWorkbook wb = new XLWorkbook())
            {

                // wb.Worksheets.Add(dtTimesheet, worksheetName);
               var wsTS = wb.Worksheets.Add(worksheetName);

                // Create Column Name Headers  and apply styling in Timesheet Report
                string[] columnNamesInTS = (from dc in dtTimesheet.Columns.Cast<DataColumn>()
                                                 select dc.ColumnName).ToArray();

                for (int c = 0; c < columnNamesInTS.Length; c++)
                {
                    var columnNumber = c + 1;
                    wsTS.Cell(1, columnNumber).Value = columnNamesInTS[c];
                    wsTS.Cell(1, columnNumber).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#bfbfc2")); //Gray Color
                    wsTS.Cell(1, columnNumber).Style.Font.Bold = true;
                    wsTS.Cell(1, columnNumber).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wsTS.Cell(1, columnNumber).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                   
                }
                //Insert timesheet data
                wsTS.Cell(2, 1).InsertData(dtTimesheet);

                wsTS.Row(1).AdjustToContents(30.00, 30.00); // To give height to the Headers
                wsTS.Columns().AdjustToContents();

                //Assign Border to the Content 
                wsTS.Range(1, 1, dtTimesheet.Rows.Count + 1, columnNamesInTS.Length).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                wsTS.Range(1, 1, dtTimesheet.Rows.Count + 1, columnNamesInTS.Length).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;



                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;             

                using ( MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.Position = 0;                   
                }
            }
                                   

            return File(MyMemoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", OutputFileName+".xlsx");

            // return View();
        }

        //Added by Sasmita to Extract monthly report / Invoice in Multiple worksheets
        public ActionResult MonthlyExport(ReportSearchModal modal)
        {
           
            MemoryStream MyMemoryStream = null;

            string summaryWorksheetName = string.Empty;
            string fixedBillingWorksheetName = string.Empty;
            string fixedBillingCreditNoteWorksheetName = string.Empty;

            DataTable dtSummary = new DataTable();
           // dtSummary.Columns.Add("Sr. No.",typeof(int));
            dtSummary.Columns.Add("Location");
            dtSummary.Columns.Add("Project");
            dtSummary.Columns.Add("Total",typeof(decimal));
            dtSummary.Columns.Add("Invoice Number");
            dtSummary.Columns.Add("Invoice Manager");
            DataRow drSummary;
            //dtSummary.Columns[0].AutoIncrement = true;

            int SlNo = 0;


            DataTable dtFixedCostBilling = new DataTable();
            dtFixedCostBilling.Columns.Add("ResourceId");
            dtFixedCostBilling.Columns.Add("ResourceName");
            dtFixedCostBilling.Columns.Add("ProjectID");
            dtFixedCostBilling.Columns.Add("ProjectName");
            dtFixedCostBilling.Columns.Add("WorkingDays",typeof(decimal));
            DataRow drFixedBill;


            DataTable dtSupportProjects = new DataTable();
            dtSupportProjects.Columns.Add("ProjectID");
            dtSupportProjects.Columns.Add("ProjectName");
            DataRow drSuppProj;

            DataTable dtSupportTeam = new DataTable();
            dtSupportTeam.Columns.Add("ResourceId",typeof(string));
            dtSupportTeam.Columns.Add("ResourceName", typeof(string));
           // DataRow drSuppTeam;

            // Step 1 - get the data from database
            List<TimesheetSearchResultModal> result = _IDashboardService.Search(modal);

            if (result.Count == 0)
            {
                return RedirectToAction("NoDataFound", "Error");
            }

       

            //Fetch Active Resources from master table with all the details Roles,cost etc
            var Query = _ITimesheetEntryService.GetAllDropdown();
            List<SelectListModal>  ResourceList = Query.Where(x => x.MasterName.ToUpper() == "RESOURCE").ToList();

            DataTable dtResourceMaster = ToDataTable<SelectListModal>(ResourceList);


            //Fetch Onsite support resources  and assign to datatable dtSupportTeam
            var suppTeamList = from p in dtResourceMaster.AsEnumerable()
                        where p.Field<string>("s_Value6") == "Support"
                        && p.Field<string>("s_Value3").ToLower() == "jersey"
                        orderby p.Field<string>("Text") //order by ResourceName
                        select new
                        {
                            ResourceId = p.Field<string>("Value"),
                            ResourceName = p.Field<string>("Text")

                        };
            dtSupportTeam = ToDataTable(suppTeamList);

            string supportTeamIds = "";
            if (dtSupportTeam.Rows.Count > 0)
            {
                foreach (DataRow dr in dtSupportTeam.Rows)
                {
                    supportTeamIds = supportTeamIds + "," + dr[0];
                }

                supportTeamIds = supportTeamIds.TrimStart(new char[] { ',' }); // returns 1453,1865,1454,1601,1458,1550,144,1467,1718,148
            }


            //Fetch the Month Name which is selected in the dropdown
            string strActualStartDate = string.Empty;
            string strActualEndDate = string.Empty;
            List<SelectListModal> SelectedMonth = Query.Where(x => x.MasterName.ToUpper() == "YEARMASTER" && x.Value == Convert.ToString(modal.MonthId)).ToList();
            

            DateTime dtstartDate = DateTime.ParseExact(SelectedMonth[0].s_Value1, "MM/dd/yyyy",CultureInfo.InvariantCulture);
            DateTime dtEndDate = DateTime.ParseExact(SelectedMonth[0].s_Value2, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            string stdate = dtstartDate.ToString("dd");
            string stmonth = dtstartDate.ToString("MMM");
            string styear =  dtstartDate.ToString("yy"); // DateTime.Now.ToString("yy"); 

            strActualStartDate = " "+ stdate + "-" + stmonth + "-" + styear;

 

            string enddate = dtEndDate.ToString("dd");
            string endmonth = dtEndDate.ToString("MMM");
            string endyear =  dtEndDate.ToString("yy"); //  DateTime.Now.ToString("yy");

            strActualEndDate = " " +enddate + "-" + endmonth + "-" + endyear;

            //  var ProjectList =  result.GroupBy(x => x.ProjectId).OrderByDescending(x => x.Max(y => y.ProjectId)).ToList();
            // Group by Project id and then order by Project Name ascending
            var ProjectList = result.GroupBy(x => x.ProjectId).OrderByDescending(x => x.Max(y => y.ProjectName)).Reverse().ToList();
                 

            if (ProjectList.Count > 0)
            {
                using (XLWorkbook wb = new XLWorkbook())
                {

                 #region ProjectWise Details Worksheets creation | Starts

                    //Looping Each Project from Projects List | Starts here

                    for (int i = 0; i < ProjectList.Count; i++)
                    {

                        string offshoreOnshoreFlag = string.Empty;

                        string worksheetName = string.Empty;
                        string worksheetName_Onsite = string.Empty;

                        decimal ProjectTotal = 0;

                        decimal OnsiteProjectTotal = 0;
                     

                        Int16 ProjId = Convert.ToInt16(ProjectList[i].Key);
                       
                        //Fetch only Billable timesheet records under the single Project
                        List<TimesheetSearchResultModal> details = result.Where(x => x.ProjectId == ProjId && x.Billable == "Yes").ToList();

                        if (details.Count > 0) // Proceed if there are Billable data
                        {
                            string ProjectName = details[0].ProjectName.ToString();
                            string OnsiteManagerName = details[0].OnsiteManagerName != null ? details[0].OnsiteManagerName.ToString() : "" ;
                            string ProjectType = details[0].ProjectType != null ? details[0].ProjectType.ToString() : "";

                            //Fetch distinct resource Locations for the Project to separate the worksheets as Invoice for Offshore and Onsite
                            var Locations = details.Where(t => t.Location != null).Select(l => l.Location).Distinct().ToList();


                            if (Locations.Count() == 1)
                                offshoreOnshoreFlag = Locations[0].ToLower() != "india" ? " Onsite" : "";


                            // var CRList = details.GroupBy(x => x.CrNumberId).ToList(); 
                            var CRList = details.GroupBy(x => x.CrNumberId).OrderByDescending(x => x.Max(y => y.CrNumberId)).ToList();

                            DataSet dsProjectDetails = new DataSet();
                            DataSet dsOnsiteCRDetails = new DataSet();

                            for (int j = 0; j < CRList.Count; j++)
                            {
                                string isOLSCR = string.Empty;

                                DataTable dtCRDetails = new DataTable();
                                List<TimesheetSearchResultModal> CRdetails = new List<TimesheetSearchResultModal>();

                                Int16 CrId = Convert.ToInt16(CRList[j].Key);



                                if (CRList[j].Key.HasValue && CrId != 0)
                                    isOLSCR = Query.Where(cr => cr.MasterName.ToUpper() == "CRNUMBER" && cr.Value == CrId.ToString()).Select(x => x.s_Value4).FirstOrDefault();
                                else
                                    isOLSCR = "false";

                                CRdetails = result.Where(x => x.ProjectId == ProjId &&
                                                               x.CrNumberId == (!CRList[j].Key.HasValue ? (int?)(null) : CrId) &&
                                                               x.Billable == "Yes").ToList();

                                dtCRDetails = ToDataTable<TimesheetSearchResultModal>(CRdetails);

                                

                                //Join to fetch Resource Role and PerDayCost
                                var JoinResult = (from cr in dtCRDetails.AsEnumerable()
                                                  join rm in dtResourceMaster.AsEnumerable()
                                                   on cr.Field<int>("ResoureId").ToString() equals rm.Field<string>("Value")
                                                  select new MonthlyInvoiceModal()
                                                  {
                                                      ResourceId = cr.Field<int>("ResoureId"),
                                                      NameOfResource = cr.Field<string>("ResourceName"),
                                                      Billable = cr.Field<string>("Billable"),
                                                      Location = cr.Field<string>("Location").ToLower() == "india" ? "Offshore" : "Onsite",
                                                      Role = rm.Field<string>("s_Value5"), //s_Value5 is RBC Role  // s_Value1 is Internal Role
                                                      CRNumber = cr.Field<string>("CrNumber") +' '+ cr.Field<string>("CrLongName"),
                                                      IsOLSCR = isOLSCR,   //crm.Field<string>("s_Value4"), 
                                                      Task = cr.Field<string>("Activity"),// +" - "+ cr.Field<string>("SubActivity"),
                                                      ActualStartDate = Convert.ToDateTime(strActualStartDate), //SelectedMonth[0].s_Value1,
                                                      ActualEndDate = Convert.ToDateTime(strActualEndDate), //SelectedMonth[0].s_Value2,

                                                      WorkingDays = cr.Field<decimal?>("Efforts_Days"),

                                                      //If OLS CR then assign OLS per day Rate else, if Non-OLS CR then assign PerDayCost
                                                      PerDayCost = !string.IsNullOrEmpty(isOLSCR) && isOLSCR.ToLower() == "true" ? Convert.ToDecimal(string.IsNullOrEmpty(rm.Field<string>("s_Value4")) ? "0" : rm.Field<string>("s_Value4")) : Convert.ToDecimal(rm.Field<string>("s_Value2")),
                                                      // PerDayCost = Convert.ToDecimal(rm.Field<string>("s_Value2")),                                                  

                                                      Total = cr.Field<decimal?>("Efforts_Days") * Convert.ToDecimal(rm.Field<string>("s_Value2"))

                                                  }).ToList();


                                DataTable dtJoinResult = new DataTable();

                                dtJoinResult = ToDataTable<MonthlyInvoiceModal>(JoinResult);

                                //Group by Resources-Task
                                var groupedResult = (from row in dtJoinResult.AsEnumerable()
                                                     group row by new
                                                     {
                                                         ResourceId = row.Field<int>("ResourceId"),
                                                         NameOfResource = row.Field<string>("NameOfResource"),
                                                         Billable = row.Field<string>("Billable"),
                                                         Location = row.Field<string>("Location"),
                                                         Role = row.Field<string>("Role"),
                                                         CRNumber = row.Field<string>("CRNumber"),
                                                         IsOLSCR = row.Field<string>("IsOLSCR"),
                                                         Task = row.Field<string>("Task"),
                                                         ActualStartDate = row.Field<DateTime>("ActualStartDate"),
                                                         ActualEndDate = row.Field<DateTime>("ActualEndDate"),
                                                         PerdayCost = row.Field<decimal?>("PerDayCost"),
                                                     }
                                              into grp
                                                     select new MonthlyInvoiceModal()
                                                     {
                                                         ResourceId = grp.Key.ResourceId,
                                                         NameOfResource = grp.Key.NameOfResource,
                                                         Billable = grp.Key.Billable,
                                                         Location = grp.Key.Location,
                                                         Role = grp.Key.Role,
                                                         CRNumber = grp.Key.CRNumber,
                                                         IsOLSCR = grp.Key.IsOLSCR,
                                                         Task = grp.Key.Task,
                                                         ActualStartDate = Convert.ToDateTime(grp.Key.ActualStartDate),
                                                         ActualEndDate = Convert.ToDateTime(grp.Key.ActualEndDate),

                                                         WorkingDays = grp.Sum(r => r.Field<Decimal?>("WorkingDays")),
                                                         PerDayCost = grp.Key.PerdayCost,
                                                         Total = grp.Sum(r => r.Field<Decimal?>("WorkingDays")) * Convert.ToDecimal(grp.Key.PerdayCost)
                                                     })
                                              .OrderBy(o => o.NameOfResource).ToList();



                                DataTable dtGroupedResult = ToDataTable<MonthlyInvoiceModal>(groupedResult);

                                // Join the Tasks with comma (,) if there are repetition of Resource name , add the Working Days and Total Cost

                                var finalResult = dtGroupedResult.AsEnumerable()
                                            .GroupBy(row => new
                                            {
                                                ResourceId = row.Field<int>("ResourceId"),
                                                NameOfResource = row.Field<string>("NameOfResource"),
                                                Billable = row.Field<string>("Billable"),
                                                Location = row.Field<string>("Location"),
                                                Role = row.Field<string>("Role"),
                                                CRNumber = row.Field<string>("CRNumber"),
                                                IsOLSCR = row.Field<string>("IsOLSCR"),
                                            //Task = row.Field<string>("Task"),
                                                ActualStartDate = row.Field<DateTime>("ActualStartDate"),
                                                ActualEndDate = row.Field<DateTime>("ActualEndDate"),
                                                PerdayCost = row.Field<decimal?>("PerDayCost"),
                                            })
                                             .Select(grp => new MonthlyInvoiceModal()
                                             {

                                                 ResourceId = grp.Key.ResourceId,
                                                 NameOfResource = grp.Key.NameOfResource,
                                                 Billable = grp.Key.Billable,
                                                 Location = grp.Key.Location,
                                                 Role = grp.Key.Role,
                                                 CRNumber = grp.Key.CRNumber,
                                                 IsOLSCR = grp.Key.IsOLSCR,
                                                 Task = String.Join(",", grp.Select(z => z.Field<string>("Task"))), //Comma separted Tasks
                                                 ActualStartDate = Convert.ToDateTime(grp.Key.ActualStartDate),
                                                 ActualEndDate = Convert.ToDateTime(grp.Key.ActualEndDate),
                                                 WorkingDays = Math.Round(Convert.ToDecimal(grp.Sum(r => r.Field<Decimal?>("WorkingDays"))),2, MidpointRounding.AwayFromZero) ,//grp.Sum(r => r.Field<Decimal?>("WorkingDays")),  
                                                 PerDayCost =  Math.Round(Convert.ToDecimal(grp.Key.PerdayCost), 2, MidpointRounding.AwayFromZero), //grp.Key.PerdayCost,
                                                 //Total = Math.Round(Convert.ToDecimal(grp.Sum(r => r.Field<Decimal?>("WorkingDays")) * Convert.ToDecimal(grp.Key.PerdayCost)), 2, MidpointRounding.AwayFromZero)  // grp.Sum(r => r.Field<Decimal?>("WorkingDays")) * Convert.ToDecimal(grp.Key.PerdayCost)

                                                 Total= Math.Round(Convert.ToDecimal(grp.Sum(r => r.Field<Decimal?>("WorkingDays"))), 2, MidpointRounding.AwayFromZero) * Convert.ToDecimal(grp.Key.PerdayCost)


                                             }).ToList();


                                DataTable dtFinalResult = ToDataTable<MonthlyInvoiceModal>(finalResult);

                                dtFinalResult.TableName = "Table_" + j;

                                //dtFinalResult.Columns.Remove("ResourceId");
                                dtFinalResult.Columns.Remove("Billable");
                                dtFinalResult.Columns.Remove("IsOLSCR");


                                //Rename the columns as equired in the Excel

                                dtFinalResult.Columns["NameOfResource"].ColumnName = "Name of Resources";
                                dtFinalResult.Columns["CRNumber"].ColumnName = "CR/IM Number";
                                dtFinalResult.Columns["ActualStartDate"].ColumnName = "Actual Start Date";
                                dtFinalResult.Columns["ActualEndDate"].ColumnName = "Actual End Date";
                                dtFinalResult.Columns["WorkingDays"].ColumnName = "Working Days";
                                dtFinalResult.Columns["PerDayCost"].ColumnName = "Per Day Cost";
                                //rest of the columns are ok. no need to rename



                                
                                DataTable dtOnsiteRecords = new DataTable();

                                dtOnsiteRecords = dtFinalResult.Clone(); // Copy the schema

                                //Filter out the Onsite records when there are both onsite and offshore records in the worksheet
                                //Add those Onsite Timesheet records to another worksheet and name that worksheet - ProjectName append with "Onsite"

                                if (Locations.Count > 1) // Onsite and Offshore
                                {

                                    //If CRdatatable has Onsite records then copy them to OnsiteDataTable and then remove from FinalResultDataTable

                                    if (dtFinalResult.AsEnumerable().Where(r => r.Field<string>("Location") == "Onsite").ToList().Count > 0)
                                    {
                                        //Copy the onsite rows to Onsite Datatable
                                        dtFinalResult.AsEnumerable().Where(r => r.Field<string>("Location") == "Onsite").ToList().ForEach(row => dtOnsiteRecords.ImportRow(row));

                                        //Remove the onsite rows from FinalResultDataTable where both Onsite and Offshore rows were present
                                        dtFinalResult.Rows.Cast<DataRow>().Where(r => r.Field<string>("Location") == "Onsite").ToList().ForEach(r => r.Delete());


                                        dtOnsiteRecords.AcceptChanges();
                                    }
                                }

                                
                                // If it is a Onsite Support project and Support Team Members are there in the Worksheet, remove their records
                                //And add them in Fixed Billing Worksheet. dtOnsiteRecords will have only TnM resources in this case

                                
                               // if ( (ProjectName.Contains("support") || ProjectName.Contains("Support")) )
                                if(!string.IsNullOrEmpty(ProjectType) && ProjectType.ToLower() == "support")
                                {
                                    drSuppProj = dtSupportProjects.NewRow(); //Creating New Row
                                    drSuppProj["ProjectID"] = ProjId;
                                    drSuppProj["ProjectName"] = ProjectName;
                                    dtSupportProjects.Rows.Add(drSuppProj); //Adding Row

                                    

                                    if (dtFinalResult.Rows.Count > 0 && dtFinalResult.Rows[0]["Location"].ToString().ToLower() == "onsite")
                                    {
                                        
                                        // DataRow[] drows = dtFinalResult.Select("ResourceId IN (1453,1865,1454,1458,1550,144,1467,1718,1601,148)"); //Onsite Support Team
                                        DataRow[] drows = dtFinalResult.Select("ResourceId IN (" + supportTeamIds + ")"); //select support team rows present in datatable                      

                                        
                                        if (drows!=null && drows.Count() > 0)
                                        {
                                            
                                            //Loop the rows , add to FixedCost datatable and remove from the current datatable
                                            foreach(DataRow dr in drows)
                                            {
                                                drFixedBill = dtFixedCostBilling.NewRow(); //  Creating Row

                                                drFixedBill["ResourceId"] = dr["ResourceId"];
                                                drFixedBill["ResourceName"] = dr["Name of Resources"];
                                                drFixedBill["ProjectID"] = ProjId;
                                                drFixedBill["ProjectName"] = ProjectName;
                                                drFixedBill["WorkingDays"] = dr["Working Days"];

                                                dtFixedCostBilling.Rows.Add(drFixedBill); //Adding Row to FixedCostBilling DataTable

                                                //Remove the row from dtFinalResult, means dont show the record in details sheet

                                                dtFinalResult.Rows.Cast<DataRow>().Where(r => Convert.ToString(r.Field<int>("ResourceId")) == dr["ResourceId"].ToString()).ToList().ForEach(r => r.Delete());
                                                dtFinalResult.AcceptChanges();
                                            }
                                        }

                                       // dtFinalResult.AcceptChanges();
                                    }

                                    if (dtOnsiteRecords.Rows.Count > 0)
                                    {
                                        
                                        // DataRow[] drows = dtOnsiteRecords.Select("ResourceId IN (1453,1865,1454,1458,1550,144,1467,1718,1601,148)"); //Onsite Support Team
                                        DataRow[] drows = dtOnsiteRecords.Select("ResourceId IN (" + supportTeamIds + ")"); //select support team rows present in datatable                      

                                        if (drows != null && drows.Count() > 0)
                                        {
                                           
                                            //Loop the rows , add to FixedCost datatable and remove from the current datatable

                                            foreach (DataRow dr in drows)
                                            {
                                                drFixedBill = dtFixedCostBilling.NewRow(); //  Creating Row

                                                drFixedBill["ResourceId"] = dr["ResourceId"];
                                                drFixedBill["ResourceName"] = dr["Name of Resources"];
                                                drFixedBill["ProjectID"] = ProjId;
                                                drFixedBill["ProjectName"] = ProjectName;
                                                drFixedBill["WorkingDays"] = dr["Working Days"];

                                                dtFixedCostBilling.Rows.Add(drFixedBill); //Adding Row to FixedCostBilling DataTable

                                                //Remove the row from dtFinalResult, means dont show the record in details sheet

                                                dtOnsiteRecords.Rows.Cast<DataRow>().Where(r => Convert.ToString(r.Field<int>("ResourceId")) == dr["ResourceId"].ToString()).ToList().ForEach(r => r.Delete());
                                                dtOnsiteRecords.AcceptChanges();
                                            }
                                        }

                                       // dtOnsiteRecords.AcceptChanges();
                                    }
                                } // End of Support project If statement


                                dtFinalResult.Columns.Remove("ResourceId");
                                dtOnsiteRecords.Columns.Remove("ResourceId");



                                //dtFinalResult can have either Offshore Or Onsite records . means All timesheet records are from Offshore or from Onsite
                                if (dtFinalResult.Rows.Count > 0) 
                                                                 
                                    dsProjectDetails.Tables.Add(dtFinalResult);

                                //dtOnsiteRecords will have Only Onsite records which are filtered from dtFinalResult, when there are both Offshore and Onsite records
                                if (dtOnsiteRecords.Rows.Count > 0)                              

                                    dsOnsiteCRDetails.Tables.Add(dtOnsiteRecords);                                
                               


                            } // End of CRList looping

                                                                                   


                            //Binding All the datatables which are grouped by CR ID will be inserted to worksheet
                            //This dsProjectDetails will have EITHER Onsite Records OR Offshore Records
                            if (dsProjectDetails.Tables.Count > 0)
                            {
                                drSummary = dtSummary.NewRow(); //Creating New Row for Summary Table
                                SlNo = SlNo + 1;
                               // drSummary["Sr. No."] = SlNo;
                                drSummary["Location"] = offshoreOnshoreFlag == " Onsite" ? "Onsite" : "Offshore";
                                drSummary["Project"] = ProjectName;
                                drSummary["Invoice Manager"] = OnsiteManagerName;


                                // name the worksheet as the first 6 digits from the Project Name and prefixed with an underscore as it was throwing error
                                if (ProjectName.ToLower() != "other")
                                    worksheetName = "_" + ProjectName.Substring(0, 7) + offshoreOnshoreFlag;
                                else
                                    worksheetName = ProjectName + offshoreOnshoreFlag;

                               
                                var ws = wb.Worksheets.Add(worksheetName); // Name of the  worksheet / Tab               
                                ws.Range(1, 1, 1, 10).Value = ProjectName + " " + "Invoice for the month of " + SelectedMonth[0].Text + " (Project Manager: " + OnsiteManagerName + ")"; //Name of the Heading
                                ws.Range(1, 1, 1, 10).Merge().AddToNamed("Titles");

                              

                                int rowcounter = 4;
                                string CRName = string.Empty;

                                for (int k = 0; k < dsProjectDetails.Tables.Count; k++)
                                {
                                    if (dsProjectDetails.Tables[k].Rows.Count > 0)
                                    {
                                       

                                        CRName = dsProjectDetails.Tables[k].Rows[0]["CR/IM Number"].ToString();

                                        // ws.Cell(rowcounter, 1).InsertTable(dsProjectDetails.Tables[k]);
                                        ws.Cell(rowcounter, 1).InsertData(dsProjectDetails.Tables[k]);

                                                                   

                                        // Create Headers  and apply styling
                                        string[] columnNames = (from dc in dsProjectDetails.Tables[0].Columns.Cast<DataColumn>()
                                                                select dc.ColumnName).ToArray();


                                        for (int c = 0; c < columnNames.Length; c++)
                                        {
                                            var columnNumber = c + 1;
                                            ws.Cell(3, columnNumber).Value = columnNames[c];                                          
                                            ws.Cell(3, columnNumber).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#bfbfc2")); // Column Header Gray Color
                                            ws.Cell(3, columnNumber).Style.Font.Bold = true;
                                            ws.Cell(3, columnNumber).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            ws.Cell(3, columnNumber).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                                            if (columnNumber == 6)                                            
                                                ws.Column(6).Style.DateFormat.Format = "dd-mmm-yy";
                                            if (columnNumber == 7)
                                                ws.Column(7).Style.DateFormat.Format = "dd-mmm-yy";
                                          

                                            if (columnNumber == 8)
                                                ws.Column(8).Style.NumberFormat.Format = "0.00";
                                            if (columnNumber == 9)
                                                ws.Column(9).Style.NumberFormat.Format = "\u00A30.00"; //PoundSymbol 0.00
                                            if (columnNumber == 10)
                                                ws.Column(10).Style.NumberFormat.Format = "\u00A30.00"; //PoundSymbol 0.00
                                        }



                                        //Decimal TotalCRCost = Convert.ToDecimal(dsProjectDetails.Tables[k].Compute("SUM([Total Cost])", string.Empty));
                                        Decimal TotalCRCost = Convert.ToDecimal(dsProjectDetails.Tables[k].Compute("SUM([Total])", string.Empty));

                                        TotalCRCost = Math.Round(TotalCRCost, 2, MidpointRounding.AwayFromZero);
                                        ProjectTotal = Math.Round(TotalCRCost + ProjectTotal ,2, MidpointRounding.AwayFromZero) ; //Adding TotalCRCost to find TotalProjectCost


                                        //Show Total CR Cost in Orange Color Row

                                        //  ws.Range(rowcounter + (dsProjectDetails.Tables[k].Rows.Count + 1), 1, rowcounter + (dsProjectDetails.Tables[k].Rows.Count + 1), 1).Value = "Total " + CRName + " Cost is : \u00A3" + TotalCRCost;
                                        ws.Range(rowcounter + (dsProjectDetails.Tables[k].Rows.Count), 1, rowcounter + (dsProjectDetails.Tables[k].Rows.Count), 1).Value = "Total " + CRName; //+ " Cost is : \u00A3" + TotalCRCost;
                                        ws.Range(rowcounter + (dsProjectDetails.Tables[k].Rows.Count), 1, rowcounter + (dsProjectDetails.Tables[k].Rows.Count), 9).Merge().AddToNamed("TotalCostStyle");

                                        ws.Cell(rowcounter + (dsProjectDetails.Tables[k].Rows.Count), 10).Value = "\u00A3" + TotalCRCost;
                                        ws.Cell(rowcounter + (dsProjectDetails.Tables[k].Rows.Count), 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                        ws.Cell(rowcounter + (dsProjectDetails.Tables[k].Rows.Count), 10).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#ffc000")); //(XLColor.Orange);


                                        if(k == (dsProjectDetails.Tables.Count -1)) //Show project Total Cost after Last Table
                                        {
                                            if (offshoreOnshoreFlag == " Onsite")
                                                ws.Cell(rowcounter + (dsProjectDetails.Tables[k].Rows.Count) + 3, 9).Value = "Onsite Total"; 
                                            else
                                                ws.Cell(rowcounter + (dsProjectDetails.Tables[k].Rows.Count) + 3, 9).Value = "Offshore Total"; 


                                            ws.Cell(rowcounter + (dsProjectDetails.Tables[k].Rows.Count) + 3, 10).Value = "\u00A3" + ProjectTotal;

                                            ws.Cell(rowcounter + (dsProjectDetails.Tables[k].Rows.Count) + 3, 9).Style.Font.Bold = true;
                                            ws.Cell(rowcounter + (dsProjectDetails.Tables[k].Rows.Count) + 3, 10).Style.Font.Bold = true;
                                            ws.Cell(rowcounter + (dsProjectDetails.Tables[k].Rows.Count) + 3, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                                            //Assign Border to the Content
                                            ws.Range(1, 1, rowcounter + (dsProjectDetails.Tables[k].Rows.Count) + 3, 10).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                                            ws.Range(rowcounter + (dsProjectDetails.Tables[k].Rows.Count) + 3,1, rowcounter + (dsProjectDetails.Tables[k].Rows.Count) + 3,10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                        }
                                           


                                        //setting the rowcounter where the next datatable will be inserted
                                        rowcounter = rowcounter + dsProjectDetails.Tables[k].Rows.Count + 1;
                                    }
                                }

                                drSummary["Total"] =  ProjectTotal; 
                                dtSummary.Rows.Add(drSummary); //Adding Row to dtSummary

                                ws.Row(3).AdjustToContents(30.00, 30.00); // To give height to the Headers
                                ws.Columns().AdjustToContents();
                            }

                            //Binding All the datatables which have Onsite records ONLY and are grouped by CR ID will be inserted to Onsite worksheet
                            //This dsOnsiteCRDetails will have ONLY Onsite records. This was created when The Project has mixed Type records Offshore and Onsite both, 
                            //Then remove the Onsite records and assign them to this dsOnsiteCRDetails and create another worksheet simultaneously for Onsite

                            if (dsOnsiteCRDetails.Tables.Count > 0)
                            {
                                drSummary = dtSummary.NewRow(); //Creating New Row for Summary Table
                                SlNo = SlNo + 1;
                              //  drSummary["Sr. No."] = SlNo;
                                drSummary["Location"] = "Onsite";
                                drSummary["Project"] = ProjectName;
                                drSummary["Invoice Manager"] = OnsiteManagerName;

                                // name the worksheet as the first 6 digits from the Project Name and prefixed with an underscore as it was throwing error
                                if (ProjectName.ToLower() != "other")
                                    worksheetName_Onsite = "_" + ProjectName.Substring(0, 7) + " Onsite";
                                else
                                    worksheetName_Onsite = ProjectName + " Onsite";

                                
                                var ws = wb.Worksheets.Add(worksheetName_Onsite); // Name of the  worksheet / Tab               
                                ws.Range(1, 1, 1, 10).Value = ProjectName + " " + "Invoice for the month of " + SelectedMonth[0].Text + " (Project Manager: " + OnsiteManagerName + ")"; //Name of the Heading
                                ws.Range(1, 1, 1, 10).Merge().AddToNamed("Titles");




                                int rowcounter = 4;
                                string CRName = string.Empty;

                                for (int k = 0; k < dsOnsiteCRDetails.Tables.Count; k++)
                                {
                                    if (dsOnsiteCRDetails.Tables[k].Rows.Count > 0)
                                    {
                                       

                                        CRName = dsOnsiteCRDetails.Tables[k].Rows[0]["CR/IM Number"].ToString();

                                       // ws.Cell(rowcounter, 1).InsertTable(dsOnsiteCRDetails.Tables[k]);
                                        ws.Cell(rowcounter, 1).InsertData(dsOnsiteCRDetails.Tables[k]);
                                       

                                        // Create Headers  and apply styling
                                        string[] columnNames = (from dc in dsOnsiteCRDetails.Tables[0].Columns.Cast<DataColumn>()
                                                                select dc.ColumnName).ToArray();


                                        for (int c = 0; c < columnNames.Length; c++)
                                        {
                                            var columnNumber = c + 1;
                                            ws.Cell(3, columnNumber).Value = columnNames[c];                                            
                                            ws.Cell(3, columnNumber).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#bfbfc2")); // Column Header Gray Color
                                            ws.Cell(3, columnNumber).Style.Font.Bold=true;
                                            ws.Cell(3, columnNumber).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                            ws.Cell(3, columnNumber).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                                            if (columnNumber == 6)                                            
                                                ws.Column(6).Style.DateFormat.Format = "dd-mmm-yy";
                                            if (columnNumber == 7)
                                                ws.Column(7).Style.DateFormat.Format = "dd-mmm-yy";
                                      
                                            if (columnNumber == 8)
                                                ws.Column(8).Style.NumberFormat.Format = "0.00";
                                            if (columnNumber == 9)
                                                ws.Column(9).Style.NumberFormat.Format = "\u00A30.00"; //PoundSymbol 0.00
                                            if (columnNumber == 10)
                                                ws.Column(10).Style.NumberFormat.Format = "\u00A30.00"; //PoundSymbol 0.00

                                        }
                                        

                                        Decimal TotalCRCost = Convert.ToDecimal(dsOnsiteCRDetails.Tables[k].Compute("SUM([Total])", string.Empty));
                                        TotalCRCost = Math.Round(TotalCRCost, 2, MidpointRounding.AwayFromZero);

                                        OnsiteProjectTotal = Math.Round(TotalCRCost + OnsiteProjectTotal, 2, MidpointRounding.AwayFromZero); // Adding TotalCRCost to find OnsiteProjectTotal

                            
                                        //Show Total CR cost in Orange Color Row
                                        ws.Range(rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count), 1, rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count), 1).Value = "Total " + CRName; // + " Cost is : \u00A3" + TotalCRCost;
                                        ws.Range(rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count), 1, rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count), 9).Merge().AddToNamed("TotalCostStyle");

                                        ws.Cell(rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count), 10).Value = "\u00A3" + TotalCRCost;                                        
                                        ws.Cell(rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count), 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                        ws.Cell(rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count), 10).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#ffc000"));  //(XLColor.Orange);


                                        if (k == (dsOnsiteCRDetails.Tables.Count - 1))  //Show project Total Cost after Last Table
                                        {
                                            ws.Cell(rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count) + 3, 9).Value = "Onsite Total" ;
                                            ws.Cell(rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count) + 3, 10).Value = "\u00A3" + OnsiteProjectTotal;
                                            ws.Cell(rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count) + 3, 9).Style.Font.Bold = true;
                                            ws.Cell(rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count) + 3, 10).Style.Font.Bold = true;
                                            ws.Cell(rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count) + 3, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;


                                            //Assign Border to the Content
                                            ws.Range(1, 1, rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count) + 3, 10).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                                            ws.Range(rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count) + 3, 1, rowcounter + (dsOnsiteCRDetails.Tables[k].Rows.Count) + 3, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                        }

                                        //setting the rowcounter where the next datatable will be inserted
                                        rowcounter = rowcounter + dsOnsiteCRDetails.Tables[k].Rows.Count + 1;
                                    }
                                }

                                drSummary["Total"] =  OnsiteProjectTotal; 
                                dtSummary.Rows.Add(drSummary); //Adding Row to dtSummary

                                ws.Row(3).AdjustToContents(30.00, 30.00);  // To give height to the Headers
                                ws.Columns().AdjustToContents();

                            } // End of dsOnsite


                            // ws.Columns().AdjustToContents();
                           // wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            // wb.Style.Font.Bold = true;
                            wb.Style.Font.FontName = "Arial";
                            wb.Style.Font.FontSize = 10;

                        }

                    }  // ProjectList Loop ends here

                    //ProjectWise Details Worksheets creation | Ends
                    #endregion


                    #region Creating Summary WorkSheet | Starts

                  
                    if (dtSummary.Rows.Count > 0)
                    { 
                        int RowCount = dtSummary.Rows.Count;

                        //Adding last 2 rows in Summary Table which are always static
                        for (int i = 0; i < 2; i++)
                        {
                            drSummary = dtSummary.NewRow(); //Creating New Row for Summary Table

                            SlNo = i == 0 ? RowCount + 1 : RowCount + 2;
                            //drSummary["Sr. No."] = SlNo;
                            drSummary["Location"] = "Onsite";
                            drSummary["Project"] = i == 0 ? "Fixed billing onsite support" : "Fixed billing onsite support (Credit Note)";                            
                            drSummary["Total"] = i == 0 ? "74500" : "-30000"; // 74500Pound -30000 Pound
                            drSummary["Invoice Manager"] = "Damon O";
                            dtSummary.Rows.Add(drSummary); //Adding Row
                        }

                        dtSummary.DefaultView.Sort = "[Location] ASC";                        
                        dtSummary.AcceptChanges();                     

                        dtSummary = dtSummary.DefaultView.ToTable();

                        dtSummary.Columns.Add("Sr. No.", typeof(int)).SetOrdinal(0);

                        for (int i=0; i < dtSummary.Rows.Count;i++ )
                        {
                            dtSummary.Rows[i][0] = i + 1;
                        }

                        

                        summaryWorksheetName = "Summary Sheet";
                        var ws = wb.Worksheets.Add(summaryWorksheetName, 0); // Name of the  worksheet / Tab               
                        ws.Range(1, 1, 1, 6).Value = "Invoice for the month of " + SelectedMonth[0].Text; //Name of the Heading
                        ws.Range(1, 1, 1, 6).Merge().AddToNamed("SummarySheetTitle");

                        // Create Column Name Headers  and apply styling in Summary Table
                        string[] columnNamesInSummary = (from dc in dtSummary.Columns.Cast<DataColumn>()
                                                         select dc.ColumnName).ToArray();

                        for (int c = 0; c < columnNamesInSummary.Length; c++)
                        {
                            var columnNumber = c + 1;
                            ws.Cell(3, columnNumber).Value = columnNamesInSummary[c];
                            ws.Cell(3, columnNumber).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#bdd7ee")); //Light sky Color
                            ws.Cell(3, columnNumber).Style.Font.Bold = true;
                            ws.Cell(3, columnNumber).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(3, columnNumber).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                            //Formating the Total (4th) Column
                            if (columnNumber == 4)
                                ws.Column(4).Style.NumberFormat.Format = "\u00A30.00"; //PoundSymbol 0.00
                        }

                    

                        ws.Cell(4, 1).InsertData(dtSummary);


                        Decimal TotalCost = Convert.ToDecimal(dtSummary.Compute("SUM([Total])", string.Empty));

                        Decimal OffshoreTotalCost = Convert.ToDecimal(dtSummary.Compute("SUM([Total])", "[Location] = 'Offshore'"));
                        Decimal OnsiteTotalCost = Convert.ToDecimal(dtSummary.Compute("SUM([Total])", "[Location] = 'Onsite'"));

                        //Last Row where Total is shown in Summary Table

                        ws.Cell (4 + (dtSummary.Rows.Count), 3).Value = "Total";
                        ws.Cell (4 + (dtSummary.Rows.Count), 4).Value = TotalCost;  
                        ws.Cell (4 + (dtSummary.Rows.Count), 3).Style.Font.Bold = true;
                        ws.Cell(4 + (dtSummary.Rows.Count), 3).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#bdd7ee"));
                        ws.Cell (4 + (dtSummary.Rows.Count), 4).Style.Font.Bold = true;
                        ws.Cell(4 + (dtSummary.Rows.Count), 4).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#bdd7ee"));

                        ws.Range(4 + (dtSummary.Rows.Count), 1, 4 + (dtSummary.Rows.Count), 2).Merge().Style.Fill.SetBackgroundColor(XLColor.FromHtml("#bdd7ee"));  //Light sky Color
                        ws.Range(4 + (dtSummary.Rows.Count), 5, 4 + (dtSummary.Rows.Count), 6).Merge().Style.Fill.SetBackgroundColor(XLColor.FromHtml("#bdd7ee"));  //Light sky Color


                        //Assign Border to the Content of Summary Table
                        ws.Range(3, 1, dtSummary.Rows.Count+4, 6).Style.Border.InsideBorder = XLBorderStyleValues.Thin;                        
                        ws.Range(3, 1, dtSummary.Rows.Count + 4, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        //Creating Pivot DataTable

                        DataTable dtPivot = new DataTable();
                        dtPivot.Columns.Add("Location");
                        dtPivot.Columns.Add("Sum of Total");

                        DataRow drPivot;

                        drPivot = dtPivot.NewRow();
                        drPivot["Location"] = "Offshore";
                        drPivot["Sum of Total"] = "\u00A3" + OffshoreTotalCost;
                        dtPivot.Rows.Add(drPivot);

                        drPivot = dtPivot.NewRow();
                        drPivot["Location"] = "Onsite";
                        drPivot["Sum of Total"] = "\u00A3" + OnsiteTotalCost;
                        dtPivot.Rows.Add(drPivot);

                        drPivot = dtPivot.NewRow();
                        drPivot["Location"] = "Grand Total";
                        drPivot["Sum of Total"] = "\u00A3" + TotalCost;
                        dtPivot.Rows.Add(drPivot);

                        // Create Column Name Headers  and apply styling In Pivot Table
                        string[] columnNamesInPivot = (from dc in dtPivot.Columns.Cast<DataColumn>()
                                                         select dc.ColumnName).ToArray();

                        for (int c = 0; c < columnNamesInPivot.Length; c++)
                        {                           
                            ws.Cell(3, c + 8).Value = columnNamesInPivot[c];
                            ws.Range(3, 8, 3, 9).AddToNamed("ColumnNameStyleInSummary");
                        }

                        ws.Cell(4, 8).InsertData(dtPivot); //Inserting Pivot Table in Right side of Summary Sheet

                        ws.Range(6, 8, 6, 9).Style.Font.Bold = true; //Grand Total in Bold


                        //Apply Border In Pivot Table

                        ws.Range(3, 8, 6, 9).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        ws.Range(3, 8, 6, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                 
                        ws.Columns().AdjustToContents();
                    }


                    //Creating Summary WorkSheet | Ends
                    #endregion


                    #region Creating Fixed Billing WorkSheet | Starts

                    if (dtSupportProjects.Rows.Count > 0)
                    {

                        //Get Distinct Support Projects from datatable                    
                        dtSupportProjects = dtSupportProjects.DefaultView.ToTable(true);

                      

                        dtFixedCostBilling.DefaultView.Sort = "[ResourceName] ASC";
                        dtFixedCostBilling.AcceptChanges();
                        dtFixedCostBilling = dtFixedCostBilling.DefaultView.ToTable();


                        if (dtFixedCostBilling.Rows.Count > 0)
                        {
                            dtFixedCostBilling = dtFixedCostBilling.AsEnumerable()
                            .GroupBy(r => new
                            {
                                ResourceId = r["ResourceId"],
                                ResourceName = r["ResourceName"],
                                ProjectID = r["ProjectID"],
                                ProjectName = r["ProjectName"]
                            })
                            .Select(g =>
                             {
                                 var row = dtFixedCostBilling.NewRow();

                                 row["ResourceId"] = g.Key.ResourceId;
                                 row["ResourceName"] = g.Key.ResourceName;
                                 row["ProjectID"] = g.Key.ProjectID;
                                 row["ProjectName"] = g.Key.ProjectName;
                                 row["WorkingDays"] = g.Sum(r => r.Field<decimal?>("WorkingDays"));

                                 return row;

                             }).CopyToDataTable();
                        }


                        fixedBillingWorksheetName = "Fixed Billing Onsite supp1.";
                        var wsFB = wb.Worksheets.Add(fixedBillingWorksheetName, 1); // Name of the  worksheet / Tab     

                        //Add first 2 static columns
                        wsFB.Cell(1, 1).Value = "Emp.ID";
                        wsFB.Cell(1, 2).Value = "Team Member";

                        //Create Columns Dynamically . Bind All Support Projects

                        int colNumber = 0;
                        for (int c = 0; c < dtSupportProjects.Rows.Count; c++)
                        {
                            colNumber = c + 3;
                            wsFB.Cell(1, colNumber).Value = dtSupportProjects.Rows[c]["ProjectName"];

                        }
                        //Add last 3 static columns
                        wsFB.Cell(1, (colNumber + 1)).Value = "Total Chargeable Days";
                        wsFB.Cell(1, (colNumber + 2)).Value = "Day Rate(£)";
                        wsFB.Cell(1, (colNumber + 3)).Value = "Total Revenue";



                        //Applying Column Header style in Fixed Billing WorkSheet

                        wsFB.Range(1, 1, 1, (dtSupportProjects.Rows.Count + 5)).AddToNamed("ColumnNameStyleInFB");


                        // Add Support Team names in the Rows dynamically
                        for (int i = 0; i < dtSupportTeam.Rows.Count; i++)
                        {
                            wsFB.Cell(i + 2, 1).Value = dtSupportTeam.Rows[i][0].ToString(); //ResoutrceID
                            wsFB.Cell(i + 2, 2).Value = dtSupportTeam.Rows[i][1].ToString(); //Resoutrce

                            decimal TotalChargeableDays = 0;

                            for (int c = 0; c < dtSupportProjects.Rows.Count; c++)
                            {
                                //Fetch the TotalWorking Days for the Team Member and for the Support project from  dtFixedCostBilling
                                decimal Workingdays_FB = 0;
                                Workingdays_FB = Convert.ToDecimal(dtFixedCostBilling.AsEnumerable().
                                                 Where(s => s.Field<string>("ProjectID") == dtSupportProjects.Rows[c]["ProjectID"].ToString()
                                                 && s.Field<string>("ResourceId") == dtSupportTeam.Rows[i][0].ToString())
                                                .Select(s => s.Field<decimal>("WorkingDays")).FirstOrDefault());


                                if (Workingdays_FB > 0)
                                {
                                    wsFB.Cell((i + 2), (c + 3)).Value = Workingdays_FB;
                                    wsFB.Cell((i + 2), (c + 3)).Style.NumberFormat.Format = "0.00"; //Upto 2 decimal place

                                    TotalChargeableDays = TotalChargeableDays + Workingdays_FB; //Horizontal Sum of Total Working Days per Resource

                                }

                            } // end of SupportProjects loop

                            //show Total Chargeable Days for the each Resource | Horizontal Sum
                            wsFB.Cell((i + 2), (dtSupportProjects.Rows.Count + 3)).Value = TotalChargeableDays;
                            wsFB.Cell((i + 2), (dtSupportProjects.Rows.Count + 3)).Style.NumberFormat.Format = "0.00"; //Upto 2 decimal place

                            //show Day rate for each resource | 450 is written here , can be modified after downloading Invoice
                            wsFB.Cell((i + 2), (dtSupportProjects.Rows.Count + 4)).Value = "450";
                            wsFB.Cell((i + 2), (dtSupportProjects.Rows.Count + 4)).Style.NumberFormat.Format = "\u00A30.00"; //Upto 2 decimal place with pound symbol

                            //show Total Revenue
                            wsFB.Cell((i + 2), (dtSupportProjects.Rows.Count + 5)).Value = TotalChargeableDays * 450;



                        } // End of Team loop

                        // Below Part -- Total Days , Fixed Fee , %Work , Overhead , Total calculation starts

                        var range = wsFB.RangeUsed();
                        var table = range.AsTable();

                        string columnLetter = string.Empty;
                        for (int c = 0; c <= dtSupportProjects.Rows.Count; c++)
                        {

                            IXLRangeColumn cell = null;


                            if (c < dtSupportProjects.Rows.Count) // Till last projects columns

                                cell = table.FindColumn(col => col.FirstCell().Value.ToString() == dtSupportProjects.Rows[c][1].ToString()); // Support Project Name

                            else

                                cell = table.FindColumn(col => col.FirstCell().Value.ToString() == "Total Chargeable Days");



                            if (cell != null)
                            {
                                columnLetter = cell.WorksheetColumn().ColumnLetter(); //A B C D E
                            }

                            if (c < dtSupportProjects.Rows.Count)
                            {
                                // Sum of Total Days Project Column wise | Vertical Sum

                                // decimal verticalSum = $"=SUM({columnLetter}{2}:{columnLetter}{dtSupportTeam.Rows.Count + 1 })";

                                wsFB.Cell((dtSupportTeam.Rows.Count + 2), (c + 3)).FormulaA1 =
                                    $"=SUM({columnLetter}{2}:{columnLetter}{dtSupportTeam.Rows.Count + 1 })";  //"=SUM(C2:C11)";


                                // Fixed Fee | Project Column wise | Vertical calculation | Total Days * Day rate

                                wsFB.Cell((dtSupportTeam.Rows.Count + 3), (c + 3)).FormulaA1 =
                                   $"= {columnLetter}{dtSupportTeam.Rows.Count + 2} * 450";

                                // %Work | Project Column wise | Vertical calculation 
                                wsFB.Cell((dtSupportTeam.Rows.Count + 4), (c + 3)).FormulaA1 =
                                    $"=IFERROR({columnLetter}{dtSupportTeam.Rows.Count + 2}/#REF!,0)";  //=IFERROR(L12/#REF!,0)

                                // Overhead | Project Column wise | Vertical calculation | Fixed Fee * 0.1
                                wsFB.Cell((dtSupportTeam.Rows.Count + 5), (c + 3)).FormulaA1 =
                                  $"= {columnLetter}{dtSupportTeam.Rows.Count + 3} * 0.1 ";

                                // Total | Project Column wise | Vertical calculation | Fixed Fee + Overhead
                                wsFB.Cell((dtSupportTeam.Rows.Count + 6), (c + 3)).FormulaA1 =
                                    $"= {columnLetter}{dtSupportTeam.Rows.Count + 3} + {columnLetter}{dtSupportTeam.Rows.Count + 5}";
                            }


                            else
                            {
                                // Sum of Total Chargeable Days | Vertical Sum

                                wsFB.Cell((dtSupportTeam.Rows.Count + 2), (dtSupportProjects.Rows.Count + 3)).FormulaA1 =
                             $"=SUM({columnLetter}{2}:{columnLetter}{dtSupportTeam.Rows.Count + 1 })";  //"=SUM(E2:E11)";

                            }


                        }

                        string lastProjectColLetter = wsFB.Cell(1, dtSupportProjects.Rows.Count + 2).WorksheetColumn().ColumnLetter();

                        //Horizontal Sum across Fixed Fee row and assign value under Total Chargeable Days column

                        wsFB.Cell((dtSupportTeam.Rows.Count + 3), (dtSupportProjects.Rows.Count + 3)).FormulaA1 =
                            $"=SUM(C{dtSupportTeam.Rows.Count + 3}:{lastProjectColLetter}{dtSupportTeam.Rows.Count + 3})"; // =SUM(C13:R13)

                        wsFB.Cell((dtSupportTeam.Rows.Count + 3), (dtSupportProjects.Rows.Count + 3)).Style.Font.Bold = true;
                        wsFB.Cell((dtSupportTeam.Rows.Count + 3), (dtSupportProjects.Rows.Count + 3)).Style.Font.FontSize = 12;




                        wsFB.Cell((dtSupportTeam.Rows.Count + 2), 2).Value = "Total Days";
                        wsFB.Cell((dtSupportTeam.Rows.Count + 2), 2).Style.Font.Bold = true;


                        wsFB.Cell((dtSupportTeam.Rows.Count + 3), 2).Value = "Fixed Fee(74500)";
                        wsFB.Cell((dtSupportTeam.Rows.Count + 3), 2).Style.Font.Bold = true;

                        wsFB.Cell((dtSupportTeam.Rows.Count + 4), 2).Value = "%Work";
                        wsFB.Cell((dtSupportTeam.Rows.Count + 4), 2).Style.Font.Bold = true;
                        wsFB.Range((dtSupportTeam.Rows.Count + 4), 3, (dtSupportTeam.Rows.Count + 4), (dtSupportProjects.Rows.Count + 2)).Style.NumberFormat.Format = "0.00%"; //Upto 2 decimal place with % symbol

                        wsFB.Cell((dtSupportTeam.Rows.Count + 5), 2).Value = "Overhead";
                        wsFB.Cell((dtSupportTeam.Rows.Count + 5), 2).Style.Font.Bold = true;
                        wsFB.Range((dtSupportTeam.Rows.Count + 5), 3, (dtSupportTeam.Rows.Count + 5), (dtSupportProjects.Rows.Count + 2)).Style.NumberFormat.Format = "\u00A30.00"; //Upto 2 decimal place with pound symbol

                        wsFB.Cell((dtSupportTeam.Rows.Count + 6), 2).Value = "Total";
                        wsFB.Cell((dtSupportTeam.Rows.Count + 6), 2).Style.Font.Bold = true;
                        wsFB.Range((dtSupportTeam.Rows.Count + 6), 3, (dtSupportTeam.Rows.Count + 6), (dtSupportProjects.Rows.Count + 2)).Style.NumberFormat.Format = "\u00A30.00"; //Upto 2 decimal place with pound symbol



                        //Apply Formating Upto 2 decimal place From column  ProjectName till Total Chargeable Days column
                        wsFB.Range((dtSupportTeam.Rows.Count + 2), 3, (dtSupportTeam.Rows.Count + 2), (dtSupportProjects.Rows.Count + 3)).Style.NumberFormat.Format = "0.00";
                        wsFB.Range((dtSupportTeam.Rows.Count + 2), 3, (dtSupportTeam.Rows.Count + 2), (dtSupportProjects.Rows.Count + 3)).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#bfbfc2")); //Gray Background


                        //Applying Background Color across Fixed Fee Row
                        wsFB.Range((dtSupportTeam.Rows.Count + 3), 2, (dtSupportTeam.Rows.Count + 3), (dtSupportProjects.Rows.Count + 3)).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#99CCFF")); //Light Blue color



                        //Apply Border

                        wsFB.Range(2, 1, dtSupportTeam.Rows.Count + 1, (dtSupportProjects.Rows.Count + 5)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        wsFB.Range(2, 1, dtSupportTeam.Rows.Count + 1, (dtSupportProjects.Rows.Count + 5)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        wsFB.Range(dtSupportTeam.Rows.Count + 2, 2, dtSupportTeam.Rows.Count + 3, (dtSupportProjects.Rows.Count + 3)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        wsFB.Range(dtSupportTeam.Rows.Count + 2, 2, dtSupportTeam.Rows.Count + 3, (dtSupportProjects.Rows.Count + 3)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        wsFB.Range(dtSupportTeam.Rows.Count + 4, 2, dtSupportTeam.Rows.Count + 6, (dtSupportProjects.Rows.Count + 2)).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        wsFB.Range(dtSupportTeam.Rows.Count + 4, 2, dtSupportTeam.Rows.Count + 6, (dtSupportProjects.Rows.Count + 2)).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        // wsFB.Columns().AdjustToContents();
                    }

                    // Creating Fixed Billing WorkSheet | Ends
                    #endregion


                    #region FixedBilling Onsite Credit Note WorkSheet | Starts

                    //The date in this Worksheet is completely static

                    fixedBillingCreditNoteWorksheetName = "FixedBilling Onsite Credit Note";
                    var wsCN = wb.Worksheets.Add(fixedBillingCreditNoteWorksheetName, 2); // Name of the  worksheet / Tab 

                    wsCN.Range(1, 1, 1, 10).Value = "FixedBilling Onsite Credit Note Invoice for the month of " + SelectedMonth[0].Text + " (Project Manager:Damon O)"; //Name of the Heading
                    wsCN.Range(1, 1, 1, 10).Merge().AddToNamed("Titles");

                    // Create Column Name Headers  and apply styling In Credit Note Table
                    string[] columnNamesInCreditNote = {"Name of Resources","Location","Role","CR/IM Number","Task", "Actual Start Date", "Actual End Date", "Working Days", "Per Day Cost", "Total" };

                    for (int c = 0; c < columnNamesInCreditNote.Length; c++)
                    {
                        wsCN.Cell(3, c+1).Value = columnNamesInCreditNote[c];
                        //ws.Range(3, 8, 3, 9).AddToNamed("ColumnNameStyleInSummary");

                        wsCN.Cell(3, c+1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#bfbfc2")); // Column Header Gray Color
                        wsCN.Cell(3, c+1).Style.Font.Bold = true;
                        wsCN.Cell(3, c+1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        wsCN.Cell(3, c+1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }


                    wsCN.Cell(4, 1).Value = "Various";
                    wsCN.Cell(4, 2).Value = "Onsite";
                    wsCN.Cell(4, 5).Value = "Fixed billing credit note";
                    wsCN.Cell(4, 10).Value = "-30000";

                  

                    //Assign Border to the Content
                    wsCN.Range(2, 1, 4, 10).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    wsCN.Range(2, 1, 4, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    //

                    //Show that Orange Row
                    wsCN.Range(5, 1, 5, 1).Value = "Fixed billing credit note"; 
                    wsCN.Range(5, 1, 5, 9).Merge().AddToNamed("TotalCostStyle");

                    wsCN.Cell(5, 10).Value = "-30000";
                    wsCN.Cell(5, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    wsCN.Cell(5, 10).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#ffc000")); //(XLColor.Orange);

                    //Onsite Total
                    wsCN.Cell(7, 8).Value = "Onsite Total";
                    wsCN.Cell(7, 10).Value = "-30000";
                    wsCN.Cell(7, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    wsCN.Range(7, 8, 7, 10).Style.Font.Bold = true;


                    //Formating the last Column/Total  Column                 
                    wsCN.Column(10).Style.NumberFormat.Format = "\u00A30.00"; //PoundSymbol 0.00

                    wsCN.Row(3).AdjustToContents(30.00, 30.00);  // To give height to the Column Headers
                    wsCN.Columns().AdjustToContents();

                    #endregion


                   // int totalWorksheetsCount = wb.Worksheets.Count();


                    //In the Final report,after Fixed Billing Onsite Credit Note worksheet,
                    //show all the Onsite details worksheets followed by Offshore details worksheets

                    int index = 2; // because Onsite details sheet will start from 4th Position index=3
                    foreach (IXLWorksheet worksheet in wb.Worksheets)
                    {                       
                        
                        if (worksheet.Name.ToString().Contains("Onsite") && 
                            worksheet.Name.ToString() != "Fixed Billing Onsite supp1." &&
                            worksheet.Name.ToString() != "FixedBilling Onsite Credit Note")
                        {
                            index++;                          
                            wb.Worksheet(worksheet.Name.ToString()).Position = index;
                        }
                       
                                                          
                    }

                    // Prepare the style for the Header title
                    var titlesStyle = wb.Style;
                    titlesStyle.Font.Bold = true;
                    titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    titlesStyle.Fill.BackgroundColor = XLColor.White;

                    // Format all titles in one shot
                    wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;


                    // Prepare the style for the Header Title in Summary WorkSheet
                    var titlesStyle_Summary = wb.Style;
                    titlesStyle_Summary.Font.Bold = true;
                    titlesStyle_Summary.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    titlesStyle_Summary.Fill.BackgroundColor = XLColor.FromHtml("#bdd7ee"); //Light sky Color

                    // Format all titles in one shot
                    wb.NamedRanges.NamedRange("SummarySheetTitle").Ranges.Style = titlesStyle_Summary;


                    // Prepare the style for the Column Names  in Summary WorkSheet
                    var columnNameStyle_Summary = wb.Style;
                    columnNameStyle_Summary.Fill.BackgroundColor = XLColor.FromHtml("#bdd7ee"); //Light sky Color
                    columnNameStyle_Summary.Font.Bold = true;
                    columnNameStyle_Summary.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    columnNameStyle_Summary.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                   

                    // Format all titles in one shot
                    wb.NamedRanges.NamedRange("ColumnNameStyleInSummary").Ranges.Style = columnNameStyle_Summary;


                    // Prepare the style for the Column Names  in Fixed Billing WorkSheet
                    var columnNameStyle_FB = wb.Style;
                    columnNameStyle_FB.Fill.BackgroundColor = XLColor.FromHtml("#333399"); //Blue Color
                    columnNameStyle_FB.Font.Bold = true;
                    columnNameStyle_FB.Font.FontColor = XLColor.White;
                    columnNameStyle_FB.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    columnNameStyle_FB.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    columnNameStyle_FB.Alignment.WrapText = true;
                    columnNameStyle_FB.Border.InsideBorder = XLBorderStyleValues.Thin;

                    if (dtFixedCostBilling.Rows.Count > 0)
                    {
                        // Format all titles in one shot
                        wb.NamedRanges.NamedRange("ColumnNameStyleInFB").Ranges.Style = columnNameStyle_FB;
                    }

                    var TotalCostStyle = wb.Style;
                    TotalCostStyle.Font.Bold = true;
                    TotalCostStyle.Font.FontColor = XLColor.Black;
                    TotalCostStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    TotalCostStyle.Fill.BackgroundColor = XLColor.FromHtml("#ffc000") ;//XLColor.Orange;

                    wb.NamedRanges.NamedRange("TotalCostStyle").Ranges.Style = TotalCostStyle;                  




                    using (MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.Position = 0;
                    }
                }
                               
               
            }

            return File(MyMemoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Invoice_" + SelectedMonth[0].Text + ".xlsx");
                     
        }


        public JsonResult GetWeekIdList(string month)
        {
            DateTime today = DateTime.Now;
            var m= month.Replace(today.Year.ToString(), "");
            m= month.Replace("-", "");
            m= month.ToLower().Trim();
            int iMonthNo = Convert.ToDateTime("01-" + m + "-" + today.Year).Month; 
 
            int daysInMonth = DateTime.DaysInMonth(today.Year, iMonthNo);
            DateTime firstOfMonth = new DateTime(today.Year, iMonthNo, 1);
            int firstDayOfMonth = (int)firstOfMonth.DayOfWeek;
            int weeksInMonth = (int)Math.Ceiling((firstDayOfMonth + daysInMonth) / 7.0);
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem()
            {
                Text = "All",
                Value = "0"
            });
            for (int i = 1; i <= weeksInMonth; i++)
            {
                items.Add(new SelectListItem()
                {
                    Text = "Week " + i,
                    Value = i.ToString()
                });
            }
            return Json(new SelectList(items, "Value", "Text"), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetProjectList()
        {

                SelectList items = (SelectList)TempData.Peek("Projects");
                return Json(items, JsonRequestBehavior.AllowGet);      
        }

        public JsonResult GetActivityList()
        {

                SelectList items = (SelectList)TempData.Peek("ActivityList");
                return Json(items, JsonRequestBehavior.AllowGet);      
        }

        public JsonResult GetCrList()
        {

            SelectList items = (SelectList)TempData.Peek("CrList");
            return Json(items, JsonRequestBehavior.AllowGet);
        }

         
        #endregion
        #region private methods
      
        private void FillDropdowns()
        {
            var Query = _ITimesheetEntryService.GetAllDropdown();
            TempData["resourceid"] = Convert.ToInt32(Query.Where(x => x.MasterName.ToUpper() == "RESOURCE" && x.Text.ToLower() == Session["IUserName"].ToString().ToLower()).Select(x => x.Value).FirstOrDefault());
            TempData.Keep("resourceid");
            var resourceid = (int)TempData.Peek("resourceid");

            if (Session["IRole"].ToString() == "Admin")
            {

                ViewBag.ResouceLst = new SelectList(
                          Query.Where(x => x.MasterName.ToUpper() == "RESOURCE").Select(x => new SelectListItem()
                          {
                              Text = x.Text,
                              Value = x.Value


                          }).ToList(), "Value", "Text");



            }
            else
            {

                ViewBag.ResouceLst = new SelectList(
                                  Query.Where(x => x.MasterName.ToUpper() == "RESOURCE" && x.RefId == resourceid.ToString()).Select(x => new SelectListItem()
                                  {
                                      Text = x.Text,
                                      Value = x.Value


                                  }).ToList(), "Value", "Text");

            }

                ViewBag.MonthLst = new SelectList(
                             Query.Where(x => x.MasterName.ToUpper() == "YEARMASTER" ).Select(x => new SelectListItem()
                             {
                                 Text = x.Text,
                                 Value = x.Value
                             }).OrderBy(n=>n.Value).ToList(), "Value", "Text");

       
           
           DateTime today = DateTime.Now;
           //extract the month
           int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
           DateTime firstOfMonth = new DateTime(today.Year, today.Month, 1);
           //days of week starts by default as Sunday = 0
           int firstDayOfMonth = (int)firstOfMonth.DayOfWeek;
           int weeksInMonth = (int)Math.Ceiling((firstDayOfMonth + daysInMonth) / 7.0);
           List<SelectListItem> items = new List<SelectListItem>();

           items.Add(new SelectListItem()
           {
               Text = "All",
               Value = "0"
           });
            for(int i=1;i <= weeksInMonth;i++)
            {
                items.Add(new SelectListItem()
                    {
                        Text = "Week "+ i,
                        Value = i.ToString()
                    });
            }

            ViewBag.WeekIdLst = new SelectList(items, "Value", "Text");

           //Keep Project Names in TempData

            TempData["Projects"] = new SelectList(
                                       Query.Where(x => x.MasterName.ToUpper() == "PROJECT").Select(x => new SelectListItem()
                                       {
                                           Text = x.Text,
                                           Value = x.Value


                                       }).ToList(), "Value", "Text");

            TempData["ActivityList"] = new SelectList(
                                                   Query.Where(x => x.MasterName.ToUpper() == "ACTIVITY").Select(x => new SelectListItem()
                                                   {
                                                       Text = x.Text,
                                                       Value = x.Value


                                                   }).ToList(), "Value", "Text");

            TempData["CrList"] = new SelectList(
                              Query.Where(x => x.MasterName.ToUpper() == "CRNUMBER").Select(x => new SelectListItem()
                              {
                                  Text = x.Text,
                                  Value = x.Value,
                                  Selected = true

                              }).ToList(), "Value", "Text");
        }

        //Added by Sasmita
        public  DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        #endregion
    }
}
