using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Timesheet.Modal
{
    public class Login
    {
        [Display(Name="User Name")]
        public string userid { get; set; }
        
        [Display(Name = "Password")]
        public string password { get;set;}

        [Display(Name = "Use Windows Auth")]
        public bool UseWindowsAuth { get; set; }

        
    }
}
