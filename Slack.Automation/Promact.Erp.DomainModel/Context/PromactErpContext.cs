using Microsoft.AspNet.Identity.EntityFramework;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using System.Data.Entity;

namespace Promact.Erp.DomainModel.Context
{
    public class PromactErpContext : IdentityDbContext<ApplicationUser>
    {
        public PromactErpContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

        }
        /// <summary>
        /// Added table of LeaveRequest on Database PromactErpContext
        /// </summary>
        public DbSet<LeaveRequest> LeaveRequest { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Scrum> Scrum { get; set; }
        public DbSet<ScrumAnswer> ScrumAnswer { get; set; }
        public DbSet<TaskMailDetails> TaskMailDetails { get; set; }
        public DbSet<TaskMail> TaskMail { get; set; }
        public DbSet<SlackUserDetails> SlackUserDetails { get; set; }
        public DbSet<SlackChannelDetails> SlackChannelDetails { get; set; }
        public static PromactErpContext Create()
        {
            return new PromactErpContext();
        }
    }
}
