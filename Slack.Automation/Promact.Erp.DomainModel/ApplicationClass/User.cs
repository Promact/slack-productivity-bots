namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class User
    {
        /// <summary>
        /// EmployeeId
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Employee Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Employee FirstName or Slack UserName
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Employee LastName
        /// </summary>
        public string LastName { get; set; }
    }
}
