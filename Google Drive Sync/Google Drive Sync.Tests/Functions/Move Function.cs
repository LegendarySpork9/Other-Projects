// Copyright © - 14/05/2025 - Toby Hunter
using GoogleDriveSync.Models;

namespace GoogleDriveSync.Tests.Functions
{
    internal static class MoveFunction
    {
        public static string MoveFile(string file)
        {
            string path = string.Empty;

            DirectoryInfo info = new(AppSettingsModel.LocalFolder);
            FileInfo[] fileInfos = info.GetFiles(file, SearchOption.AllDirectories);

            foreach (FileInfo fileInfo in fileInfos)
            {
                if (fileInfo.Name == file)
                {
                    if (fileInfo.Directory.Name == "Test")
                    {
                        File.Move($"{AppSettingsModel.LocalFolder}\\{file}", $"{AppSettingsModel.LocalFolder}\\Test Two\\{file}");
                        path = $"{AppSettingsModel.LocalFolder}\\Test Two\\{file}";
                    }

                    else
                    {
                        File.Move($"{AppSettingsModel.LocalFolder}\\Test Two\\{file}", $"{AppSettingsModel.LocalFolder}\\{file}");
                        path = $"{AppSettingsModel.LocalFolder}\\{file}";
                    }
                }
            }

            return path;
        }
    }
}
