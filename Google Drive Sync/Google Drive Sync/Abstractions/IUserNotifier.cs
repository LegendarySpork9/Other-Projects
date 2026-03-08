// Copyright © - Unpublished - Toby Hunter
namespace GoogleDriveSync.Abstractions
{
    /// <summary>
    /// Interface for the message box.
    /// </summary>
    public interface IUserNotifier
    {
        void ShowMessage(string message, string title);
    }
}
