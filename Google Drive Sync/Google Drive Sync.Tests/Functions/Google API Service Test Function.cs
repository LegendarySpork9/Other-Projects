// Copyright © - 14/05/2025 - Toby Hunter
namespace GoogleDriveSync.Tests.Functions
{
    internal static class GoogleAPIServiceTestFunction
    {
        public static void UpdateTestNumber(string test, string file)
        {
            if (test == "TestCreate")
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

            else if (test == "TestUpdate")
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

            else if (test == "TestMove")
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

            else if (test == "TestDownload")
            {
                string[] fileText = File.ReadAllLines(file);
                int index = fileText[5].IndexOf(':') + 2;
                int runNumber = int.Parse(fileText[5][index].ToString());

                if (runNumber == -1)
                {
                    runNumber = 0;
                }

                else
                {
                    runNumber++;
                }

                fileText[5] = fileText[5].Replace(fileText[5][index], char.Parse(runNumber.ToString()));
                File.WriteAllLines(file, fileText);
            }

            else if (test == "TestDelete")
            {
                string[] fileText = File.ReadAllLines(file);
                int index = fileText[6].IndexOf(':') + 2;
                int runNumber = int.Parse(fileText[6][index].ToString());

                if (runNumber == -1)
                {
                    runNumber = 0;
                }

                else
                {
                    runNumber++;
                }

                fileText[6] = fileText[6].Replace(fileText[6][index], char.Parse(runNumber.ToString()));
                File.WriteAllLines(file, fileText);
            }
        }
    }
}
