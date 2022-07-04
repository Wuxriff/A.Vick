namespace A_Vick.Telegram.Models.Enums
{
    public enum TelegramBotHandlerStates
    {
        //None
        None = 0,

        //IdleHandler
        Idle = 100,

        //CommandCloneSetHandler
        CommandCloneSet_Start = 200,
        CommandCloneSet_WaitingSticker = 201,
        CommandCloneSet_WaitingSetName = 202,

        //CommandImportSetHandler
        CommandImportSet_Start = 300,
        CommandImportSet_WaitingSticker = 301,
        CommandImportSet_WaitingStickerSet = 302,

        //CommandAddStickerToSetHandler
        CommandAddStickerToSet_Start = 400,
        CommandAddStickerToSet_WaitingSticker = 401,
        CommandAddStickerToSet_WaitingStickerSet = 402,

        //CommandDeleteSetHandler
        CommandDeleteSet_Start = 500,
        CommandDeleteSet_WaitingSticker = 501,
        CommandDeleteSet_WaitingApproval = 502,


        //CommandDeleteStickerFromSetHandler
        CommandDeleteStickerFromSet_Start = 600,
        CommandDeleteStickerFromSet_WaitingSticker = 601,
        CommandDeleteStickerFromSet_WaitingApproval = 602,
    }
}