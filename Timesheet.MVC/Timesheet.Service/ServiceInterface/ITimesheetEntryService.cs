using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Modal;
//using System.Web.Mvc;

namespace Timesheet.Service.ServiceInterface
{
   public interface ITimesheetEntryService
    {
        List<TimesheetModal> GetAll();
        List<TimesheetModal> GetById(int id);
        CommoSaveResult Add(TimesheetCreateModal modal);
        CommoSaveResult Update(TimesheetCreateModal modal, string comm);
        TimesheetSubmitValidationResult DeleteSubmit(string ids, bool Delete, bool submit, int userid,bool partialSubmit=false);
        List<TimesheetModal> Delete(int id);
        List<SelectListModal> GetAllDropdown();
        List<TimesheetSearchResultModal> Search(TimesheetSearchModal modal);
        CommoSaveResult ApproveReject(TimesheetApproverModal modal, int userid,bool insertRecords);
        ProjectDashboardModal GetProjectCrBasedDashboard(ProjectCrBasedSearchModal modal);
        CommoSaveResult UpdateRecordFromApproverDashboard(List<TimesheetSearchResultModal> modal, int userid);
        TimesheetFileUploadResultModal UploadCsv(System.Web.HttpPostedFileBase file, int Userid, bool overwriteExistsing);
        TimesheetFileUploadResultModal UploadExcel(System.Web.HttpPostedFileBase file, int Userid, bool overwriteExistsing, string FileNameWithPath);

        List<TimesheetDefaulterListModal> GetDefaulterList(string Fromdate, string ToDate);
    }
}
