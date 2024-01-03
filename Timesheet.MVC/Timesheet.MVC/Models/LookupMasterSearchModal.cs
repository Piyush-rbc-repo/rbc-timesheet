using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Timesheet.MVC.Models
{
    public class LookupMasterSearchModal
    {
        [Display(Name="Master Code")]
        public string MasterCode { get; set; }
        [Display(Name = "Master Name")]
        public string MasterName { get; set; }

    }

   public class LookupMasterAjaxResult
   {
       public int MasterId { get; set; }
       public string MasterCode { get; set; }
       public string MasterName { get; set; }
       public string CreatedOn { get; set; }
       public string ActiveStatus { get; set; }
   }

    public class HolidayMasterSearchModal
    {
        public int Id { get; set; }
        public DateTime HolidayDate { get; set; }
        public string HolidayDescription { get; set; }
        public string ActiveStatus { get; set; }
    }
}