using Microsoft.AspNet.Identity.EntityFramework;
using Promact.Erp.DomainModel.Models;

namespace Promact.Erp.DomainModel.Context
{
    public class PromactErpContext : IdentityDbContext<ApplicationUser>
    {
        public PromactErpContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

        }

        public static PromactErpContext Create()
        {
            return new PromactErpContext();
        }
    }
}
