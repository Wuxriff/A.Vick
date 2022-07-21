using A_Vick.Telegram.BL.Interfaces;
using System;
using Telegram.Bot;

namespace A_Vick.Telegram.BL
{
    public class TelegramBotContext : ITelegramBotContext
    {
        private TelegramBotClient? _botClient;

        public TelegramBotClient BotClient
        {
            get
            {
                if (_botClient != null)
                {
                    return _botClient;
                }

                throw new NullReferenceException("Failed to get TelegramBotClient");
            }
            set
            {
                _botClient = value;
            }
        }
    }
}