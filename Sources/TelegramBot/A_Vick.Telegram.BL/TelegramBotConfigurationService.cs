using A_Vick.Telegram.BL.Interfaces;
using A_Vick.Telegram.Models.Models;
using Microsoft.Extensions.Options;
using System;

namespace A_Vick.Telegram.BL
{
    public class TelegramBotConfigurationService : ITelegramBotConfigurationService
    {
        private readonly TelegramBotOptions _options;

        public TelegramBotConfigurationService(IOptions<TelegramBotOptions> options)
        {
            _options = options.Value;
        }

        public string GetApiToken()
        {
            if (string.IsNullOrWhiteSpace(_options.Token))
            {
                throw new NullReferenceException("Empty telegram bot token value");
            }

            return _options.Token;
        }
    }
}