using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Modal;
namespace Timesheet.Service.ServiceInterface
{
    public interface IDashboardService
    {
        List<TimesheetSearchResultModal> Search(ReportSearchModal modal);
    }
}
