using A_Vick.Telegram.BL.Interfaces;
using A_Vick.Telegram.BL.StateHandlers;
using A_Vick.Telegram.Shared;
using System;

namespace A_Vick.Telegram.BL
{
    public class TelegramBotHandlerManagerService : ITelegramBotHandlerManagerService
    {
        private readonly IMemoryCacheManager _memoryCacheManager;

        private static readonly TimeSpan _lifeTime = new(0, 5, 0);

        public TelegramBotHandlerManagerService(IMemoryCacheManager memoryCacheManager)
        {
            _memoryCacheManager = memoryCacheManager;
        }

        public void AddOrUpdateHandler(long userId, BaseStateHanlder hanlder)
        {
            _memoryCacheManager.AddOrReplace(GetCacheKey(userId), hanlder, _lifeTime);
        }

        public bool TryGetHandler(long userId, out BaseStateHanlder? handler)
        {
            return _memoryCacheManager.TryGet(GetCacheKey(userId), out handler);
        }

        private string GetCacheKey(long userId)
        {
            return _memoryCacheManager.GetCacheKey(Constants.TelegramBotStateUser, userId);
        }
    }
}