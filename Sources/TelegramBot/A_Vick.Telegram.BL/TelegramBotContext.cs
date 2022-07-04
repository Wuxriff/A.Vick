using A_Vick.Telegram.BL.Interfaces;
using System;
using Telegram.Bot;

namespace A_Vick.Telegram.BL
{
    public class TelegramBotContext : ITelegramBotContext
    {
        private readonly IMemoryCacheManager _memoryCacheManager;

        public TelegramBotContext(IMemoryCacheManager memoryCacheManager)
        {
            _memoryCacheManager = memoryCacheManager;
        }

        public TelegramBotClient BotClient
        {
            get
            {
                if (_memoryCacheManager.TryGet<TelegramBotClient>(nameof(TelegramBotClient), out var result))
                {
                    return result!;
                }

                throw new Exception("Failed to get TelegramBotClient");
            }
            set
            {
                _memoryCacheManager.AddOrReplace(nameof(TelegramBotClient), value);
            }
        }
    }
}