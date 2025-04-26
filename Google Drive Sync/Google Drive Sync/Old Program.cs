using GoogleDriveSync.Converters;
using GoogleDriveSync.Models;
using GoogleDriveSync.Services;

namespace GoogleDriveSync
{
    internal class OldProgram
    {
        static void OldMain(string[] args)
        {
            /*log4net.Config.XmlConfigurator.Configure();

            LoggerService _logger = new();
            ApplicationService _applicationService = new();

            _logger.LogMessage(StandardValues.LoggerValues.Info, "Logging Started");
            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"Google Drive Folder(s): {string.Join(',', AppSettingsModel.DriveFolder)}");
            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"Local Folder(s): {string.Join(',', AppSettingsModel.LocalFolder)}");
            _logger.LogMessage(StandardValues.LoggerValues.Debug, $"Ignore File(s): {string.Join('|', AppSettingsModel.IgnoreFiles)}");

            Console.WriteLine(@"Welcome to the Google Drive Sync tool.

Please check the above details are correct before choosing one of the following run options. If the details are inccorrect, please close the console window down and alter the App Settings before running the tool again.
1 - Update Check
2 - Sync
3 - Close

Enter the number for the option you would like to perform.");

            string option = Console.ReadLine();

            while (true)
            {
                if (!string.IsNullOrWhiteSpace(option))
                {
                    if (option == "1")
                    {
                        _applicationService.CheckUpdates();
                        break;
                    }

                    else if (option == "2")
                    {
                        _applicationService.SyncChanges();
                        break;
                    }

                    else if (option == "3")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Closing Application.");
                        _logger.LogMessage(StandardValues.LoggerValues.Info, "Closing application.");
                        _logger.LogMessage(StandardValues.LoggerValues.Info, "Logging Stopped");
                        Environment.Exit(0);
                    }

                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("That is not a valid option, please choose from one of the following.");
                        Console.WriteLine(@"1 - Update Check
2 - Sync
3 - Close");
                    }
                }

                else
                {
                    Console.WriteLine();
                    Console.WriteLine("That is not a valid option, please choose from one of the following.");
                    Console.WriteLine(@"1 - Update Check
2 - Sync
3 - Close");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Closing Application.");
            _logger.LogMessage(StandardValues.LoggerValues.Info, "Closing application.");
            _logger.LogMessage(StandardValues.LoggerValues.Info, "Logging Stopped");
            Environment.Exit(0);*/
        }
    }
}
