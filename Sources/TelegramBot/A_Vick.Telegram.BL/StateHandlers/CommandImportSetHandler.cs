using A_Vick.Telegram.BL.Interfaces;
using A_Vick.Telegram.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace A_Vick.Telegram.BL.StateHandlers
{
    public class CommandImportSetHandler : BaseStateHandler
    {
        public CommandImportSetHandler()
        {
            CurrentState = TelegramBotHandlerStates.CommandImportSet_Start;
        }

        private const string WelcomeMessage = "Please, send any sticker from the sticker set you want to import";
        private const string StickerSetMessage = "Please, send a sticker from the sticker set to which you want to import a sticker set";
        private const string ImportStickerSetToSet = "Sticker set has been imported";

        private const string ErrorStickerMessage = "Not a sticker, sorry. Please, send us a sticker!";

        private string? _sourceSetName;
        private string? _importTosetName;

        public override async ValueTask<(string Message, BaseStateHandler Handler)> ProcessAsync(IServiceProvider services, Message message)
        {
            if (CurrentState == TelegramBotHandlerStates.CommandImportSet_Start)
            {
                CurrentState = TelegramBotHandlerStates.CommandImportSet_WaitingSticker;
                return (WelcomeMessage, this);
            }

            //TODO Add step to validate and replace dupe emoji association 

            if (CurrentState == TelegramBotHandlerStates.CommandImportSet_WaitingSticker)
            {
                var sticker = message.Sticker;

                if (sticker == null)
                {
                    return (ErrorStickerMessage, this);
                }

                _sourceSetName = sticker.SetName;

                CurrentState = TelegramBotHandlerStates.CommandImportSet_WaitingStickerSet;
                return (StickerSetMessage, this);
            }

            if (CurrentState == TelegramBotHandlerStates.CommandImportSet_WaitingStickerSet)
            {
                var sticker = message.Sticker;

                if (sticker == null)
                {
                    return (ErrorStickerMessage, this);
                }

                _importTosetName = sticker.SetName;

                var service = services.GetRequiredService<ITelegramBotStickerService>();
                await service.ImportStickerSetAsync(message.From!.Id, _sourceSetName!, _importTosetName!);

                CurrentState = TelegramBotHandlerStates.None;
                return (ImportStickerSetToSet, new IdleHandler());
            }

            return (string.Empty, new IdleHandler());
        }
    }
}
