using Autofac;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class AttachmentRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IAttachmentRepository _attachmentRepository;
        public AttachmentRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _attachmentRepository = _componentContext.Resolve<IAttachmentRepository>();
        }

        /// <summary>
        /// Test case to check creating attchment of slack used generically
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackResponseAttachment()
        {
            string hello = "Hello";
            var response = _attachmentRepository.SlackResponseAttachment("1", hello).Last();
            Assert.Equal(response.Title, hello);
            Assert.Equal(response.Color, StringConstant.Color);
        }

        /// <summary>
        /// Test case to check creating text corresponding to leave details and user which is to be send on slack as reply
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void ReplyText()
        {
            LeaveRequest leave = new LeaveRequest();
            leave.Reason = "testing";
            var response = _attachmentRepository.ReplyText("siddhartha",leave);
            Assert.NotEqual(response, null);
        }

        /// <summary>
        /// Test case to check break string by spaces only if spaces are not between quotes
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlackText()
        {
            string hello = "Hello All";
            var response = _attachmentRepository.SlackText(hello).Last();
            Assert.Equal(response, "All");
        }

        /// <summary>
        /// Test cases to check transform NameValueCollection to SlashCommand class
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void SlashCommandTransfrom()
        {
            NameValueCollection value = new NameValueCollection();
            var response = _attachmentRepository.SlashCommandTransfrom(value);
            Assert.Equal(response.ChannelName, null);
        }
    }
}
