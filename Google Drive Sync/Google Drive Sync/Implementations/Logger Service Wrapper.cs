// Copyright © - Unpublished - Toby Hunter
using GoogleDriveSync.Abstractions;
using GoogleDriveSync.Services;

namespace GoogleDriveSync.Implementations
{
    public class LoggerServiceWrapper : ILoggerService
    {
        readonly LoggerService _Logger = new LoggerService();

        // Logs the given message to the log file.
        public void LogMessage(string level, string message)
        {
            _Logger.LogMessage(level, message);
        }
    }
}
