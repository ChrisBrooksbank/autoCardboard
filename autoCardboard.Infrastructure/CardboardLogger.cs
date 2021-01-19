using Serilog;
using Serilog.Core;
using System;

namespace autoCardboard.Infrastructure
{
    public class CardboardLogger: ICardboardLogger
    {
        private readonly Logger _logger;

        public CardboardLogger()
        {
            _logger = new LoggerConfiguration()
             .WriteTo.File("logs\\autoCardboard.log", rollingInterval: RollingInterval.Day)
             .CreateLogger();

            Log.Logger = _logger;
        }

        public void Information(string message)
        {
            _logger.Information(message);
            Console.WriteLine(message);
        }
    }
}
