using A_Vick.Telegram.BL.StateHandlers;

namespace A_Vick.Telegram.BL.Interfaces
{
    public interface ITelegramBotHandlerManagerService
    {
        void AddOrUpdateHandler(long userId, BaseStateHandler handler);
        public bool TryGetHandler(long userId, out BaseStateHandler? handler);
    }
}