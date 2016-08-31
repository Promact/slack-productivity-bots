
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [Required]
        [StringLength(255)]
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }

        public string Role { get; set; }

        [Required]
        [EmailAddress]
        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("UserName")]
        public string UserName { get; set; }

        [JsonProperty("NumberOfCasualLeave")]
        public double NumberOfCasualLeave { get; set; }

        [JsonProperty("NumberOfSickLeave")]
        public double NumberOfSickLeave { get; set; }

        [JsonProperty("UniqueName")]
        public string UniqueName { get { return FirstName + "-" + Email; } }

        public string SlackUserName { get; set; }
    }
}
