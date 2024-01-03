using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Modal
{
    public class TimesheetFileUploadModal
    {
        public int? n_id { get; set; }
        public string Date { get; set; }
        public string ResourceName  { get; set; }
        public string Project { get; set; }
        public string CRNumber { get; set; }
        public string Activity { get; set; }
        public string SubActivity { get; set; }
        public string Efforts { get; set; }
        public string Efforts_days { get; set; }
        public string BillableFlag { get; set; }
        public string Comments { get; set; }
    }

    public class TimesheetFileUploadResultModal
    {
        public List<TimesheetSearchResultModal> timesheetFileUploadModal = new List<TimesheetSearchResultModal>();
        public  CommoSaveResult result = new CommoSaveResult();
    }

}
