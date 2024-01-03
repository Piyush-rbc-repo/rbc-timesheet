using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Modal;
using Timesheet.Service.ServiceInterface;
namespace Timesheet.Service.ServiceLibrary
{
    public class DashboardService : IDashboardService
    {
        Database.DataService.TimesheetEntry _TimesheetDL = new Database.DataService.TimesheetEntry();
        public List<TimesheetSearchResultModal> Search(ReportSearchModal modal)
        {

            return _TimesheetDL.GetDashboard(modal);
            
        }

    }
}
