using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Modal;
namespace Timesheet.Database.DataInterface
{
    public interface IMasterData
    {
        CommoSaveResult InsertUpdateMaster(LookupMasterModal modal);



    }
}
