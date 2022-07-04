using System.Threading;
using System.Threading.Tasks;

namespace A_Vick.Telegram.BL.Interfaces
{
    public interface ITelegramBotService
    {
        void Init();
        ValueTask StartAsync(CancellationToken cancellationToken);
    }
}