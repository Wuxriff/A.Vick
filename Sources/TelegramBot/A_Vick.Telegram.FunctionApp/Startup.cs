using A_Vick.Telegram.DataAccess.Extensions;
using A_Vick.Telegram.FunctionApp;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace A_Vick.Telegram.FunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddScoped<Service>();

            builder.Services.AddDbContexts();
            //builder.Services.AddDbContextMigration<DataContext>();
            // https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#from-a-design-time-factory
            // https://dev.to/azure/using-entity-framework-with-azure-functions-50aa
        }
    }
}
