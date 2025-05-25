using log4net;

namespace ServerSiteAutomation.Services
{
    public class LoggerService
    {
        private readonly ILog Logger = LogManager.GetLogger("Logs");
        private readonly string Identifier = "Automation";

        public void LogMessage(string level, string message)
        {
            switch (level)
            {
                case "Info": Logger.Info($"{Identifier} - {message.Trim()}"); break;
                case "Debug": Logger.Debug($"{Identifier} - {message.Trim()}"); break;
                case "Warn": Logger.Warn($"{Identifier} - {message.Trim()}"); break;
                case "Error": Logger.Error($"{Identifier} - {message.Trim()}"); break;
            }
        }
    }
}
