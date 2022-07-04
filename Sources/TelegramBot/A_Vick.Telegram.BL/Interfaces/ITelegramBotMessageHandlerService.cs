using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace A_Vick.Telegram.BL.Interfaces
{
    public interface ITelegramBotMessageHandlerService
    {
        public ValueTask<string> HandleMessageAsync(Message message);
    }
}