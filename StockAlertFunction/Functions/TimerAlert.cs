using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace StockAlertFunction.Functions
{
    public class TimerAlert
    {
        private readonly ILogger _logger;

        public TimerAlert(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TimerAlert>();
        }

        [Function("TimeAlert")]
        public void Run([TimerTrigger("0 0 15-23 * * 1-5")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }

    }
}
