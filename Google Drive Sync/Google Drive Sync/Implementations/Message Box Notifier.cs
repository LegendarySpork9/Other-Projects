// Copyright © - Unpublished - Toby Hunter
using GoogleDriveSync.Abstractions;
using System.Windows.Forms;

namespace GoogleDriveSync.Implementations
{
    public class MessageBoxNotifier : IUserNotifier
    {
        // Displays a message box to the user with a warning.
        public void ShowMessage(string message, string title) => MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
