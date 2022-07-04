namespace A_Vick.Telegram.Shared
{
    public static class Constants
    {
        //Database
        public const string DatabaseName = "Filename=Bot.db";

        //Telegram
        public const string TelegramToken = nameof(TelegramToken);
        public const string TelegramBotUserName = nameof(TelegramBotUserName);

        public const string TelegramBotCommandStart = "/start";
        public const string TelegramBotCommandCloneSet = "/clonestickerset";
        public const string TelegramBotCommandImportSet = "/importstickerset";
        public const string TelegramBotCommandAddStickerToSet = "/addstickertoset";
        public const string TelegramBotCommandDeleteSet = "/deletestickerset";
        public const string TelegramBotCommandDeleteStickerFromSet = "/deletestickerfromset";

        public const string TelegramBotStateUser = nameof(TelegramBotStateUser);
    }
}