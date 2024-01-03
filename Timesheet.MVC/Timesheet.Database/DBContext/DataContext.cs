using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Timesheet.Modal;
namespace Timesheet.Database
{
   public class DataContext  : DbContext
    {
       public DataContext()
           : base("Database")
       {

       }
       public DbSet<LookupMasterModal> LookupMaster { get; set; }
     
    }
}
