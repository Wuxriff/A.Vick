using A_Vick.Telegram.BL.Interfaces;
using A_Vick.Telegram.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace A_Vick.Telegram.BL.StateHandlers
{
    public class CommandDeleteStickerFromSetHandler : BaseStateHandler
    {
        public CommandDeleteStickerFromSetHandler()
        {
            CurrentState = TelegramBotHandlerStates.CommandDeleteStickerFromSet_Start;
        }

        private const string WelcomeMessage = "Please, send a sticker from the sticker set you want to delete";
        private const string ApprovalMessage = "Are you sure you want to delete this sticker? Send 'yes' if you approve this action";
        private const string CancelMessage = "Delete operation cancelled";
        private const string DeleteSticker = "Sticker has been deleted";

        private const string ErrorStickerMessage = "Not a sticker, sorry. Please, send us a sticker!";
        private const string ErrorTextApprovalMessage = "Not a text, sorry. Delete operation cancelled";

        private Sticker? _sticker;

        public override async ValueTask<(string Message, BaseStateHandler Handler)> ProcessAsync(IServiceProvider services, Message message)
        {
            if (CurrentState == TelegramBotHandlerStates.CommandDeleteStickerFromSet_Start)
            {
                CurrentState = TelegramBotHandlerStates.CommandDeleteStickerFromSet_WaitingSticker;
                return (WelcomeMessage, this);
            }

            if (CurrentState == TelegramBotHandlerStates.CommandDeleteStickerFromSet_WaitingSticker)
            {
                var sticker = message.Sticker;

                if (sticker == null)
                {
                    return (ErrorStickerMessage, this);
                }

                _sticker = sticker;

                CurrentState = TelegramBotHandlerStates.CommandDeleteStickerFromSet_WaitingApproval;
                return (ApprovalMessage, this);
            }

            if (CurrentState == TelegramBotHandlerStates.CommandDeleteStickerFromSet_WaitingApproval)
            {
                if (string.IsNullOrWhiteSpace(message.Text))
                {
                    CurrentState = TelegramBotHandlerStates.None;
                    return (ErrorTextApprovalMessage, new IdleHandler());
                }

                if (!string.Equals(message.Text, "yes", StringComparison.OrdinalIgnoreCase))
                {
                    CurrentState = TelegramBotHandlerStates.None;
                    return (CancelMessage, new IdleHandler());
                }

                var service = services.GetRequiredService<ITelegramBotStickerService>();
                await service.DeleteStickerAsync(_sticker!);

                return (DeleteSticker, new IdleHandler());
            }

            return (string.Empty, new IdleHandler());
        }
    }
}
