using Telegram.Bot;

namespace A_Vick.Telegram.BL.Interfaces
{
    public interface ITelegramBotContext
    {
        TelegramBotClient BotClient { get; set; }
    }
}