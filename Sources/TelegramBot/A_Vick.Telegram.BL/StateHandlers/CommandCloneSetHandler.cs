using A_Vick.Telegram.BL.Interfaces;
using A_Vick.Telegram.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace A_Vick.Telegram.BL.StateHandlers
{
    public class CommandCloneSetHandler : BaseStateHanlder
    {
        public CommandCloneSetHandler()
        {
            CurrentState = TelegramBotHandlerStates.CommandCloneSet_Start;
        }

        private const string WelcomeMessage = "Please, send any sticker from the sticker set you want to clone";
        private const string StickerCloneNameMessage = "Please, send name for your new sticker set";

        private const string ErrorStickerMessage = "Not a sticker, sorry. Please, send us a sticker!";
        private const string ErrorCloneNameMessage = "Not a name, sorry. Please, send us a correct sticker set name!";

        private const string AddStickersLinkTemplate = "https://t.me/addstickers/{0}";

        private long _userId;
        private string? _originalSetName;
        private string? _newSetName;

        public override async ValueTask<(string, BaseStateHanlder)> ProcessAsync(IServiceProvider services, Message message)
        {
            if (CurrentState == TelegramBotHandlerStates.CommandCloneSet_Start)
            {
                _userId = message.From!.Id;

                CurrentState = TelegramBotHandlerStates.CommandCloneSet_WaitingSticker;
                return (WelcomeMessage, this);
            }

            if (CurrentState == TelegramBotHandlerStates.CommandCloneSet_WaitingSticker)
            {
                var sticker = message.Sticker;

                if (sticker == null)
                {
                    return (ErrorStickerMessage, this);
                }

                _originalSetName = sticker.SetName;

                CurrentState = TelegramBotHandlerStates.CommandCloneSet_WaitingSetName;
                return (StickerCloneNameMessage, this);
            }

            if (CurrentState == TelegramBotHandlerStates.CommandCloneSet_WaitingSetName)
            {
                var setName = message.Text;

                if (!IsValidSetName(setName))
                {
                    return (ErrorCloneNameMessage, this);
                }

                _newSetName = setName;

                var service = services.GetRequiredService<ITelegramBotStickerService>();
                var generatedSetName = await service.CloneStickerSetAsync(_userId, _originalSetName!, _newSetName!);

                CurrentState = TelegramBotHandlerStates.None;
                return (string.Format(AddStickersLinkTemplate, generatedSetName), new IdleHandler());
            }

            return (string.Empty, new IdleHandler());
        }

        //TODO Add real validation
        private bool IsValidSetName(string? setName)
        {
            return true;
        }
    }
}