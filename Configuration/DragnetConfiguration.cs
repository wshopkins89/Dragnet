using System;
using System.Configuration;

namespace DragnetControl.Configuration
{
    public sealed class DragnetConfiguration
    {
        private DragnetConfiguration(DatabaseCredentials usersDatabase)
        {
            UsersDatabase = usersDatabase;
        }

        public DatabaseCredentials UsersDatabase { get; }

        public static DragnetConfiguration FromAppConfig()
        {
            var host = ReadSetting("DRAGNET_USERSDB_HOST") ?? ReadAppSetting("UsersDbHost") ?? "localhost";
            var username = ReadSetting("DRAGNET_USERSDB_USER") ?? ReadAppSetting("UsersDbUser") ?? "dragnet";
            var password = ReadSetting("DRAGNET_USERSDB_PASSWORD") ?? ReadAppSetting("UsersDbPassword") ?? "dragnet5";
            var database = ReadSetting("DRAGNET_USERSDB_NAME") ?? ReadAppSetting("UsersDbName") ?? "userdata";

            return new DragnetConfiguration(new DatabaseCredentials(host, username, password, database));
        }

        private static string? ReadSetting(string key) => Environment.GetEnvironmentVariable(key);

        private static string? ReadAppSetting(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (ConfigurationErrorsException)
            {
                return null;
            }
        }
    }
}
