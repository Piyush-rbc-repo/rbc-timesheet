using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Timesheet.Modal
{
    public class TimesheetSearchModal
    {
        
        [Display(Name = "From Date*")]
        public string FromDate { get; set; }
        
        
        [Display(Name = "To Date*")]
        public string ToDate { get; set; }

        [Display(Name = "Activity")]
        public int? Activity { get; set; }

        [Display(Name = "Tasks")]           // Added By piyush for storing subtask 
        public int? Tasks { get; set; }

        [Display(Name = "Project")]
        public int? Project { get; set; }

        [Display(Name = "Billable")]
        public bool? Billable { get; set; }

        [Display(Name = "Resource")]
        public int? Resource { get; set; }

        // CR Type Added By Piyush 
        [Display(Name = "Cr Type")]
        public string CrType { get; set; }

        [Display(Name = "CR Number")]
        public int? Cr { get; set; }

        [Display(Name = "CR Project Name")]
        public int? CrProject { get; set; }

        //[Display(Name = "ResourceId")]
        //public int? ResourceId { get; set; }
    }
    public class TimesheetSearchResultModal
    {
        public int n_id  {get;set;}
        public string ActivityDate { get; set; }
        public string ResourceName { get; set; }
        public string ProjectName { get; set; }
        public string CrNumber { get; set; }
        public string CrTypeName { get;set; }
        public string Activity { get; set; }
         
        public string Tasks { get;set; }   // Added By piyush for storing subtask 
        public string SubActivity { get; set; }
        public decimal Efforts { get; set; }
        public decimal Efforts_Days { get; set; }
        public string Billable { get; set; }
        public string Comments { get; set; }
        public string CrProject { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CrNumberId { get; set; }
        public int ProjectId { get; set; }
        public int ResoureId { get; set; }
        public int? n_status { get; set; }
        public decimal Approved_Cost { get; set; }
        public int? ManagerId { get; set; }
        public string RejectionReason { get; set; }
        public decimal ActualEfforts { get; set; }
        public bool VisibleToUser { get; set; }
        public string Location { get; set; }
        public int ActivityId { get; set; }
        public string CrLongName { get; set; }
        public string OnsiteManagerName { get; set; }
        public string ProjectType { get; set; }

    }
    public class TimesheetViwmodal
    {
      public  TimesheetCreateModal timesheetCreateModal = new TimesheetCreateModal();
      public TimesheetSearchModal timesheetSearchModal = new TimesheetSearchModal();

    }
    public class TimesheetCreateModal
    {
        public int? Id { get; set; }

        [Required(ErrorMessage="Required")]
        [Display(Name="Activity Date*")]
        public string Activitydate {get;set;}

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Resource Name*")]
        public int ResourceId { get; set; }


        [Required(ErrorMessage = "Required")]
        [Display(Name = "Project*")]
        public int ProjectId { get; set; }


        
        [Display(Name = "CR Number")]
        public int? CrNumberId { get; set; }


        [Required(ErrorMessage = "Required")]
        [Display(Name = "Activity*")]
        public int ActivityId { get; set; }

        
        [Display(Name = "Sub Activity")]
        public string SubActivity { get; set; }

        [Required(ErrorMessage="Required")]
        [Display(Name="Tasks")]
        public int TasksId { get; set; }


        [Required(ErrorMessage = "Required")]
        [Range(0.1,999.99,ErrorMessage="Invalid Value")]
        [Display(Name = "Efforts (Hrs.)")]
        public float Efforts { get; set; }

        public float ActualEfforts { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Billable Flag*")]
        public bool IsBillable { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Display(Name = "With Submit")]
        public bool IsSubmit { get; set; }

        public string mode { get; set; }

    }

    public class TimesheetDefaulterListModal
    {
        public string ResourceID { get; set; }
        public string ResourceName { get; set; }
        public string MissingDate { get; set; }
        public string Location { get; set; }
    }

    public class MonthlyInvoiceModal
    {
        public int ResourceId { get; set; }
        public string NameOfResource { get; set; }

        public string Billable { get; set; }
        public string Location { get; set; }
        public string Role { get; set; }
        public string CRNumber { get; set; }
        public string IsOLSCR { get; set; }
        public string Task { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public decimal? WorkingDays { get; set; }
        public decimal? PerDayCost { get; set; }
        public decimal? Total { get; set; }
    }
}
