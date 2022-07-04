using A_Vick.Telegram.BL.Interfaces;
using A_Vick.Telegram.Shared;
using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace A_Vick.Telegram.BL
{
    public class TelegramBotStickerService : ITelegramBotStickerService
    {
        private readonly ITelegramBotContext _botContext;
        private readonly IMemoryCacheManager _memoryCacheManager;

        public TelegramBotStickerService(ITelegramBotContext botContext, IMemoryCacheManager memoryCacheManager)
        {
            _botContext = botContext;
            _memoryCacheManager = memoryCacheManager;
        }

        public async ValueTask<string> CloneStickerSetAsync(long userId, string setNameToClone, string newSetName)
        {
            var originalSet = await _botContext.BotClient.GetStickerSetAsync(setNameToClone);
            var generatedSetName = await GetStickerSetName(newSetName);

            var firstSticker = originalSet.Stickers[0];

            await CreateStickerSetHanlder(firstSticker.IsAnimated, firstSticker.IsVideo, userId, generatedSetName, newSetName, new InputOnlineFile(firstSticker.FileId), firstSticker.Emoji!);

            if (originalSet.Stickers.Length != 1)
            {
                for (int i = 1; i < originalSet.Stickers.Length; i++)
                {
                    var sticker = originalSet.Stickers[i];

                    await AddToStickerSetHanlder(sticker.IsAnimated, sticker.IsVideo, userId, generatedSetName, new InputOnlineFile(sticker.FileId), sticker.Emoji!);
                }
            }

            return generatedSetName;
        }

        public async ValueTask ImportStickerSetAsync(long userId, string setNameToImport, string setName)
        {
            var originalSet = await _botContext.BotClient.GetStickerSetAsync(setNameToImport);

            var firstSticker = originalSet.Stickers[0];

            ValidateAddStickerOperation(originalSet, firstSticker);

            for (int i = 0; i < originalSet.Stickers.Length; i++)
            {
                var sticker = originalSet.Stickers[i];

                await AddToStickerSetHanlder(sticker.IsAnimated, sticker.IsVideo, userId, setName, new InputOnlineFile(sticker.FileId), sticker.Emoji!);
            }
        }

        public async ValueTask AddStickerToStickerSetAsync(long userId, string setName, Sticker sticker)
        {
            var originalSet = await _botContext.BotClient.GetStickerSetAsync(sticker.SetName!);

            ValidateAddStickerOperation(originalSet, sticker);

            var generatedSetName = await GetStickerSetName(setName);

            await AddToStickerSetHanlder(sticker.IsAnimated, sticker.IsVideo, userId, generatedSetName, new InputOnlineFile(sticker.FileId), sticker.Emoji!);
        }

        public async ValueTask DeleteStickerSetAsync(string setName)
        {
            var generatedSetName = await GetStickerSetName(setName);

            var originalSet = await _botContext.BotClient.GetStickerSetAsync(generatedSetName);

            foreach (var sticker in originalSet.Stickers)
            {
                await DeleteStickerFromSet(sticker.FileId);
            }
        }

        public async ValueTask DeleteStickerAsync(Sticker sticker)
        {
            await DeleteStickerFromSet(sticker.FileId);
        }

        #region Help methods

        private async ValueTask<string> GetStickerSetName(string setName)
        {
            var botName = await _memoryCacheManager.GetOrAddAsync(Constants.TelegramBotUserName, async () => (await _botContext.BotClient.GetMeAsync()).Username);

            return setName.Contains($"_by_")
                ? setName
                : $"{setName}_by_{botName}";
        }

        private async ValueTask<Stream> DownloadFileToStream(string fileId)
        {
            var stream = new MemoryStream();

            var file = await _botContext.BotClient.GetFileAsync(fileId);
            await _botContext.BotClient.DownloadFileAsync(file.FilePath!, stream);

            stream.Position = 0;
            return stream;
        }

        private static void ValidateAddStickerOperation(StickerSet stickerSet, Sticker sticker)
        {
            if (stickerSet.IsAnimated == sticker.IsAnimated && stickerSet.IsVideo == sticker.IsVideo)
                return;

            throw new InvalidOperationException("This sticker cannot be added to this sticker set due to it's type mismatch");
        }

        #endregion

        #region Create sets

        private async ValueTask CreateStickerSetHanlder(bool isAnimated, bool isVideo, long userId, string setName, string setTitle, InputOnlineFile inputFile, string emoji)
        {
            if (isAnimated || isVideo)
            {
                inputFile = (await DownloadFileToStream(inputFile.FileId!))!;
            }

            if (isAnimated)
            {
                await CreateAnimatedStickerSet(userId, setName, setTitle, inputFile, emoji);
                return;
            }
            if (isVideo)
            {
                await CreateVideoStickerSet(userId, setName, setTitle, inputFile, emoji);
                return;
            }

            await CreateStaticStickerSet(userId, setName, setTitle, inputFile, emoji);
        }

        private Task CreateStaticStickerSet(long userId, string setName, string setTitle, InputOnlineFile inputFile, string emoji)
        {
            return _botContext.BotClient.CreateNewStaticStickerSetAsync(userId, setName, setTitle, inputFile, emoji);
        }

        private Task CreateAnimatedStickerSet(long userId, string setName, string setTitle, InputFileStream inputFile, string emoji)
        {
            return _botContext.BotClient.CreateNewAnimatedStickerSetAsync(userId, setName, setTitle, inputFile, emoji);
        }

        private Task CreateVideoStickerSet(long userId, string setName, string setTitle, InputFileStream inputFile, string emoji)
        {
            return _botContext.BotClient.CreateNewVideoStickerSetAsync(userId, setName, setTitle, inputFile, emoji);
        }

        #endregion

        #region Add to set

        private async ValueTask AddToStickerSetHanlder(bool isAnimated, bool isVideo, long userId, string setName, InputOnlineFile inputFile, string emoji)
        {
            if (isAnimated || isVideo)
            {
                inputFile = (await DownloadFileToStream(inputFile.FileId!))!;
            }

            if (isAnimated)
            {
                await AddToAnimatedStickerSet(userId, setName, inputFile, emoji);
                return;
            }
            if (isVideo)
            {
                await AddTovideoStickerSet(userId, setName, inputFile, emoji);
                return;
            }

            await AddToStaticStickerSet(userId, setName, inputFile, emoji);
        }

        private Task AddToStaticStickerSet(long userId, string setName, InputOnlineFile inputFile, string emoji)
        {
            return _botContext.BotClient.AddStaticStickerToSetAsync(userId, setName, inputFile, emoji);
        }

        private Task AddToAnimatedStickerSet(long userId, string setName, InputFileStream inputFile, string emoji)
        {
            return _botContext.BotClient.AddAnimatedStickerToSetAsync(userId, setName, inputFile, emoji);
        }

        private Task AddTovideoStickerSet(long userId, string setName, InputFileStream inputFile, string emoji)
        {
            return _botContext.BotClient.AddVideoStickerToSetAsync(userId, setName, inputFile, emoji);
        }

        #endregion

        #region Delete from set

        private Task DeleteStickerFromSet(string fileId)
        {
            return _botContext.BotClient.DeleteStickerFromSetAsync(fileId);
        }

        #endregion

    }
}