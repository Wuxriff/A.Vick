using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace A_Vick.Telegram.FunctionApp
{
    public class ProcessTelegramCallbackFunction
    {
        [FunctionName("ProcessTelegramCallback")]
        public async Task<IActionResult> UpdateAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request, ILogger logger)
        {
            var body = await request.ReadAsStringAsync();

            return new OkResult();
        }
    }
}
