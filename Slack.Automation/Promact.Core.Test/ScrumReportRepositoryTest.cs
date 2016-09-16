using Autofac;
using Moq;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ScrumReportRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Test
{
    public class ScrumReportRepositoryTest
    {
        private IComponentContext _componentContext;
        private IScrumReportRepository _scrumReportRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;

        public ScrumReportRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _scrumReportRepository = _componentContext.Resolve<IScrumReportRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
        }


    }
}
