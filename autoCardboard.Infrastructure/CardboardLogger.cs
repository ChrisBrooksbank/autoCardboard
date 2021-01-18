using Serilog;
using Serilog.Core;

namespace autoCardboard.Infrastructure
{
    public class CardboardLogger: ICardboardLogger
    {
        private readonly Logger _logger;

        public CardboardLogger()
        {
            _logger = new LoggerConfiguration()
             .WriteTo.Console()
             .WriteTo.File("logs\\autoCardboard.log", rollingInterval: RollingInterval.Day)
             .CreateLogger();

            Log.Logger = _logger;
        }

        public void Information(string message)
        {
            _logger.Information(message);
        }
    }
}
