using A_Vick.Telegram.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace A_Vick.Telegram.DataAccess.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void AddDbContexts(this IServiceCollection services)
        {
            services.AddEntityFrameworkSqlite().AddDbContext<DataContext>(options => options.UseSqlite(Constants.DatabaseName));
        }

        #region AddDbContextMigration

        public static IServiceCollection AddDbContextMigration<T>(this IServiceCollection services) where T : DbContext
        {
            return services.AddTransient<IStartupFilter, AutomaticDatabaseMigrationStartupFilter<T>>();
        }

        public static IServiceCollection AddDbContextMigrations(this IServiceCollection services)
        {
            return services.AddTransient<IStartupFilter, AutomaticDatabaseMigrationStartupFilter>();
        }

        #endregion
    }
}