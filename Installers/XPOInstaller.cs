using ExampleAPI.Models.ExampleXPOModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public int Order => 10;

        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            // services.AddDxSampleModelJsonOptions();
            var entities = GetClasses("ExampleAPI", "ExampleAPI.Models.ExampleXPOModel"); //Todo: Update Where the Data is
            services
                .AddXpoDefaultUnitOfWork(true, options => options
                .UseConnectionString(configuration.GetConnectionString("InMemoryDataStore")) //TODO: Update Databse Connection string
                .UseThreadSafeDataLayer(true)
                .UseConnectionPool(false) //TODO: Remove this line if you use a database server like SQL Server, Oracle, PostgreSql, etc.
                .UseAutoCreationOption(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema) // Remove this line if the database already exists
                .UseEntityTypes(entities)); // Pass all of your persistent object types to this method.


        }
        static Type[] GetClasses(string AssemblyName, string Namespace = "")
        {
            var asm2 = Assembly.Load(AssemblyName);
            Type[] allTypes;
            try
            {
                allTypes = asm2.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                _logger.Warn(ex, "Some types could not be loaded from assembly '{0}'. Skipping unloadable types.", AssemblyName);
                allTypes = ex.Types.Where(t => t != null).ToArray();
            }

            List<Type> types;
            if (string.IsNullOrEmpty(Namespace))
            {
                types = allTypes.Where(t => t.IsClass).ToList();
            }
            else
            {
                types = allTypes.Where(t => t.IsClass && t.Namespace == Namespace).ToList();
            }
            for (int i = types.Count() - 1; i >= 0; i--) { if (!types[i].IsAssignableTo(typeof(DevExpress.Xpo.PersistentBase))) { types.RemoveAt(i); } }
            return types.ToArray();
        }
    }
}
