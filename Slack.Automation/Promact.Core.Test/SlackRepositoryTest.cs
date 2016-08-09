using Autofac;
using Promact.Core.Repository.SlackRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class SlackRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ISlackRepository _slackRepository;
        public SlackRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _slackRepository = _componentContext.Resolve<ISlackRepository>();
        }

        [Fact]
        public void LeaveList()
        {
            var leaves = 
            Assert.Equal(68, leaves.Count());
        }
    }
}
