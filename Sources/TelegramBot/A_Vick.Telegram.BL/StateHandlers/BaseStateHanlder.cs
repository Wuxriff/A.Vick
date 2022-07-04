using A_Vick.Telegram.Models.Enums;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace A_Vick.Telegram.BL.StateHandlers
{
    public abstract class BaseStateHanlder
    {
        public TelegramBotHandlerStates CurrentState { get; protected set; }
        public abstract ValueTask<(string Message, BaseStateHanlder Handler)> ProcessAsync(IServiceProvider services, Message message);
    }
}