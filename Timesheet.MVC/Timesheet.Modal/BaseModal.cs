using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Modal
{
    public class BaseModal
    {
        public int n_Id { get; set; }
        public bool b_IsActive { get; set; }
        public bool b_IsDeleted { get; set; }
        public DateTime d_CreatedOn { get; set; }
        public int n_CreatedBy { get; set; }
        public DateTime? d_ModifiedOn { get; set; }
        public int? n_ModifiedBy { get; set; }
        
        
    }
}
