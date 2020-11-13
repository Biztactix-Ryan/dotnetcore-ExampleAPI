using ExampleAPI.Models.ExampleXPOModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
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
                .UseEntityTypes(GetClasses("ExampleAPI", "ExampleAPI.Models.ExampleXPOModel"))); // Pass all of your persistent object types to this method.


        }
        static Type[] GetClasses(string AssemblyName, string Namespace = "")
        {
            var asm2 = Assembly.Load(AssemblyName);
            IEnumerable<Type> types;
            if (string.IsNullOrEmpty(Namespace))
            {
                types = asm2.GetTypes().Where(t => t.IsClass);
            }
            else
            {
                types = asm2.GetTypes().Where(t => t.IsClass && t.Namespace == Namespace);
            }
            if (types != null) { return types.ToArray(); }
            return null;
        }
    }
}
