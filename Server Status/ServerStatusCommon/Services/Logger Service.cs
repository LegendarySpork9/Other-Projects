// Copyright © - 05/10/2025 - Toby Hunter
using log4net;

namespace ServerSiteCommon.Services
{
    public class LoggerService
    {
        private readonly ILog Logger = LogManager.GetLogger("Logs");
        private string Identifier = "System";

        // Updates the value of the Identifier variable.
        public void ChangeIdentifier(string value)
        {
            Identifier = value;
        }

        // Sends a meessage to the specified logs.
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
