using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Modal;
namespace Timesheet.Database.DataInterface
{
    public interface IDataMaster
    {
        List<LookupMasterModal> GetAll();
        List<LookupMasterModal> GetById(int id);
        CommoSaveResult Add(LookupMasterModal modal);
        CommoSaveResult Update(LookupMasterModal modal);
        List<LookupMasterModal> Delete(int id);

    }
}
