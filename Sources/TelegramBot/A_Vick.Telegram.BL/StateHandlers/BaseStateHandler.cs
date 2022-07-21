using A_Vick.Telegram.Models.Enums;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace A_Vick.Telegram.BL.StateHandlers
{
    public abstract class BaseStateHandler
    {
        protected TelegramBotHandlerStates CurrentState { get; set; }
        public abstract ValueTask<(string Message, BaseStateHandler Handler)> ProcessAsync(IServiceProvider services, Message message);
    }
}