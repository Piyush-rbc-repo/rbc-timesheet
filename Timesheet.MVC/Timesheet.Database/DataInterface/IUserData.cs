using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Database.DataInterface
{
    public interface IUserData
    {
        string UserRolesGet(string userName);
    }
}
