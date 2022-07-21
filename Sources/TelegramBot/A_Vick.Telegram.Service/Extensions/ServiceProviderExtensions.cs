using A_Vick.Telegram.BL;
using A_Vick.Telegram.BL.Interfaces;
using A_Vick.Telegram.Service.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace A_Vick.Telegram.Service.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void AddHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<TelegramBotHostedService>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ITelegramBotService, TelegramBotService>();
            services.AddScoped<ITelegramBotStickerService, TelegramBotStickerService>();
            services.AddSingleton<IMemoryCacheManager, MemoryCacheManager>();
            services.AddSingleton<ITelegramBotConfigurationService, TelegramBotConfigurationService>();
            services.AddSingleton<ITelegramBotContext, TelegramBotContext>();
            services.AddSingleton<ITelegramBotHandlerManagerService, TelegramBotHandlerManagerService>();
            services.AddScoped<ITelegramBotMessageHandlerService, TelegramBotMessageHandlerService>();
        }

        public static void AddOption<T>(this IServiceCollection services, IConfiguration configuration) where T : class
        {
            services.Configure<T>(configuration.GetSection(typeof(T).Name));
        }
    }
}