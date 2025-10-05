namespace DragnetControl.Configuration
{
    public sealed class DatabaseCredentials
    {
        public DatabaseCredentials(
            string host,
            string username,
            string password,
            string database,
            int? primaryPort = null,
            int? secondaryPort = null)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Database = database ?? throw new ArgumentNullException(nameof(database));
            PrimaryPort = primaryPort;
            SecondaryPort = secondaryPort;
        }

        public string Host { get; }
        public string Username { get; }
        public string Password { get; }
        public string Database { get; }
        public int? PrimaryPort { get; }
        public int? SecondaryPort { get; }

        public string BuildConnectionString()
        {
                return $"server={Host};uid={Username};password={Password};database={Database}";
            }
    }
}
