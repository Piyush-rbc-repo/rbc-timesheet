using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace Timesheet.Common
{
    public sealed class Constants
    {
        private Constants() { }
        #region UserRoles
            
            public const string Manager = "Project Manager";
            public const string TechnicalLeader = "Technical Leader";
            public const string Developer = "Senior Developer";
        
        #endregion


        #region SP Name Constants
        
            public const string spGetAllMaster = "usp_AddUpdateLookupMaster";
            public const string spAddUpdate_TimesheetEntry = "usp_AddUpdate_TimesheetEntry";
            public const string spDeleteSubmit_Timesheet = "usp_DeleteSubmit_Timesheet";
            public const string spDashboard = "usp_Dashboard";
            public const string spSearchTimeSheet = "USP_GetTimeSheet";
            public const string spProjectCrLevelDashboard = "usp_GetCrLevelSummary";
            public const string spApproveRejectTimesheet = "usp_ApproveRejectTimesheet";
            public const string spUpdateTimeSheetDataByApprovar = "usp_UpdateTimeSheetDataByApprovar";
            public const string spBulkSaveTimesheet = "usp_BulkSaveTimesheet";
            public const string spGetDefaulterList = "usp_GetDefaulterList";

        #endregion

        #region File Path Constatnts

        public const string UploadFilePath = @"D:\TimeSheet\Application\Uploads\";
            public const string DownloadFileBaseUrl = @"~/download/";

        #endregion
            
    }
}
