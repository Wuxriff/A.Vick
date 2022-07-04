using A_Vick.Telegram.BL.Interfaces;
using A_Vick.Telegram.BL.StateHandlers;
using A_Vick.Telegram.Shared;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace A_Vick.Telegram.BL
{
    public class TelegramBotMessageHandlerService : ITelegramBotMessageHandlerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITelegramBotHandlerManagerService _handlerManagerService;

        public TelegramBotMessageHandlerService(IServiceProvider serviceProvider, ITelegramBotHandlerManagerService handlerManagerService)
        {
            _serviceProvider = serviceProvider;
            _handlerManagerService = handlerManagerService;
        }

        public async ValueTask<string> HandleMessageAsync(Message message)
        {
            if (message.From == null)
            {
                throw new InvalidOperationException("This bot cannot work in channles or groups, private messages only");
            }

            var userId = message.From.Id;

            if (!TryGetCommandHandler(message.Text, out var handler) && !_handlerManagerService.TryGetHandler(userId, out handler))
            {
                handler = new IdleHandler();
            }

            var result = await handler.ProcessAsync(_serviceProvider, message);
            _handlerManagerService.AddOrUpdateHandler(userId, result.Handler);

            return result.Message;
        }

        private static bool TryGetCommandHandler(string? command, out BaseStateHanlder? handler)
        {
            if (command == Constants.TelegramBotCommandCloneSet)
            {
                handler = new CommandCloneSetHandler();
                return true;
            }

            if (command == Constants.TelegramBotCommandImportSet)
            {
                handler = new CommandImportSetHandler();
                return true;
            }

            if (command == Constants.TelegramBotCommandAddStickerToSet)
            {
                handler = new CommandAddStickerToSetHandler();
                return true;
            }

            if (command == Constants.TelegramBotCommandDeleteSet)
            {
                handler = new CommandDeleteSetHandler();
                return true;
            }

            if (command == Constants.TelegramBotCommandDeleteStickerFromSet)
            {
                handler = new CommandDeleteStickerFromSetHandler();
                return true;
            }

            handler = null;

            return false;
        }
    }
}