using A_Vick.Telegram.BL.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace A_Vick.Telegram.Service.HostedServices
{
    internal class TelegramBotHostedService : IHostedService, IDisposable
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public TelegramBotHostedService(ITelegramBotService telegramBotService)
        {
            _telegramBotService = telegramBotService;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _telegramBotService.Init();
            await _telegramBotService.StartAsync(_cancellationTokenSource.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}