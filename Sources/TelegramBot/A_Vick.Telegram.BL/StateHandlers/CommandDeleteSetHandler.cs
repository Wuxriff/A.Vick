using A_Vick.Telegram.BL.Interfaces;
using A_Vick.Telegram.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace A_Vick.Telegram.BL.StateHandlers
{
    public class CommandDeleteSetHandler : BaseStateHanlder
    {
        public CommandDeleteSetHandler()
        {
            CurrentState = TelegramBotHandlerStates.CommandDeleteSet_Start;
        }

        private const string WelcomeMessage = "Please, send any sticker from the sticker set which you want to delete";
        private const string ApprovalMessageTemplate = "Are you sure you want to delete this ({0}) sticker set? Send 'yes' if you approve this action";
        private const string CancelMessage = "Delete operation cancelled";

        private const string ErrorStickerMessage = "Not a sticker, sorry. Please, send us a sticker!";
        private const string ErrorTextApprovalMessage = "Not a text, sorry. Delete operation cancelled";

        private const string DeleteStickerSetTemplate = "Sticker set ({0}) has been deleted";

        private string? _originalSetName;

        public override async ValueTask<(string Message, BaseStateHanlder Handler)> ProcessAsync(IServiceProvider services, Message message)
        {
            if (CurrentState == TelegramBotHandlerStates.CommandDeleteSet_Start)
            {
                CurrentState = TelegramBotHandlerStates.CommandDeleteSet_WaitingSticker;
                return (WelcomeMessage, this);
            }

            if (CurrentState == TelegramBotHandlerStates.CommandDeleteSet_WaitingSticker)
            {
                var sticker = message.Sticker;

                if (sticker == null)
                {
                    return (ErrorStickerMessage, this);
                }

                _originalSetName = sticker.SetName;

                CurrentState = TelegramBotHandlerStates.CommandDeleteSet_WaitingApproval;
                return (string.Format(ApprovalMessageTemplate, _originalSetName), this);
            }

            if (CurrentState == TelegramBotHandlerStates.CommandDeleteSet_WaitingApproval)
            {
                if (string.IsNullOrWhiteSpace(message.Text))
                {
                    CurrentState = TelegramBotHandlerStates.None;
                    return (ErrorTextApprovalMessage, new IdleHandler());
                }

                if(!string.Equals(message.Text, "yes", StringComparison.OrdinalIgnoreCase))
                {
                    CurrentState = TelegramBotHandlerStates.None;
                    return (CancelMessage, new IdleHandler());
                }

                var service = services.GetRequiredService<ITelegramBotStickerService>();
                await service.DeleteStickerSetAsync(_originalSetName!);

                return (string.Format(DeleteStickerSetTemplate, _originalSetName), new IdleHandler());
            }

            return (string.Empty, new IdleHandler());
        }
    }
}