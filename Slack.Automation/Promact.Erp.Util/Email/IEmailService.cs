using Promact.Erp.DomainModel.ApplicationClass;

namespace Promact.Erp.Util.Email
{
    public interface IEmailService
    {
        void Send(EmailApplication email);
    }
}
