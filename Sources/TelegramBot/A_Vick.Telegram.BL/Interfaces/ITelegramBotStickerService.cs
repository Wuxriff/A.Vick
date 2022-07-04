using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace A_Vick.Telegram.BL.Interfaces
{
    public interface ITelegramBotStickerService
    {
        ValueTask<string> CloneStickerSetAsync(long userId, string setNameToClone, string newSetName);
        ValueTask ImportStickerSetAsync(long userId, string setNameToImport, string setName);
        ValueTask AddStickerToStickerSetAsync(long userId, string setName, Sticker sticker);
        ValueTask DeleteStickerSetAsync(string setName);
        ValueTask DeleteStickerAsync(Sticker sticker);
    }
}