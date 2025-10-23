// Copyright © - 05/10/2025 - Toby Hunter
using ServerSiteCommon.Converters;
using ServerSiteCommon.Models;
using ServerSiteCommon.Services;
using System.Configuration;
using System.Reflection;

namespace ServerSiteCommon.Functions
{
    public static class SharedSettingsLoader
    {
        // Loads the given configuration file.
        public static Configuration LoadConfig(string config) => ConfigurationManager.OpenMappedExeConfiguration(new ()
            {
                ExeConfigFilename = config
    },ConfigurationUserLevel.None);

        // Loads the app settings dynamically from the App.config.
        public static SharedSettingsModel LoadSettingsFromConfig(Configuration config)
        {
            LoggerService _logger = new();

            SharedSettingsModel sharedSettings = new();
            PropertyInfo[] properties = typeof(SharedSettingsModel).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object? configurationValue = config.AppSettings.Settings[property.Name]?.Value;

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
