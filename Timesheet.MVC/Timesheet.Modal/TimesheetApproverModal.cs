using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Modal
{
    public   class TimesheetApproverModal
    {
        public string Ids {get;set;} 
        public int? IsApproved {get;set;}
        public string RejectionReason {get;set;}
        public bool status {get;set;}
    }
}
