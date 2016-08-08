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
        Task<User> GetUserByUsername(string userName);
        Task<List<ProjectUserDetailsApplicationClass>> GetTeamLeaderUserName(string userName);
        Task<List<ProjectUserDetailsApplicationClass>> GetManagementUserName();
    }
}
