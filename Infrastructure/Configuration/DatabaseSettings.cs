using System;
using System.Configuration;

namespace DragnetControl.Infrastructure.Configuration
{
    /// <summary>
    /// Centralizes database connection strings so that they are read once from configuration
    /// and can be reused throughout the application without relying on scattered global state.
    /// </summary>
    public sealed class DatabaseSettings
    {
        private static readonly Lazy<DatabaseSettings> _instance =
            new Lazy<DatabaseSettings>(() => new DatabaseSettings());

        private DatabaseSettings()
        {
            UsersConnectionString = ReadConnectionString("Users");
            ControlConnectionString = ReadConnectionString("Control");
            DragnetConnectionString = ReadOptionalConnectionString("Dragnet") ?? ControlConnectionString;
            AssetConnectionString = ReadConnectionString("Asset");
            NewsConnectionString = ReadConnectionString("News");
        }

        public static DatabaseSettings Current => _instance.Value;

        public string UsersConnectionString { get; }

        public string ControlConnectionString { get; }

        public string DragnetConnectionString { get; }

        public string AssetConnectionString { get; }

        public string NewsConnectionString { get; }

        private static string ReadConnectionString(string name)
        {
            var settings = ConfigurationManager.ConnectionStrings[name];
            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    $"Connection string '{name}' is not configured in App.config.");
            }

            return settings.ConnectionString;
        }

        private static string? ReadOptionalConnectionString(string name)
        {
            var settings = ConfigurationManager.ConnectionStrings[name];
            return string.IsNullOrWhiteSpace(settings?.ConnectionString)
                ? null
                : settings.ConnectionString;
        }
    }
}
