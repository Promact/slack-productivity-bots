using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class TaskMailDetailReportAc
    {
        public TaskMailDetailReportAc(string description,string comment,int id=0, decimal hours=0,int totalItems=0, TaskMailStatus ? status=null)
        {
            Id = id;
            Description = description;
            Comment = comment;
            Hours = hours;
            TotalItems = totalItems;
            Status  = status.Value;
        }
        public int Id { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public decimal Hours { get; set; }
        public int TotalItems { get; set;}
        public TaskMailStatus Status { get; set; }

        
    }
}
