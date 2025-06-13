using ServerSiteCommon.Converters;
using ServerSiteCommon.Models;
using ServerSiteCommon.Services;
using System.Configuration;
using System.Reflection;

namespace ServerSiteCommon.Functions
{
    public static class SharedSettingsLoader
    {
        // Loads the app settings dynamically from the App.config.
        public static SharedSettingsModel LoadSettingsFromConfig()
        {
            LoggerService _logger = new();

            SharedSettingsModel sharedSettings = new();
            PropertyInfo[] properties = typeof(SharedSettingsModel).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object? configurationValue = ConfigurationManager.AppSettings[property.Name];

                if (configurationValue != null)
                {
                    try
                    {
                        object convertedValue = Convert.ChangeType(configurationValue, property.PropertyType);
                        property.SetValue(sharedSettings, convertedValue);
                    }

                    catch (Exception ex)
                    {
                        _logger.LogMessage(StandardValues.LoggerValues.Warning, ex.Message);
                        _logger.LogMessage(StandardValues.LoggerValues.Error, ex.ToString());
                    }
                }
            }

            return sharedSettings;
        }
    }
}
