using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.ScrumRepository
{
    public interface IScrumBotRepository
    {
        //  void InitiateScrum(ScrumAnswer ScrumAnswer);

        Task<string> StartScrum(string GroupName);
     //   string AddAnswer(string userName, string message,string groupName);
    }
}
