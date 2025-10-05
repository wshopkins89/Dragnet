using System;
using DragnetControl.Configuration;

namespace DragnetControl
{
    public static class GlobalVariables
    {
        private static RuntimeSessionState? _sessionState;
        private static DragnetConfiguration? _configuration;

        public static void Initialize(DragnetConfiguration configuration, RuntimeSessionState sessionState)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _sessionState = sessionState ?? throw new ArgumentNullException(nameof(sessionState));
        }

        public static RuntimeSessionState SessionState => _sessionState ?? throw new InvalidOperationException("Runtime session state has not been initialised.");
        public static DragnetConfiguration Configuration => _configuration ?? throw new InvalidOperationException("Configuration has not been initialised.");

        public static string username => SessionState.Username;
        public static string firstname => SessionState.FirstName ?? string.Empty;
        public static string lastname => SessionState.LastName ?? string.Empty;
        public static string pin => SessionState.Pin ?? string.Empty;
        public static int accountstatus => SessionState.AccountStatus;

        public static string UsersDBConnect => SessionState.UsersDbConnectionString ?? Configuration.UsersDatabase.BuildConnectionString();
        public static string UsersDBIP => Configuration.UsersDatabase.Host;
        public static string UsersDBUsername => Configuration.UsersDatabase.Username;
        public static string usersdbPW => Configuration.UsersDatabase.Password;

        public static string DragnetDBIP => SessionState.DragnetDatabase?.Host ?? string.Empty;
        public static string DragnetDBUser => SessionState.DragnetDatabase?.Username ?? string.Empty;
        public static string DragnetDBPassword => SessionState.DragnetDatabase?.Password ?? string.Empty;
        public static int DragnetPort1 => SessionState.DragnetDatabase?.PrimaryPort ?? 0;
        public static int DragnetPort2 => SessionState.DragnetDatabase?.SecondaryPort ?? 0;
        public static string DragnetDBName => SessionState.DragnetDatabase?.Database ?? string.Empty;
        public static string DragnetDBConnect => SessionState.DragnetDbConnectionString ?? string.Empty;

        public static string DragnetControlIP => SessionState.ControlDatabase?.Host ?? string.Empty;
        public static string DragnetControlUser => SessionState.ControlDatabase?.Username ?? string.Empty;
        public static string DragnetControlPassword => SessionState.ControlDatabase?.Password ?? string.Empty;
        public static int DragnetControlPort1 => SessionState.ControlDatabase?.PrimaryPort ?? 0;
        public static int DragnetControlPort2 => SessionState.ControlDatabase?.SecondaryPort ?? 0;
        public static string DragnetControlName => SessionState.ControlDatabase?.Database ?? string.Empty;
        public static string ControlDBConnect => SessionState.ControlDbConnectionString ?? string.Empty;

        public static string assetIP => SessionState.AssetDatabase?.Host ?? string.Empty;
        public static string assetUser => SessionState.AssetDatabase?.Username ?? string.Empty;
        public static string assetPW => SessionState.AssetDatabase?.Password ?? string.Empty;
        public static int assetport1 => SessionState.AssetDatabase?.PrimaryPort ?? 0;
        public static int assetport2 => SessionState.AssetDatabase?.SecondaryPort ?? 0;
        public static string assetDBName => SessionState.AssetDatabase?.Database ?? string.Empty;
        public static string AssetDBConnect => SessionState.AssetDbConnectionString ?? string.Empty;

        public static string newsIP => SessionState.NewsDatabase?.Host ?? string.Empty;
        public static string newsUser => SessionState.NewsDatabase?.Username ?? string.Empty;
        public static string newsPW => SessionState.NewsDatabase?.Password ?? string.Empty;
        public static int newsport1 => SessionState.NewsDatabase?.PrimaryPort ?? 0;
        public static int newsport2 => SessionState.NewsDatabase?.SecondaryPort ?? 0;
        public static string NewsDBConnect => SessionState.NewsDbConnectionString ?? string.Empty;

        public static string smIP => SessionState.SocialMediaDatabase?.Host ?? string.Empty;
        public static string smUser => SessionState.SocialMediaDatabase?.Username ?? string.Empty;
        public static string smPW => SessionState.SocialMediaDatabase?.Password ?? string.Empty;

        public static string trendsIP => SessionState.TrendsDatabase?.Host ?? string.Empty;
        public static string trendsUser => SessionState.TrendsDatabase?.Username ?? string.Empty;
        public static string trendsPW => SessionState.TrendsDatabase?.Password ?? string.Empty;

        public static string CoinbaseScannerHost => SessionState.CoinbaseScannerHost ?? string.Empty;
        public static string CoinbaseScannerUser => SessionState.CoinbaseScannerUser ?? string.Empty;
        public static string CoinbaseScannerPW => SessionState.CoinbaseScannerPassword ?? string.Empty;
        public static string CoinbaseScannerPath => SessionState.CoinbaseScannerPath ?? string.Empty;
        public static string coinbaseAPIKey => SessionState.CoinbaseApiKey ?? string.Empty;
        public static string CoinbaseSecret => SessionState.CoinbaseSecret ?? string.Empty;
        public static string CoinbasePassphrase => SessionState.CoinbasePassphrase ?? string.Empty;
        public static int CoinbaseScannerPort1 => SessionState.CoinbaseScannerPort1;
        public static int CoinbaseScannerPort2 => SessionState.CoinbaseScannerPort2;

        public static int CryptoGranularity => SessionState.CryptoGranularity;
        public static float CryptoTimeSpan => SessionState.CryptoTimeSpan;
        public static float CryptoDelay => SessionState.CryptoDelay;

        public static string TelegramHost => SessionState.TelegramHost ?? string.Empty;
        public static string TelegramUser => SessionState.TelegramUser ?? string.Empty;
        public static string TelegramPW => SessionState.TelegramPassword ?? string.Empty;
        public static string TelephoneNumber => SessionState.TelephoneNumber ?? string.Empty;
        public static string ScriptsPath => SessionState.ScriptsPath ?? string.Empty;
        public static string TelegramAPIKey => SessionState.TelegramApiKey ?? string.Empty;
        public static string TelegramAPIHash => SessionState.TelegramApiHash ?? string.Empty;
        public static int TelegramDelay => SessionState.TelegramDelay;
        public static int TelegramTimespan => SessionState.TelegramTimespan;
        public static string PhoneNumber => SessionState.TelephoneNumber ?? string.Empty;

        public static int CurationDelayTime => SessionState.CurationDelayTime;
        public static decimal CurationHistoryTime => SessionState.CurationHistoryTime;
        public static string CuratorPath => SessionState.CuratorPath ?? string.Empty;

        public static string KrakenScannerHost => SessionState.KrakenScannerHost ?? string.Empty;
        public static string KrakenScannerUser => SessionState.KrakenScannerUser ?? string.Empty;
        public static string KrakenScannerPW => SessionState.KrakenScannerPassword ?? string.Empty;
        public static int KrakenScannerPort1 => SessionState.KrakenScannerPort1;
        public static int KrakenScannerPort2 => SessionState.KrakenScannerPort2;
        public static string KrakenPath => SessionState.KrakenPath ?? string.Empty;
        public static string KrakenAPIKey => SessionState.KrakenApiKey ?? string.Empty;
        public static string KrakenPrivateKey => SessionState.KrakenPrivateKey ?? string.Empty;

        public static string BinanceHost => SessionState.BinanceHost ?? string.Empty;
        public static string BinanceUser => SessionState.BinanceUser ?? string.Empty;
        public static string BinancePW => SessionState.BinancePassword ?? string.Empty;
        public static int BinancePort1 => SessionState.BinancePort1;
        public static int BinancePort2 => SessionState.BinancePort2;
        public static string BinancePath => SessionState.BinancePath ?? string.Empty;
        public static string BinanceAPI => SessionState.BinanceApi ?? string.Empty;
        public static string BinanceSecret => SessionState.BinanceSecret ?? string.Empty;

        public static string CongressImagesFilepath => SessionState.CongressImagesFilepath ?? string.Empty;

        public static string LLMHost => SessionState.LlmHost ?? string.Empty;
        public static string LLMPort => SessionState.LlmPort ?? string.Empty;
        public static string LLMModel => SessionState.LlmModel ?? string.Empty;
        public static int LLMContextWindow => SessionState.LlmContextWindow;

        public static string DataDumpIP => SessionState.DataDumpIp ?? string.Empty;
        public static string DataDumpUser => SessionState.DataDumpUser ?? string.Empty;
        public static string DataDumpPW => SessionState.DataDumpPassword ?? string.Empty;

        public static string ActiveLLMPrompt => SessionState.ActiveLlmPrompt ?? string.Empty;
        public static string ActiveLLMPromptVersion => SessionState.ActiveLlmPromptVersion ?? string.Empty;

        public static string TargetURL => SessionState.TargetUrl ?? string.Empty;
        public static string ActiveTitle => SessionState.ActiveTitle ?? string.Empty;
        public static string activesuffix => SessionState.ActiveSuffix ?? string.Empty;

        public static void UpdateSessionState(Action<RuntimeSessionState> updater)
        {
            if (updater == null)
            {
                throw new ArgumentNullException(nameof(updater));
            }

            updater(SessionState);
        }
    }
}
