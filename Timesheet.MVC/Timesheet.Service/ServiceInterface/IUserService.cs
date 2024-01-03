using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Service.ServiceInterface
{
    public interface IUserService
    {
        string UserRolesGet(string userName);
    }
}
