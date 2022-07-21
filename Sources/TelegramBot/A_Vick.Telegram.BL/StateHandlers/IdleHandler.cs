using A_Vick.Telegram.Models.Enums;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace A_Vick.Telegram.BL.StateHandlers
{
    public class IdleHandler : BaseStateHandler
    {
        private const string WelcomeMessage = "Please, use commands menu to perform actions";
        public IdleHandler()
        {
            CurrentState = TelegramBotHandlerStates.Idle;
        }

        public override ValueTask<(string, BaseStateHandler)> ProcessAsync(IServiceProvider services, Message message)
        {
            return new ValueTask<(string, BaseStateHandler)>((WelcomeMessage, this));
        }
    }
}