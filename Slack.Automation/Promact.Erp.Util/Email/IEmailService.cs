using Promact.Erp.DomainModel.ApplicationClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.Util.Email
{
    public interface IEmailService
    {
        void Send(EmailApplication email);
    }
}
