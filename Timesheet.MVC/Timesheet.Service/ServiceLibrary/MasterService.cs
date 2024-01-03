using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Service.ServiceInterface;
using Timesheet.Modal;
using Timesheet.Database;
namespace Timesheet.Service.ServiceLibrary
{
    public class MasterService : IMasterService
    {
        private Database.DataInterface.IDataMaster _IDataMaster = new Database.DataService.DataMaster();
        public List<LookupMasterModal> GetAll() {

            return _IDataMaster.GetAll();
            
        }
        public List<LookupMasterModal> GetById(int id) {
            return _IDataMaster.GetById(id);
        }
        public CommoSaveResult Add(LookupMasterModal modal) {

            return _IDataMaster.Add(modal);
            
        }
        public CommoSaveResult Update(LookupMasterModal modal)
        {
            return _IDataMaster.Update(modal);

        }
        public List<LookupMasterModal> Delete(int id) {
            return new List<LookupMasterModal>();
        }
    }
}
