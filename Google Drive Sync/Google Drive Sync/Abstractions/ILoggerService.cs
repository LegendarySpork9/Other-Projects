// Copyright © - Unpublished - Toby Hunter
namespace GoogleDriveSync.Abstractions
{
    /// <summary>
    /// Interface for the logger service.
    /// </summary>
    public interface ILoggerService
    {
        void LogMessage(string level, string message);
    }
}