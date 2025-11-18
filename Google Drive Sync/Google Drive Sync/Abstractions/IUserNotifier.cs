// Copyright © - Unpublished - Toby Hunter
namespace GoogleDriveSync.Abstractions
{
    // Interface for the message box.
    public interface IUserNotifier
    {
        void ShowMessage(string message, string title);
    }
}
