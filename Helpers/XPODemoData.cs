using DevExpress.Xpo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Helpers
{
    public static class XPODemoData
    {
        public static IApplicationBuilder UseXpoDemoData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                UnitOfWork uow = scope.ServiceProvider.GetService<UnitOfWork>();
                SeedDataHelper.Seed(uow);
            }
            return app;
        }
    }
}
