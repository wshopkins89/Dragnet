using DragnetControl.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using DragnetControl.Configuration;
using System;

namespace DragnetControl.Configuration
{
    public sealed class RuntimeSessionState
    {
        public RuntimeSessionState(string username)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
        }

        public string Username { get; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Pin { get; set; }
        public int AccountStatus { get; set; }

        public DatabaseCredentials? DragnetDatabase { get; set; }
        public DatabaseCredentials? ControlDatabase { get; set; }
        public DatabaseCredentials? AssetDatabase { get; set; }
        public DatabaseCredentials? NewsDatabase { get; set; }
        public DatabaseCredentials? SocialMediaDatabase { get; set; }
        public DatabaseCredentials? TrendsDatabase { get; set; }

        public string? UsersDbConnectionString { get; set; }
        public string? DragnetDbConnectionString { get; set; }
        public string? ControlDbConnectionString { get; set; }
        public string? AssetDbConnectionString { get; set; }
        public string? NewsDbConnectionString { get; set; }

        public string? CoinbaseScannerHost { get; set; }
        public string? CoinbaseScannerUser { get; set; }
        public string? CoinbaseScannerPassword { get; set; }
        public string? CoinbaseScannerPath { get; set; }
        public string? CoinbaseApiKey { get; set; }
        public string? CoinbaseSecret { get; set; }
        public string? CoinbasePassphrase { get; set; }
        public int CoinbaseScannerPort1 { get; set; }
        public int CoinbaseScannerPort2 { get; set; }

        public int CryptoGranularity { get; set; }
        public float CryptoTimeSpan { get; set; }
        public float CryptoDelay { get; set; }

        public string? TelegramHost { get; set; }
        public string? TelegramUser { get; set; }
        public string? TelegramPassword { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? ScriptsPath { get; set; }
        public string? TelegramApiKey { get; set; }
        public string? TelegramApiHash { get; set; }
        public int TelegramDelay { get; set; }
        public int TelegramTimespan { get; set; }

        public int CurationDelayTime { get; set; }
        public decimal CurationHistoryTime { get; set; }
        public string? CuratorPath { get; set; }

        public string? KrakenScannerHost { get; set; }
        public string? KrakenScannerUser { get; set; }
        public string? KrakenScannerPassword { get; set; }
        public int KrakenScannerPort1 { get; set; }
        public int KrakenScannerPort2 { get; set; }
        public string? KrakenPath { get; set; }
        public string? KrakenApiKey { get; set; }
        public string? KrakenPrivateKey { get; set; }

        public string? BinanceHost { get; set; }
        public string? BinanceUser { get; set; }
        public string? BinancePassword { get; set; }
        public int BinancePort1 { get; set; }
        public int BinancePort2 { get; set; }
        public string? BinancePath { get; set; }
        public string? BinanceApi { get; set; }
        public string? BinanceSecret { get; set; }

        public string? SelectedMarket { get; set; }
        public string? CongressImagesFilepath { get; set; }

        public string? LlmHost { get; set; }
        public string? LlmPort { get; set; }
        public string? LlmModel { get; set; }
        public int LlmContextWindow { get; set; }

        public string? DataDumpIp { get; set; }
        public string? DataDumpUser { get; set; }
        public string? DataDumpPassword { get; set; }

        public string? ActiveLlmPrompt { get; set; }
        public string? ActiveLlmPromptVersion { get; set; }

        public string? TargetUrl { get; set; }
        public string? ActiveTitle { get; set; }
        public string? ActiveSuffix { get; set; }
    }
}

