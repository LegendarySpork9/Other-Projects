// Copyright © - 05/10/2025 - Toby Hunter
namespace ServerStatusSite.Converters
{
    // Returns the CSS to change the component to dark mode.
    public class StyleConverter
    {
        public string GetTopBarDarkMode(bool darkMode)
        {
            return darkMode switch
            {
                true => "background-color: #3E3E3E; border: 1px solid transparent;",
                _ => string.Empty
            };
        }

        public string GetTopNavLinkDarkMode(bool darkMode)
        {
            return darkMode switch
            {
                true => "color: white;",
                _ => string.Empty
            };
        }

        public string GetBodyDarkMode(bool darkMode)
        {
            return darkMode switch
            {
                true => "background-color: #313131; color: #A9A9A9;",
                _ => string.Empty
            };
        }

        public string GetNavMenuDarkMode(bool darkMode)
        {
            return darkMode switch
            {
                true => "background-color: #4E4E4E; color: white;",
                _ => string.Empty
            };
        }

        public string GetTableDarkMode(bool darkMode)
        {
            return darkMode switch
            {
                true => "color: #A9A9A9;",
                _ => string.Empty
            };
        }

        public string GetFormDarkMode(bool darkMode)
        {
            return darkMode switch
            {
                true => "form-dark",
                _ => string.Empty
            };
        }

        public string GetInputDarkMode(bool darkMode)
        {
            return darkMode switch
            {
                true => "background-color: #3E3E3E; color: #A9A9A9; border: 1px solid deepskyblue;",
                _ => string.Empty
            };
        }

        public string GetTableRowDarkMode(bool darkMode)
        {
            return darkMode switch
            {
                true => "dark-mode",
                _ => string.Empty
            };
        }
    }
}
