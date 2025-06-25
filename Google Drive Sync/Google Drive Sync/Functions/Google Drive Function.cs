namespace GoogleDriveSync.Functions
{
    internal class GoogleDriveFunction
    {
        // Returns the final folder in the given string.
        public string RemoveStringCharacters(string value, char[] characters, string direction)
        {
            string modifiedValue = value;

            if (direction == "Right")
            {
                foreach (char character in characters)
                {
                    if (modifiedValue.IndexOf(character) != -1)
                    {
                        modifiedValue = modifiedValue.Remove(0, modifiedValue.LastIndexOf(character) + 1);
                    }
                }
            }

            else
            {
                foreach (char character in characters)
                {
                    if (modifiedValue.IndexOf(character) != -1)
                    {
                        modifiedValue = modifiedValue.Remove(modifiedValue.IndexOf(character));
                    }
                }
            }

            return modifiedValue;
        }
    }
}
