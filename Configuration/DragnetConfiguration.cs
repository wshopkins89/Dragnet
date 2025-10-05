using DragnetControl.Configuration;

using System;

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
    var host = ReadSetting("DRAGNET_USERSDB_HOST") ?? "localhost";
    var username = ReadSetting("DRAGNET_USERSDB_USER") ?? "dragnet";
    var password = ReadSetting("DRAGNET_USERSDB_PASSWORD") ?? "dragnet5";
    var database = ReadSetting("DRAGNET_USERSDB_NAME") ?? "userdata";
    
                return new DragnetConfiguration(new DatabaseCredentials(host, username, password, database));
            }

        private static string? ReadSetting(string key)
        {
                return Environment.GetEnvironmentVariable(key);
            }
    }
}
