using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Modal
{
    public class TimesheetSubmitValidationResult
    {
        public List<TimesheetSubmitValidationResultModal> timesheetSubmitValidationOutput = new List<TimesheetSubmitValidationResultModal>();
        public CommoSaveResult SaveResult = new CommoSaveResult();


    }
   public class TimesheetSubmitValidationResultModal
    {
       public int n_ResourceId { get; set; }
       public string FullName { get; set; }
       public string Date { get; set; }
       public string Cause { get; set; }
    }
}
