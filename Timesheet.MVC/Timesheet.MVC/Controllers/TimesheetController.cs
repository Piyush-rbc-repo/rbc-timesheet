using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.MVC.Models;
using Timesheet.Modal;
using System.Web.UI.WebControls;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Web.UI;
using System.Globalization;
using System.Data;
using System.Reflection;
using ClosedXML.Excel;

namespace Timesheet.MVC.Controllers
{
    [Authorize]
    public class TimesheetController : Controller
    {
        #region Constructor and Global Variable
        private Timesheet.Service.ServiceInterface.ITimesheetEntryService _ITimesheetEntryService;
        private Timesheet.Service.ServiceInterface.IMasterService _IMasterService;
        
        
        public TimesheetController()
        {
            _ITimesheetEntryService = new Timesheet.Service.ServiceLibrary.TimesheetEntryService();
            _IMasterService = new Timesheet.Service.ServiceLibrary.MasterService();
       }

        #endregion
        #region public Action Method
        
        
        
        public ActionResult Index()
        {
            TimesheetViwmodal viewmodal = new TimesheetViwmodal();
            viewmodal.timesheetSearchModal.FromDate = "01/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
            viewmodal.timesheetSearchModal.ToDate =DateTime.DaysInMonth(DateTime.Now.Year,DateTime.Now.Month).ToString()+ DateTime.Now.ToString("'/'MM'/'yyyy");
            FillDropdown();
            return View(viewmodal);
        }
        public ActionResult Create(TimesheetCreateModal model)
        {
            if (ModelState.IsValid)
            {
                model.ResourceId = (int)TempData.Peek("resourceid");
                CommoSaveResult result;
                if (!model.Id.HasValue)
                {
                    model.mode = "I";
                    result = _ITimesheetEntryService.Add(model);
                }
                else
                {
                    model.mode = "U";
                    result = _ITimesheetEntryService.Update(model, "U");
                }

                if (result.pn_Error)
                {
                    return new HttpStatusCodeResult(400, result.ps_Msg);

                }
                else
                {
                    model.Id = result.pn_RecordId;
                    return new JsonResult()
                    {
                        Data = new { model = model, msg = result.ps_Msg }
                        ,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            else
            {

                return new HttpStatusCodeResult(400, "Please fill all the required information!!!");
            }

            
        }
        public JsonResult Search(TimesheetSearchModal model)
            {
            if (model.Resource == null)
            {
                model.Resource = (int)TempData.Peek("resourceid");
            }

            List<TimesheetSearchResultModal> data = _ITimesheetEntryService.Search(model);
            var filtereddata=data.Where(x => x.VisibleToUser == true);
            return Json(filtereddata, JsonRequestBehavior.AllowGet);


        }

        private void LogDetails(string details)
        {
            string filePath = System.Configuration.ConfigurationManager.AppSettings["ErrorLogFile"].ToString();

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + details + "<br/>" + Environment.NewLine +
                   "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }
        public ActionResult Excel(TimesheetSearchModal model)
        {
            //   model.Resource = (int)TempData.Peek("resourceid");
            List<TimesheetSearchResultModal> result = _ITimesheetEntryService.Search(model);
            

            if (result.Count == 0)
            {
                return RedirectToAction("NoDataFound", "Error");
            }
            result.ToList().ForEach(i => i.Efforts_Days = i.ActualEfforts / 8);
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
            gridview.Columns.Add(new BoundField() { DataField = "Location", HeaderText = "Location" });
            gridview.DataSource = result;
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
            gridview.Columns[10].HeaderStyle.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            //gridview.Columns[0].ItemStyle.Width = 100;
            //gridview.HeaderRow.BackColor = System.Drawing.Color.FromArgb(192,192,192);

            // Clear all the content from the current response
            Response.ClearContent();
            Response.Buffer = true;
            DateTime dt = DateTime.ParseExact(result[0].ActivityDate, "dd/MM/yyyy",
                           CultureInfo.InvariantCulture);
            string month = dt.ToString("MMMM");
            string OutputFileName = (model.Resource == null ? "Full" : result[0].ResourceName.ToString()).Replace(' ', '_') + "_" +
                                month + "_" +
                                dt.ToString("yyyy") + " _TIMESHEET_Offshore";


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
        public ActionResult DownloadExcel(TimesheetSearchModal model)
        {
            string offshoreOnshoreFlag = string.Empty;
            string strLocation = string.Empty;
            MemoryStream MyMemoryStream = null;

            // model.Resource = (int)TempData.Peek("resourceid");
            // model.Resource = model.ResourceId;
           
            List<TimesheetSearchResultModal> result = _ITimesheetEntryService.Search(model);


            if (result.Count == 0)
            {
                return RedirectToAction("NoDataFound", "Error");
            }
            result.ToList().ForEach(i => i.Efforts_Days = i.ActualEfforts / 8);


            if (model.Resource != null)
                strLocation = Convert.ToString(result.Select(x => x.Location).First());

            if (!string.IsNullOrEmpty(strLocation))
                offshoreOnshoreFlag = strLocation.ToLower() == "india" ? "_TIMESHEET_Offshore" : "_TIMESHEET_Onshore";

            DataTable dtTimesheet = ToDataTable<TimesheetSearchResultModal>(result);

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
            dtTimesheet.Columns.Remove("ActivityId");
            
            dtTimesheet.Columns.Remove("CrLongName");
            dtTimesheet.Columns.Remove("OnsiteManagerName");
            dtTimesheet.Columns.Remove("ProjectType");

            //Rename the columns to show in Excel
            dtTimesheet.Columns["ActivityDate"].ColumnName = "Date";
            dtTimesheet.Columns["ResourceName"].ColumnName = "Resource Name";
            dtTimesheet.Columns["ProjectName"].ColumnName = "Project";
            dtTimesheet.Columns["CrNumber"].ColumnName = "CR Number";
          
            dtTimesheet.Columns["CrTypeName"].ColumnName = "Cr Type";       // Added CrType Name
            dtTimesheet.Columns["Tasks"].ColumnName = "Tasks";     // Addded by Piyush 

            dtTimesheet.Columns["Activity"].ColumnName = "Activity";
            dtTimesheet.Columns["SubActivity"].ColumnName = "Sub Activity";
            dtTimesheet.Columns["Efforts"].ColumnName = "Efforts (Hrs)";
            dtTimesheet.Columns["Efforts_Days"].ColumnName = "Efforts (Days)";
            dtTimesheet.Columns["Billable"].ColumnName = "Billable Flag";
            dtTimesheet.Columns["Comments"].ColumnName = "Comments";
            dtTimesheet.Columns["Location"].ColumnName = "Location";


            DateTime dt = DateTime.ParseExact(result[0].ActivityDate, "dd/MM/yyyy",
                           CultureInfo.InvariantCulture);
            string month = dt.ToString("MMMM");


            string OutputFileName = (model.Resource == null ? "Full" : result[0].ResourceName.ToString()).Replace(' ', '_') + "_" +
                                month + "_" +
                                dt.ToString("yyyy") + offshoreOnshoreFlag;


            string worksheetName = (model.Resource == null ? "Full" : result[0].ResourceName.ToString()).Replace(' ', '_');
                //+ "_" + month + "_" + dt.ToString("yyyy");


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

                using (MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.Position = 0;
                }
            }


            return File(MyMemoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", OutputFileName + ".xlsx");

            // return View();


        }

        public ActionResult DownloadDefaulterList(string FromDate, string ToDate)
        {
         
            MemoryStream MyMemoryStream = null;

            
            List<TimesheetDefaulterListModal> result = _ITimesheetEntryService.GetDefaulterList(FromDate, ToDate);

            if (result.Count == 0)
            {
                return RedirectToAction("NoDataFound", "Error");
            }


            string month = FromDate.Split('/')[1].ToString();

            string NewFromDate;

            //This is to append 0 in the month. When you don't select any date from start date calendar the date was coming as 01/9/2021 (an example) . 0 is missing in leftside of 9
            if (month.Length < 2)
                NewFromDate = FromDate.Split('/')[0].ToString() + "/" + (string.Format("{0}{1}", "0", month ))+ "/" + FromDate.Split('/')[2].ToString();

            else
                NewFromDate = FromDate;

            DateTime dt = DateTime.ParseExact(NewFromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            string monthName = dt.ToString("MMMM");

            DataTable dtDefaulter = ToDataTable<TimesheetDefaulterListModal>(result);

            dtDefaulter.Columns.Remove("ResourceID");            

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("DefaulterList_" + monthName); // Name of the  worksheet /Tab               
                ws.Range(1, 1, 1, 2).Value = "Defaulter List"; //Name of the Heading
                ws.Range(1, 1, 1, 2).Merge().AddToNamed("Titles");
               
                ws.Cell(2, 1).InsertTable(dtDefaulter);

                // Prepare the style for the titles
                var titlesStyle = wb.Style;
                titlesStyle.Font.Bold = true;
                titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                titlesStyle.Fill.BackgroundColor = XLColor.Cyan;


                // Format all titles in one shot
                wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

                // wb.Worksheets.Add(dtDefaulter, "DefaulterList_"+ monthName);

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                using (MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.Position = 0;
                }
            }



            return File(MyMemoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DefaulterList_"+ monthName + ".xlsx");

        }

        public JsonResult SubmitRecords(string i, string partialSubmit)
        {
            TimesheetSubmitValidationResult result = _ITimesheetEntryService.DeleteSubmit(i, false, true, (int)TempData.Peek("resourceid"), Convert.ToBoolean(partialSubmit));
            return new JsonResult(){
                Data=new{result=result.SaveResult,CauseList=result.timesheetSubmitValidationOutput,Idarray=i,type='S'},
                JsonRequestBehavior=JsonRequestBehavior.AllowGet

            };

        }
        public JsonResult DeleteRecords(string i)
        {
            TimesheetSubmitValidationResult result = _ITimesheetEntryService.DeleteSubmit(i, true, false, (int)TempData.Peek("resourceid"));
            return new JsonResult()
            {
                Data = new { result = result.SaveResult, Idarray = i, type = 'D' },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet

            };

        }
        
        #endregion
        #region public FileUpload Method

        public ActionResult GetCrList(int ProjectId )
        {
            var result =CrNumber(ProjectId);
            return Json(result, JsonRequestBehavior.AllowGet);


        }
        //Added by Sasmita from Old code
        public ActionResult GetProjectList(int CrNumber)
        {
            var result = ProjectNumber(CrNumber);
            return Json(result, JsonRequestBehavior.AllowGet);


        }
        public ActionResult UploadCsv(HttpPostedFileBase file,bool overWriteExistsing)
        {
            if(TempData.Peek("resourceid")==null)
            {
                
                return new JsonResult()
                {

                    Data = new { pn_Error = true, ps_Msg = "Session expired. please relogin" }

                };

            }
            if(file==null)
            {
                return new JsonResult()
                {

                    Data = new { pn_Error = true, ps_Msg = "Please select file to upload" }

                };


            }
            else if (file.ContentLength == 0 )
            {

                return new JsonResult()
                {

                    Data = new { pn_Error = true, ps_Msg = "Empty file can not be processed" }

                };

            }
            else
            {
                int userid =(int)TempData.Peek("resourceid");
               var result = _ITimesheetEntryService.UploadCsv(file, userid, overWriteExistsing);
              
                return new JsonResult()
                {

                    Data = new { pn_Error = result.result.pn_Error, ps_Msg = result.result.ps_Msg, data = result.timesheetFileUploadModal}

                };

            }

        }
        public ActionResult UploadExcel(HttpPostedFileBase file,bool overWriteExistsing)
        {
            
            if (TempData.Peek("resourceid") == null)
            {

                return new JsonResult()
                {

                    Data = new { pn_Error = true, ps_Msg = "Session expired. please relogin" }

                };

            }
            if (file == null)
            {
                return new JsonResult()
                {

                    Data = new { pn_Error = true, ps_Msg = "Please select file to upload" }

                };


            }
            else if (file.ContentLength == 0)
            {

                return new JsonResult()
                {

                    Data = new { pn_Error = true, ps_Msg = "Empty file can not be processed" }

                };

            }
            else
            {
                try
                {
                    int userid = (int)TempData.Peek("resourceid");
                    string fileName = userid.ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + System.IO.Path.GetExtension(file.FileName);
                    if (System.IO.File.Exists(Common.Constants.UploadFilePath + fileName))
                    {
                        try
                        {
                            System.IO.File.Delete(Common.Constants.UploadFilePath + fileName);


                        }
                        catch (Exception e)
                        {
                            return new JsonResult()
                            {

                                Data = new { pn_Error = true, ps_Msg = "your request can not be processed this moment. <br> Exception :  " + e.Message }
                            };

                        }


                    }
                    if (!System.IO.Directory.Exists(Common.Constants.UploadFilePath))
                    {
                        System.IO.Directory.CreateDirectory(Common.Constants.UploadFilePath);

                    }
                    file.SaveAs(Common.Constants.UploadFilePath + fileName);

                    var result = _ITimesheetEntryService.UploadExcel(file, userid, overWriteExistsing, Common.Constants.UploadFilePath + fileName);


                    return new JsonResult()
                    {

                        Data = new { pn_Error = result.result.pn_Error, ps_Msg = result.result.ps_Msg, data = result.timesheetFileUploadModal }

                    };

                }

                catch (Exception ex)
                {
                    return new JsonResult()
                               {

                                   Data = new { pn_Error = true, ps_Msg = "your request can not be processed this moment. <br> Exception :  " + ex.Message }
                               };


                }
            }

        }
        #endregion
        #region PrivateMethods
        private SelectList CrNumber(int projectid)
        {

            return new SelectList(_IMasterService.GetById(6).Where(x => (int)x.n_RefId == projectid).Select(x => new SelectListItem()
                                      {
                                          Text = x.s_MasterCode,
                                          Value = x.n_Id.ToString()


                                      }).ToList(), "Value", "Text");
                                      
           

        }


        //Added by Sasmita from Old code
        private SelectList ProjectNumber(int crNumber)
        {
            if (crNumber == 0)
            {
                //Commented by Sasmita to mantain the Project List same in the dropdown, both on Page load and when Cr is --Select--

                //return new SelectList(_IMasterService.GetById(6).Select(x => new SelectListItem()
                //{
                //    Text = x.ParentName,
                //    Value = x.n_RefId.ToString()


                //}).ToList(), "Value", "Text");


                var Query = _ITimesheetEntryService.GetAllDropdown();
                return new SelectList(
                                 Query.Where(x => x.MasterName.ToUpper() == "PROJECT").Select(x => new SelectListItem()
                                 {
                                     Text = x.Text,
                                     Value = x.Value


                                 }).ToList(), "Value", "Text");


            }
            else
            {
                return new SelectList(_IMasterService.GetById(6).Where(x => (int)x.n_Id == crNumber).Select(x => new SelectListItem()
                {
                    Text = x.ParentName,
                    Value = x.n_RefId.ToString()


                }).ToList(), "Value", "Text");
            }


        }

        private void FillDropdown()
        {
            string UserLocation ;
            var Query = _ITimesheetEntryService.GetAllDropdown();
            ViewBag.ActivityList = new SelectList(
                                                   Query.Where(x => x.MasterName.ToUpper() == "ACTIVITY").Select(x => new SelectListItem()
                                                               {
                                                                   Text = x.Text,
                                                                   Value = x.Value


                                                               }).ToList(), "Value", "Text");

            // Added by Piyush to fetch data for dropdown in timesheet

            ViewBag.TaskList = new SelectList(
                            Query.Where(x => x.MasterName.ToUpper() == "TASKS").Select(x => new SelectListItem()
                            {
                                Text = x.Text,   
                                Value = x.Value
                            }).ToList(), "value", "Text");


            ViewBag.ProjectList = new SelectList(
                                      Query.Where(x => x.MasterName.ToUpper() == "PROJECT").Select(x => new SelectListItem()
                                      {
                                          Text = x.Text,
                                          Value = x.Value


                                      }).ToList(), "Value", "Text");
            ViewBag.BillableEnum = new SelectList(new List<SelectListItem> {
            new SelectListItem{Text="Yes", Value="True"},
            new SelectListItem{Text="No", Value="False"}
            }, "Value", "Text", 0);

            ViewBag.ResourceList = new SelectList(
                             Query.Where(x => x.MasterName.ToUpper() == "RESOURCE" ).Select(x => new SelectListItem()
                             {
                                 Text = x.Text,
                                 Value = x.Value


                             }).ToList(), "Value", "Text",0);

            ViewBag.ResourceListMe = new SelectList(
                              Query.Where(x => x.MasterName.ToUpper() == "RESOURCE" && x.Value.ToLower() == Session["IUsecode"].ToString().ToLower()).Select(x => new SelectListItem()
                              {
                                  Text = x.Text,
                                  Value = x.Value
                                  

                              }).ToList(), "Value", "Text");

            //commented code to fix issue 
         //   TempData["resourceid"]=Convert.ToInt32(Query.Where(x => x.MasterName.ToUpper() == "RESOURCE" && x.Text.ToLower() == Session["IUserName"].ToString().ToLower()).Select(x => x.Value).FirstOrDefault());

            // new code to handle resource id - Mitesh Patil
            int resourceid = Convert.ToInt32(Query.Where(x => x.MasterName.ToUpper() == "RESOURCE" && x.Value.ToLower() == Session["IUsecode"].ToString().ToLower()).Select(x => x.Value).FirstOrDefault());
            if (resourceid>0)
            {
                TempData["resourceid"] = resourceid;
            }
            else
            {
                TempData["resourceid"] = Session["IUsecode"];
            }


            ViewBag.CrList = new SelectList(
                              Query.Where(x => x.MasterName.ToUpper() == "CRNUMBER").Select(x => new SelectListItem()
                              {
                                  Text = x.Text,
                                  Value = x.Value,
                                  Selected=true

                              }).ToList(), "Value", "Text");
            
            //if user is not a onsite person disable the bulk upload feature.
            UserLocation = Query.Where(x => x.MasterName.ToUpper() == "RESOURCE" && x.Value.ToLower() == Session["IUsecode"].ToString().ToLower()).Select(x => x.s_Value3).FirstOrDefault();
            ViewBag.BulkUpload = (string.Equals(UserLocation, "Jersey", StringComparison.OrdinalIgnoreCase) ? "block" : "none");
           
        }



        //Added by Sasmita
        public DataTable ToDataTable<T>(List<T> items)
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
