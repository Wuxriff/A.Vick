using A_Vick.Telegram.BL.Interfaces;
using A_Vick.Telegram.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace A_Vick.Telegram.BL
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ITelegramBotConfigurationService _configurationService;
        private readonly ITelegramBotContext _botContext;
        private readonly ITelegramBotMessageHandlerService _handlerService;

        private TelegramBotClient? Bot;

        public TelegramBotService(ITelegramBotConfigurationService configurationService, ITelegramBotContext botContext, ITelegramBotMessageHandlerService handlerService)
        {
            _configurationService = configurationService;
            _botContext = botContext;
            _handlerService = handlerService;
        }

        public void Init()
        {
            var apiToken = _configurationService.GetApiToken();

            _botContext.BotClient = Bot = new TelegramBotClient(apiToken);
        }

        public async ValueTask StartAsync(CancellationToken cancellationToken)
        {
            if (Bot == null)
                throw new InvalidOperationException("Telegram bot in not initialized. Call Init() method first!");

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
            };

            Bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);

            var commands = new List<BotCommand>
            {
                new BotCommand { Command = Constants.TelegramBotCommandCloneSet, Description = "Clones selected set" },
                new BotCommand { Command = Constants.TelegramBotCommandImportSet, Description = "Imports selected set to a existing one" },
                new BotCommand { Command = Constants.TelegramBotCommandAddStickerToSet, Description = "Imports selected sticker to a existing set" },
                new BotCommand { Command = Constants.TelegramBotCommandDeleteSet, Description = "Removes sticker set" },
                new BotCommand { Command = Constants.TelegramBotCommandDeleteStickerFromSet, Description = "Removes sticker from a set" },
            };

            await Bot.SetMyCommandsAsync(commands, cancellationToken: cancellationToken);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message!, cancellationToken),

                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            if (message.Type == MessageType.Text && message.Text == Constants.TelegramBotCommandStart)
            {
                var textMessage = string.Format("\U0001F44B Hello {0}, welcome to A.Vick system. Please, use commands menu to perform actions.", message.From!.Username);
                await WriteChatMessage(message.Chat.Id, textMessage, cancellationToken);

                return;
            }

            var resultMessage = await _handlerService.HandleMessageAsync(message);

            await WriteChatMessage(message.Chat.Id, resultMessage, cancellationToken);
        }

        private Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private Task WriteChatMessage(long chatId, string text, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(text))
                return Task.CompletedTask;

            return Bot!.SendTextMessageAsync(chatId, text, cancellationToken: cancellationToken);
        }
    }
}