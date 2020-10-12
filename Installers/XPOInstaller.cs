using ExampleAPI.Models.ExampleXPOModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Installers
{
    public class XPOInstaller : IInstaller

    {

        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            // services.AddDxSampleModelJsonOptions();

            services
                .AddXpoDefaultUnitOfWork(true, options => options
                .UseConnectionString(configuration.GetConnectionString("InMemoryDataStore"))
                .UseThreadSafeDataLayer(true)
                .UseConnectionPool(false) // Remove this line if you use a database server like SQL Server, Oracle, PostgreSql, etc.                    
                .UseAutoCreationOption(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema) // Remove this line if the database already exists
                .UseEntityTypes(typeof(ExampleObject))); // Pass all of your persistent object types to this method.


        }
    }
}
