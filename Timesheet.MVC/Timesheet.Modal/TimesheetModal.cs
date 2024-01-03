using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Modal
{
    public class TimesheetModal : BaseModal
    {
        public DateTime d_ActivityDate { get; set; }
        public int n_ResourceId { get; set; }
        public int n_ProjectId { get; set; }
        public int n_CrId { get; set; }
        public int n_ActivityId { get; set; }
        public string s_SubActivity { get; set; }
        public float n_Efforts_Mins { get; set; }
        public bool n_IsBillable { get; set; }
        public string s_Comments { get; set; }
        public bool B_IsSubmitted { get; set; }
    }
}

