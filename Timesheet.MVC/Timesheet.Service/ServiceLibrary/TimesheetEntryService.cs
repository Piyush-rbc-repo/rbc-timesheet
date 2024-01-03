using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Service.ServiceInterface;
using Timesheet.Modal;
using Timesheet.Database;
namespace Timesheet.Service.ServiceLibrary
{
    public class TimesheetEntryService : ITimesheetEntryService
    {
        Database.DataInterface.IDataTimesheetEntryService  _TimesheetDL = new Database.DataService.TimesheetEntry();
        //public TimesheetEntryService (Database.DataInterface.IDataTimesheetEntryService TimesheetDL)
        //{
        //    this._TimesheetDL = TimesheetDL;

        //}

        public List<TimesheetModal> GetAll() {
            
            return new List<TimesheetModal>();
        }
        public List<TimesheetModal> GetById(int id) {
            return new List<TimesheetModal>();
        }
        public CommoSaveResult Add(TimesheetCreateModal modal)
        {
          Timesheet.Modal.CommoSaveResult result=_TimesheetDL.Add(modal);
          return result;
        }
        public CommoSaveResult Update(TimesheetCreateModal modal, string comm)
        {
            Timesheet.Modal.CommoSaveResult result = _TimesheetDL.Update(modal, comm);
            return result;
        }

        public TimesheetSubmitValidationResult DeleteSubmit(string ids, bool Delete, bool submit, int userid,bool partialSubmit=false)
        {
            TimesheetSubmitValidationResult result = _TimesheetDL.DeleteSubmit(ids, Delete, submit, userid, partialSubmit);
            return result;
        }

        public List<TimesheetModal> Delete(int id) {
            return new List<TimesheetModal>();
        }


        public List<SelectListModal> GetAllDropdown()
        {
            return _TimesheetDL.GetAllDropdown();
        }


        public List<TimesheetSearchResultModal> Search(TimesheetSearchModal modal)
        {

            return _TimesheetDL.Search(modal.FromDate, modal.ToDate, modal.Activity, modal.Project, modal.Billable, modal.Resource, modal.Cr, modal.CrProject);
            
        }


       public CommoSaveResult ApproveReject(TimesheetApproverModal modal, int userid,bool insertRecords =false)
        {
            return _TimesheetDL.ApproveReject(modal, userid, insertRecords);

        }


       public ProjectDashboardModal GetProjectCrBasedDashboard(ProjectCrBasedSearchModal modal)
       {
           return _TimesheetDL.GetProjectCrBasedDashboard(modal);
        
       }

       public CommoSaveResult UpdateRecordFromApproverDashboard(List<TimesheetSearchResultModal> modal, int userid)
       {

           return _TimesheetDL.UpdateRecordFromApproverDashboard(modal, userid);
       }

       public TimesheetFileUploadResultModal UploadCsv(System.Web.HttpPostedFileBase file, int Userid, bool overwriteExistsing)
       {
           var reader = new System.IO.StreamReader(file.InputStream);
           List<TimesheetFileUploadModal> datalist = new List<TimesheetFileUploadModal>();
           int i =0;
           while (!reader.EndOfStream)
           {
               var line = reader.ReadLine();
               if(i==0)
               {
                   i++;
                   continue; //skip reading header

               }
               i++;
        
               var values = line.Split('\t');
               if (values.Length<10)
               {
                   TimesheetFileUploadResultModal modal = new TimesheetFileUploadResultModal();
                   modal.result.pn_Error = true;
                   modal.result.ps_Msg = "there are format mismatch on line no . " + i.ToString();
                   return modal;
               }
               TimesheetFileUploadModal data = new TimesheetFileUploadModal();

               data.Date = values[0];
               data.ResourceName = values[1];
               data.Project = values[2];
               data.CRNumber = values[3];
               data.Activity = values[4];
               data.SubActivity = values[5];
               data.Efforts = values[6];
               data.Efforts_days = values[7];
               data.BillableFlag = values[8];
               data.BillableFlag = values[8];
               data.Comments = values[9];
               datalist.Add(data);
                
           }
           return ProcessUploadCsv(datalist, Userid, overwriteExistsing);


       }
        private TimesheetFileUploadResultModal ProcessUploadCsv(List<TimesheetFileUploadModal> modal, int Userid, bool overwriteExistsing)
       {
           return _TimesheetDL.UploadCsv(modal, Userid, overwriteExistsing);

       }
        private TimesheetFileUploadResultModal ProcessUploadExcel(List<TimesheetFileUploadModal> modal, int Userid, bool overwriteExistsing)
        {
            return _TimesheetDL.UploadCsv(modal, Userid, overwriteExistsing);

        }


        public TimesheetFileUploadResultModal UploadExcel(System.Web.HttpPostedFileBase file, int Userid, bool overwriteExistsing,string FileNameWithPath)
        {
            var result = Common.Functions.ReadExcel<TimesheetFileUploadModal>(FileNameWithPath).Where(x=>
                string.IsNullOrEmpty(x[0].ToString())==false && 
                string.IsNullOrEmpty(x[1].ToString())==false && 
                string.IsNullOrEmpty(x[2].ToString())==false && 
                string.IsNullOrEmpty(x[4].ToString())==false && 
                string.IsNullOrEmpty(x[6].ToString())==false && 
                string.IsNullOrEmpty(x[8].ToString())==false 
                
                ).Select(x => new TimesheetFileUploadModal() { 
            
            Date= x[0].ToString(),
            ResourceName = x[1].ToString(),
            Project = x[2].ToString(),
            CRNumber = x[3].ToString(),
            Activity = x[4].ToString(),
            SubActivity = x[5].ToString(),
            Efforts = x[6].ToString(),
            Efforts_days = x[7].ToString(),
            BillableFlag = x[8].ToString(),
            
            }).ToList(); ;

            return _TimesheetDL.UploadCsv(result, Userid, overwriteExistsing);

        }

        //Added by Sasmita | Extract DefaulterList functionality
        public List<TimesheetDefaulterListModal> GetDefaulterList(string Fromdate, string ToDate)
        {

            return _TimesheetDL.GetDefaulterList(Fromdate, ToDate);

        }
    }
}
