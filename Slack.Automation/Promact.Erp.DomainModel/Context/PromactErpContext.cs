using Microsoft.AspNet.Identity.EntityFramework;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
using System.Data.Common;
using System.Data.Entity;

namespace Promact.Erp.DomainModel.Context
{
    public class PromactErpContext : IdentityDbContext<ApplicationUser>
    {
        public PromactErpContext()
            : base("MS_TableConnectionString", throwIfV1Schema: false)
        {

        }
        public PromactErpContext(DbConnection dbConnection) : base(dbConnection, true)
        {

        }
        /// <summary>
        /// Added table of LeaveRequest on Database PromactErpContext
        /// </summary>
        public DbSet<LeaveRequest> LeaveRequest { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Scrum> Scrum { get; set; }
        public DbSet<TemporaryScrumDetails> TemporaryScrumDetails { get; set; }
        public DbSet<ScrumAnswer> ScrumAnswer { get; set; }
        public DbSet<TaskMailDetails> TaskMailDetails { get; set; }
        public DbSet<TaskMail> TaskMail { get; set; }
        public DbSet<SlackUserDetails> SlackUserDetails { get; set; }
        public DbSet<SlackChannelDetails> SlackChannelDetails { get; set; }
        public DbSet<IncomingWebHook> IncomingWebHook { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<GroupEmailMapping> GroupEmailMapping { get; set; }
        public DbSet<MailSetting> MailSetting { get; set; }
        public DbSet<MailSettingMapping> MailSettingMapping { get; set; }
        
        public static PromactErpContext Create()
        {
            return new PromactErpContext();
        }
    }
}
