using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ExampleAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;

namespace ExampleAPI.Installers
{
    public class aspnetCoreInstaller:IInstaller

    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services
               .AddMvc(options =>
               {
                   options.EnableEndpointRouting = false;
                   options.Filters.Add<ValidationFilter>();
               })
               .AddFluentValidation(mvcConfiguration => mvcConfiguration.RegisterValidatorsFromAssemblyContaining<Startup>());

            // Add Cors            
            services.AddCors(options =>
            {                
                options.AddPolicy("NNCors", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().SetPreflightMaxAge(new System.TimeSpan(24, 0, 0)));
                options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().SetPreflightMaxAge(new System.TimeSpan(24, 0, 0)));
            });


            // Add Metrics
            var metrics = AppMetrics.CreateDefaultBuilder().Build();
            services.AddMetrics(metrics);
            services.AddMetricsTrackingMiddleware();
        }		

     
    }
}
