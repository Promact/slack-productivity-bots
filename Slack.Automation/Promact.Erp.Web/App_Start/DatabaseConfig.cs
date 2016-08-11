using Autofac;
using Promact.Erp.DomainModel.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Promact.Erp.Web.App_Start
{
    public static class DatabaseConfig
    {
        /// <summary>
        /// Method to enable auto-migration and update Database after migration
        /// </summary>
        /// <param name="componentContext"></param>
        public static void Initialize(IComponentContext componentContext)
        {
            Database.SetInitializer<PromactErpContext>(new MigrateDatabaseToLatestVersion<PromactErpContext, Promact.Erp.DomainModel.Migrations.Configuration>());

            using (var dbContext = componentContext.Resolve<DbContext>())
            {
                dbContext.Database.Initialize(false);
            }
        }
    }
}