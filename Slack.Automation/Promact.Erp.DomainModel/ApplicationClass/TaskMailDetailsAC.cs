namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class TaskMailDetailsAC
    {
        /// <summary>
        /// Primary key Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Description of task mail
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Number of hours send in a single task
        /// </summary>
        public decimal Hours { get; set; }

        /// <summary>
        /// Comment or remark or roadblock in task
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Status of task completed or inprogress or roadblock
        /// </summary>
        public string Status { get; set; }
    }
}
