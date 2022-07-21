using A_Vick.Telegram.BL.Interfaces;
using A_Vick.Telegram.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace A_Vick.Telegram.BL.StateHandlers
{
    public class CommandAddStickerToSetHandler : BaseStateHandler
    {
        public CommandAddStickerToSetHandler()
        {
            CurrentState = TelegramBotHandlerStates.CommandAddStickerToSet_Start;
        }

        private const string WelcomeMessage = "Please, send a sticker from the sticker set you want to copy";
        private const string StickerSetMessage = "Please, send a sticker from the sticker set to which you want to add a sticker";
        private const string AddStickerToSet = "Sticker has been added";

        private const string ErrorStickerMessage = "Not a sticker, sorry. Please, send us a sticker!";

        private long _userId;
        private Sticker? _sticker;
        private string? _setName;

        public override async ValueTask<(string Message, BaseStateHandler Handler)> ProcessAsync(IServiceProvider services, Message message)
        {
            if (CurrentState == TelegramBotHandlerStates.CommandAddStickerToSet_Start)
            {
                _userId = message.From!.Id;

                CurrentState = TelegramBotHandlerStates.CommandAddStickerToSet_WaitingSticker;
                return (WelcomeMessage, this);
            }

            //TODO Add step to validate and replace dupe emoji association 

            if (CurrentState == TelegramBotHandlerStates.CommandAddStickerToSet_WaitingSticker)
            {
                var sticker = message.Sticker;

                if (sticker == null)
                {
                    return (ErrorStickerMessage, this);
                }

                _sticker = sticker;

                CurrentState = TelegramBotHandlerStates.CommandAddStickerToSet_WaitingStickerSet;
                return (StickerSetMessage, this);
            }

            if (CurrentState == TelegramBotHandlerStates.CommandAddStickerToSet_WaitingStickerSet)
            {
                var sticker = message.Sticker;

                if (sticker == null)
                {
                    return (ErrorStickerMessage, this);
                }

                _setName = sticker.SetName;

                var service = services.GetRequiredService<ITelegramBotStickerService>();
                await service.AddStickerToStickerSetAsync(_userId, _setName!,_sticker!);

                CurrentState = TelegramBotHandlerStates.None;
                return (AddStickerToSet, new IdleHandler());
            }

            return (string.Empty, new IdleHandler());
        }
    }
}
