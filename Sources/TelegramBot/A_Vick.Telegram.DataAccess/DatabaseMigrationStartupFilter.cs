using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace A_Vick.Telegram.DataAccess
{
    internal class AutomaticDatabaseMigrationStartupFilter<T> : IStartupFilter where T : DbContext
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                using (var scope = builder.ApplicationServices.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;

                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    var useMigration = configuration.GetValue<bool>("AutomigrateDatabase");

                    if (useMigration)
                    {
                        var context = serviceProvider.GetRequiredService<T>();
                        var logger = serviceProvider.GetRequiredService<ILogger<AutomaticDatabaseMigrationStartupFilter<T>>>();

                        try
                        {
                            context.Database.SetCommandTimeout(TimeSpan.FromMinutes(1));
                            context.Database.Migrate();
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to process {ContextName} context migration", typeof(T).Name);
                            throw;
                        }
                    }
                }
                next(builder);
            };
        }
    }

    internal class AutomaticDatabaseMigrationStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                using (var scope = builder.ApplicationServices.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;

                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    var useMigration = configuration.GetValue<bool>("AutomigrateDatabase");
                    var logger = serviceProvider.GetRequiredService<ILogger<AutomaticDatabaseMigrationStartupFilter>>();

                    if (useMigration)
                    {
                        logger.LogInformation($"AutomigrateDatabase option is enabled, starting automatic migrations");
                        var contexts = serviceProvider.GetRequiredService<IEnumerable<DbContext>>();

                        foreach (var context in contexts)
                        {
                            try
                            {
                                context.Database.SetCommandTimeout(TimeSpan.FromMinutes(1));
                                context.Database.Migrate();
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Failed to process {ContextName} context migration", context.GetType().Name);
                                throw;
                            }
                        }
                    }
                    else
                    {
                        logger.LogInformation($"AutomigrateDatabase option is not set, skipping automatic migrations");
                    }
                }
                next(builder);
            };
        }
    }
}
