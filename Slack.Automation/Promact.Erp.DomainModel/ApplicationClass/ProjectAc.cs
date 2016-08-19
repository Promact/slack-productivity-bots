
namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class ProjectAc
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SlackChannelName { get; set; }
        public bool IsActive { get; set; }
        public string TeamLeaderId { get; set; }
    }
}
