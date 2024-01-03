using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Timesheet.Modal
{
    public class ReportSearchModal
    {
        public int? ResourceId { get; set; }
        
        [Display(Name="Month*")]
        [Required(ErrorMessage="Required")]
        public int MonthId { get; set; }

        [Display(Name = "Week*")]
        [Required(ErrorMessage = "Required")]
        public int WeekId { get; set; }

        [Display(Name = "Approved Only")]
        public bool statusId { get; set; }
    }

    public class SummaryReport
    {
        public string CrNumber { get; set; }
        public string ProjectName { get; set; }
        public float UnBilledDays{ get; set; }
        public string BilledDays { get; set; }
        public float BilledHrs { get; set; }
        public int? crId { get; set; }
        public int? ProjectId { get; set; }
        public float ApprovedCost { get; set; }
        
    }
}
