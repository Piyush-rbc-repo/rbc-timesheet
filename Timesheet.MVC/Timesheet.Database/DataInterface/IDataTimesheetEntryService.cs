using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Modal;
//using System.Web.Mvc;
using System.Web;
namespace Timesheet.Database.DataInterface
{
    public interface IDataTimesheetEntryService
    {
        List<TimesheetModal> GetAll();
        List<TimesheetModal> GetById(int id);
        CommoSaveResult Add(TimesheetCreateModal modal);
        CommoSaveResult Update(TimesheetCreateModal modal, string comm);
        TimesheetSubmitValidationResult DeleteSubmit(string ids, bool Delete, bool submit, int userid,bool partialSubmit=false);
        List<SelectListModal> GetAllDropdown();
        List<TimesheetSearchResultModal> Search(string Fromdate, string ToDate, int? activityId, int? projectId, bool? billable, int? resourceId, int? crNumberId, int? crProjectId);
        List<TimesheetSearchResultModal> GetDashboard(ReportSearchModal modal);
        CommoSaveResult ApproveReject(TimesheetApproverModal modal, int userid,bool insertRecords );
        ProjectDashboardModal GetProjectCrBasedDashboard(ProjectCrBasedSearchModal modal);
        CommoSaveResult UpdateRecordFromApproverDashboard(List<TimesheetSearchResultModal> modal, int userid);
        TimesheetFileUploadResultModal UploadCsv(List<TimesheetFileUploadModal> modal, int Userid, bool overwriteExistsing);

        List<TimesheetDefaulterListModal> GetDefaulterList(string Fromdate, string ToDate);

    }
}
