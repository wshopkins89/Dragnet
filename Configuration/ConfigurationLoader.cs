using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MySqlConnector;

namespace DragnetControl.Configuration
{
    public sealed class ConfigurationLoader
    {
        private readonly DragnetConfiguration _configuration;

        public ConfigurationLoader(DragnetConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<RuntimeSessionState> LoadAsync(
            string username,
            IProgress<ConfigurationProgress>? progress = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username must be supplied", nameof(username));
            }

            var sessionState = new RuntimeSessionState(username)
            {
                UsersDbConnectionString = _configuration.UsersDatabase.BuildConnectionString()
            };

            var milestones = new Queue<(string Message, int Progress)>(new[]
            {
                ("Opening user database", 10),
                ("Loading user profile", 30),
                ("Loading database credentials", 60),
                ("Loading integrations", 90),
                ("Finishing", 100)
            });

            progress?.Report(new ConfigurationProgress("Preparing to load user profile", 0));

            await using var connection = new MySqlConnection(sessionState.UsersDbConnectionString);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            ReportProgress(progress, milestones);

            const string query = "SELECT * FROM users WHERE username = @username";
            await using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                throw new InvalidOperationException($"No configuration found for user '{username}'.");
            }

            sessionState.FirstName = GetString(reader, "Firstname");
            sessionState.LastName = GetString(reader, "Lastname");
            sessionState.Pin = GetString(reader, "Pin");
            sessionState.AccountStatus = GetInt32(reader, "accountstatus");
            ReportProgress(progress, milestones);

            sessionState.DragnetDatabase = new DatabaseCredentials(
                GetString(reader, "DragnetIP"),
                GetString(reader, "DragnetUser"),
                GetString(reader, "DragnetPassword"),
                GetString(reader, "DragnetDBName"),
                GetInt32(reader, "DragnetPort1"),
                GetInt32(reader, "DragnetPort2"));
            sessionState.ControlDatabase = new DatabaseCredentials(
                GetString(reader, "DragnetControlIP"),
                GetString(reader, "DragnetControlUser"),
                GetString(reader, "DragnetControlPassword"),
                GetString(reader, "DragnetControlDBName"),
                GetInt32(reader, "DragnetControlPort1"),
                GetInt32(reader, "DragnetControlPort2"));
            sessionState.AssetDatabase = new DatabaseCredentials(
                GetString(reader, "AssetIP"),
                GetString(reader, "assetuser"),
                GetString(reader, "assetpw"),
                GetString(reader, "assetDBName"),
                GetInt32(reader, "assetPort1"),
                GetInt32(reader, "assetPort2"));
            sessionState.NewsDatabase = new DatabaseCredentials(
                GetString(reader, "newsIP"),
                GetString(reader, "newsuser"),
                GetString(reader, "newspw"),
                "newsdata",
                GetInt32(reader, "newsport1"),
                GetInt32(reader, "newsport2"));
            sessionState.SocialMediaDatabase = new DatabaseCredentials(
                GetString(reader, "SMIP"),
                GetString(reader, "SMuser"),
                GetString(reader, "smPW"),
                GetOptionalString(reader, "SMDatabase") ?? string.Empty,
                GetNullableInt32(reader, "SMPort1"),
                GetNullableInt32(reader, "SMPort2"));
            sessionState.TrendsDatabase = new DatabaseCredentials(
                GetString(reader, "trendsip"),
                GetString(reader, "trendsuser"),
                GetString(reader, "trendsPW"),
                GetOptionalString(reader, "trendsDatabase") ?? string.Empty,
                GetNullableInt32(reader, "trendsPort1"),
                GetNullableInt32(reader, "trendsPort2"));

            sessionState.DragnetDbConnectionString = sessionState.DragnetDatabase.BuildConnectionString();
            sessionState.ControlDbConnectionString = sessionState.ControlDatabase.BuildConnectionString();
            sessionState.AssetDbConnectionString = sessionState.AssetDatabase.BuildConnectionString();
            sessionState.NewsDbConnectionString = sessionState.NewsDatabase.BuildConnectionString();
            ReportProgress(progress, milestones);

            sessionState.CoinbaseScannerHost = GetString(reader, "CBScannerHost");
            sessionState.CoinbaseScannerUser = GetString(reader, "CBScannerUser");
            sessionState.CoinbaseScannerPassword = GetString(reader, "CBScannerPW");
            sessionState.CoinbaseScannerPath = GetString(reader, "CBScannerPath");
            sessionState.CoinbaseApiKey = GetString(reader, "coinbaseAPIKey");
            sessionState.CoinbaseSecret = GetString(reader, "CoinbaseSecret");
            sessionState.CoinbasePassphrase = GetString(reader, "CoinbasePassphrase");
            sessionState.CoinbaseScannerPort1 = GetInt32(reader, "CBScannerPort1");
            sessionState.CoinbaseScannerPort2 = GetInt32(reader, "CBScannerPort2");
            sessionState.CryptoGranularity = GetInt32(reader, "CryptoGranularity");
            sessionState.CryptoTimeSpan = GetFloat(reader, "CryptoTimeFrame");
            sessionState.CryptoDelay = GetFloat(reader, "CryptoDelay");

            sessionState.TelegramHost = GetString(reader, "TelegramHost");
            sessionState.TelegramUser = GetString(reader, "TelegramUser");
            sessionState.TelegramPassword = GetString(reader, "TelegramPW");
            sessionState.TelephoneNumber = GetString(reader, "TelephoneNumber");
            sessionState.ScriptsPath = GetString(reader, "ScriptsPath");
            sessionState.TelegramApiKey = GetString(reader, "TelegramAPIID");
            sessionState.TelegramApiHash = GetString(reader, "TelegramAPIHash");
            sessionState.TelegramDelay = GetInt32(reader, "TelegramDelay");
            sessionState.TelegramTimespan = GetInt32(reader, "TelegramTimespan");

            sessionState.CurationDelayTime = GetInt32(reader, "CurationDelayTime");
            sessionState.CurationHistoryTime = GetDecimal(reader, "CurationHistoryTime");
            sessionState.CuratorPath = GetString(reader, "CuratorPath");

            sessionState.KrakenScannerHost = GetString(reader, "KrakenHost");
            sessionState.KrakenScannerUser = GetString(reader, "KrakenScannerUser");
            sessionState.KrakenScannerPassword = GetString(reader, "KrakenScannerPW");
            sessionState.KrakenScannerPort1 = GetInt32(reader, "KrakenScannerPort1");
            sessionState.KrakenScannerPort2 = GetInt32(reader, "KrakenScannerPort2");
            sessionState.KrakenPath = GetString(reader, "KrakenPath");
            sessionState.KrakenApiKey = GetString(reader, "KrakenAPI");
            sessionState.KrakenPrivateKey = GetString(reader, "KrakenPrivateKey");

            sessionState.BinanceHost = GetString(reader, "BinanceHost");
            sessionState.BinanceUser = GetString(reader, "BinanceUser");
            sessionState.BinancePassword = GetString(reader, "BinancePW");
            sessionState.BinancePort1 = GetInt32(reader, "BinancePort1");
            sessionState.BinancePort2 = GetInt32(reader, "BinancePort2");
            sessionState.BinancePath = GetString(reader, "BinancePath");
            sessionState.BinanceApi = GetString(reader, "BinanceAPI");
            sessionState.BinanceSecret = GetString(reader, "BinanceSecret");

            sessionState.CongressImagesFilepath = GetString(reader, "CongressImagesFilePath");
            sessionState.LlmHost = GetString(reader, "LLMHost");
            sessionState.LlmPort = GetString(reader, "LLMPort");
            sessionState.LlmModel = GetString(reader, "LLMModel");
            sessionState.LlmContextWindow = GetInt32(reader, "LLMContextWindow");
            sessionState.DataDumpIp = GetString(reader, "DataDumpIP");
            sessionState.DataDumpUser = GetString(reader, "DataDumpUser");
            sessionState.DataDumpPassword = GetString(reader, "DataDumpPassword");
            sessionState.ActiveLlmPrompt = GetString(reader, "LLMPromptName");
            sessionState.ActiveLlmPromptVersion = GetString(reader, "LLMPromptVersion");

            ReportProgress(progress, milestones);

            sessionState.TargetUrl = GetOptionalString(reader, "TargetURL");
            sessionState.ActiveTitle = GetOptionalString(reader, "ActiveTitle");
            sessionState.ActiveSuffix = GetOptionalString(reader, "ActiveSuffix");

            ReportProgress(progress, milestones, forceComplete: true);

            return sessionState;
        }

        private static void ReportProgress(
            IProgress<ConfigurationProgress>? progress,
            Queue<(string Message, int Progress)> milestones,
            bool forceComplete = false)
        {
            if (progress == null)
            {
                return;
            }

            if (forceComplete)
            {
                progress.Report(new ConfigurationProgress("Configuration loaded", 100));
                return;
            }

            if (milestones.Count == 0)
            {
                return;
            }

            var milestone = milestones.Dequeue();
            progress.Report(new ConfigurationProgress(milestone.Message, milestone.Progress));
        }

        private static string GetString(MySqlDataReader reader, string column)
        {
            if (!TryGetOrdinal(reader, column, out var ordinal))
            {
                throw new InvalidOperationException($"Required column '{column}' was not returned by the query.");
            }

            if (reader.IsDBNull(ordinal))
            {
                throw new InvalidOperationException($"Column '{column}' contained NULL but a value was required.");
            }

            return reader.GetString(ordinal);
        }

        private static string? GetOptionalString(MySqlDataReader reader, string column)
        {
            if (!TryGetOrdinal(reader, column, out var ordinal))
            {
                return null;
            }

            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static int GetInt32(MySqlDataReader reader, string column)
        {
            if (!TryGetOrdinal(reader, column, out var ordinal))
            {
                return 0;
            }

            return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
        }

        private static int? GetNullableInt32(MySqlDataReader reader, string column)
        {
            if (!TryGetOrdinal(reader, column, out var ordinal))
            {
                return null;
            }

            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }

        private static float GetFloat(MySqlDataReader reader, string column)
        {
            if (!TryGetOrdinal(reader, column, out var ordinal))
            {
                return 0f;
            }

            return reader.IsDBNull(ordinal) ? 0 : reader.GetFloat(ordinal);
        }

        private static decimal GetDecimal(MySqlDataReader reader, string column)
        {
            if (!TryGetOrdinal(reader, column, out var ordinal))
            {
                return 0m;
            }

            return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
        }

        private static bool TryGetOrdinal(MySqlDataReader reader, string column, out int ordinal)
        {
            try
            {
                ordinal = reader.GetOrdinal(column);
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                ordinal = -1;
                return false;
            }
        }
    }
}
