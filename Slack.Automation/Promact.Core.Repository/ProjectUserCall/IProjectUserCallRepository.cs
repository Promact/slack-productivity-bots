using Promact.Erp.DomainModel.ApplicationClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ProjectUserCall
{ 
    public interface IProjectUserCallRepository
    {
        User GetUserByUsername(string userName);
        List<string> GetTeamLeaderUserName(string userName);
        List<string> GetManagementUserName();
    }
}
