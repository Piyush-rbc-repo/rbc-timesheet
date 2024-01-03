using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Timesheet.Modal
{
    public class LookupMasterModal : BaseModal
    {
        /*public int? n_Id { get;set }
        */[Display(Name="Master Name")]
        public int? n_ParentId { get; set; }
        
        [Display(Name = "Parent Code")]
        public int? n_RefId { get; set; }
        
        [Display(Name = "Code")]
        public string s_MasterCode { get; set; }    

        [Display(Name = "Name")]
        public string s_MasterName { get; set; }

        public string value1 { get; set; }
        public string value2 { get; set; }
        public string value3 { get; set; }

        public string s_value1 { get; set; }
        public string s_value2 { get; set; }
        public string s_value3 { get; set; }
        public string s_value4 { get; set; }
        public string s_value5 { get; set; }
        public string s_value6 { get; set; }
        public string Oper { get; set; }

        public int MakerId { get; set; }
        public string ParentName { get; set; }

        //CR TypeName Added by Piyush 
        public string CRTypeName { get; set; }
        //Task Name Added by Piyush 
        public string TaskName { get; set; }

    }


}
