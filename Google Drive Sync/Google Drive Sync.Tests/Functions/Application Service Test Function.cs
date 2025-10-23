// Copyright © - 14/05/2025 - Toby Hunter
namespace GoogleDriveSync.Tests.Functions
{
    internal static class ApplicationServiceTestFunction
    {
        public static void UpdateTestNumber(string test, string file)
        {
            if (test == "TestSyncChangesUploadCreate")
            {
                string[] fileText = File.ReadAllLines(file);
                int index = fileText[2].IndexOf(':') + 2;
                int runNumber = int.Parse(fileText[2][index].ToString());

                if (runNumber == -1)
                {
                    runNumber = 0;
                }

                else
                {
                    runNumber++;
                }

                fileText[2] = fileText[2].Replace(fileText[2][index], char.Parse(runNumber.ToString()));
                File.WriteAllLines(file, fileText);
            }

            else if (test == "TestSyncChangesUploadMove")
            {
                string[] fileText = File.ReadAllLines(file);
                int index = fileText[3].IndexOf(':') + 2;
                int runNumber = int.Parse(fileText[3][index].ToString());

                if (runNumber == -1)
                {
                    runNumber = 0;
                }

                else
                {
                    runNumber++;
                }

                fileText[3] = fileText[3].Replace(fileText[3][index], char.Parse(runNumber.ToString()));
                File.WriteAllLines(file, fileText);
            }

            else if (test == "TestSyncChangesUploadUpdate")
            {
                string[] fileText = File.ReadAllLines(file);
                int index = fileText[4].IndexOf(':') + 2;
                int runNumber = int.Parse(fileText[4][index].ToString());

                if (runNumber == -1)
                {
                    runNumber = 0;
                }

                else
                {
                    runNumber++;
                }

                fileText[4] = fileText[4].Replace(fileText[4][index], char.Parse(runNumber.ToString()));
                File.WriteAllLines(file, fileText);
            }
        }
    }
}
