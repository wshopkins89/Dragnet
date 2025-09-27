using MySqlConnector;


namespace DragnetControl
{
    public partial class AssetLoadingScreen : Form
    {
        string username = GlobalVariables.username;
        public string userConnStr = "server=192.168.1.210;uid=dragnet;password=dragnet5;database=userdata";

        public AssetLoadingScreen()
        {
            InitializeComponent();
            this.Load += AssetLoadingScreen_Load;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 200;
        }

        private void AssetLoadingScreen_Load(object sender, EventArgs e)
        {
            LoadUserSettings();
        }

        private void LoadUserSettings()
        {
            using (MySqlConnection conn = new MySqlConnection(userConnStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM users WHERE username = @username";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {

                            if (reader.HasRows)
                            {
                                reader.Read();
                                {
                                    GlobalVariables.firstname = reader.GetString("Firstname");
                                    StatusLabel.Text = "Loading User Reference Data";

                                    GlobalVariables.lastname = reader.GetString("Lastname");
                                    StatusLabel.Text = "Loading User Reference Data";

                                    GlobalVariables.pin = reader.GetString("Pin");
                                    StatusLabel.Text = "Loading User Reference Data: Pin";

                                    GlobalVariables.DragnetDBIP = reader.GetString("DragnetIP");
                                    StatusLabel.Text = "Loading Dragnet Database Configuration: DragnetIP";

                                    GlobalVariables.DragnetDBUser = reader.GetString("DragnetUser");
                                    StatusLabel.Text = "Loading Dragnet Database Configuration: DragnetUser";

                                    GlobalVariables.DragnetDBPassword = reader.GetString("DragnetPassword");
                                    StatusLabel.Text = "Loading Dragnet Database Configuration: DragnetPassword";

                                    GlobalVariables.DragnetPort1 = reader.GetInt32("DragnetPort1");
                                    StatusLabel.Text = "Loading Dragnet Database Configuration: DragnetPort1";

                                    GlobalVariables.DragnetPort2 = reader.GetInt32("DragnetPort2");
                                    StatusLabel.Text = "Loading Dragnet Database Configuration: DragnetPort2";

                                    GlobalVariables.DragnetDBName = reader.GetString("DragnetDBName");
                                    StatusLabel.Text = "Loading Dragnet Database Configuration: DragnetDBName";

                                    GlobalVariables.DragnetControlIP = reader.GetString("DragnetControlIP");
                                    StatusLabel.Text = "Loading Dragnet Control Database Configuration: DragnetControlIP";

                                    GlobalVariables.DragnetControlUser = reader.GetString("DragnetControlUser");
                                    StatusLabel.Text = "Loading Dragnet Control Database Configuration: DragnetControlUser";

                                    GlobalVariables.DragnetControlPassword = reader.GetString("DragnetControlPassword");
                                    StatusLabel.Text = "Loading Dragnet Control Database Configuration: DragnetControlPassword";

                                    GlobalVariables.DragnetControlPort1 = reader.GetInt32("DragnetControlPort1");
                                    StatusLabel.Text = "Loading Dragnet ControlDatabase Configuration: DragnetControlPort1";

                                    GlobalVariables.DragnetControlPort2 = reader.GetInt32("DragnetControlPort2");
                                    StatusLabel.Text = "Loading Dragnet Control Database Configuration: DragnetControlPort2";

                                    GlobalVariables.DragnetControlName = reader.GetString("DragnetControlDBName");
                                    StatusLabel.Text = "Loading Dragnet Control Database Configuration: DragnetControlName";

                                    GlobalVariables.assetIP = reader.GetString("AssetIP");
                                    StatusLabel.Text = "Loading Asset Database Configuration: AssetIP";

                                    GlobalVariables.assetUser = reader.GetString("assetuser");
                                    StatusLabel.Text = "Loading Asset Database Configuration: AssetUsername";

                                    GlobalVariables.assetPW = reader.GetString("assetpw");
                                    StatusLabel.Text = "Loading Asset Database Configuration: AssetPassword";

                                    GlobalVariables.assetport1 = reader.GetInt32("assetPort1");
                                    StatusLabel.Text = "Loading Asset Database Configuration: AssetPort1";

                                    GlobalVariables.assetport2 = reader.GetInt32("assetPort2");
                                    StatusLabel.Text = "Loading Asset Database Configuration: AssetPort2";

                                    GlobalVariables.assetDBName = reader.GetString("assetDBName");
                                    StatusLabel.Text = "Loading Asset Database Configuration: AssetDBName";

                                    GlobalVariables.newsIP = reader.GetString("newsIP");
                                    StatusLabel.Text = "Loading News Scanner Configuration: NewsDatabaseIP";

                                    GlobalVariables.newsUser = reader.GetString("newsuser");
                                    StatusLabel.Text = "Loading News Scanner Configuration: NewsDataBaseUser";

                                    GlobalVariables.newsPW = reader.GetString("newspw");
                                    StatusLabel.Text = "Loading News Scanner Configuration: NewsDataBasePassword";

                                    GlobalVariables.newsport1 = reader.GetInt32("newsport1");
                                    StatusLabel.Text = "Loading News Scanner Configuration: NewsPort1";

                                    GlobalVariables.newsport2 = reader.GetInt32("newsport2");
                                    StatusLabel.Text = "Loading News Scanner Configuration: NewsPort2";

                                    GlobalVariables.smIP = reader.GetString("SMIP");
                                    StatusLabel.Text = "Loading Social Media Scanner Configuration: SMIP";

                                    GlobalVariables.smUser = reader.GetString("SMuser");
                                    StatusLabel.Text = "Loading Social Media Scanner Configuration: SMuser";

                                    GlobalVariables.smPW = reader.GetString("smPW");
                                    StatusLabel.Text = "Loading Social Media Scanner Configuration: SMPassword";

                                    GlobalVariables.trendsIP = reader.GetString("trendsip");
                                    StatusLabel.Text = "Loading Trends Scanner Configuration: TrendsIP";

                                    GlobalVariables.trendsUser = reader.GetString("trendsuser");
                                    StatusLabel.Text = "Loading Trends Scanner Configuration: TrendsUser";

                                    GlobalVariables.trendsPW = reader.GetString("trendsPW");
                                    StatusLabel.Text = "Loading Trends Scanner Configuration: TrendsPW";                   

                                    GlobalVariables.CoinbaseScannerHost = reader.GetString("CBScannerHost");
                                    StatusLabel.Text = "Loading Coinbase Asset Scanner Data: CoinbaseHost";

                                    GlobalVariables.CoinbaseScannerUser = reader.GetString("CBScannerUser");
                                    StatusLabel.Text = "Loading Coinbase Asset Scanner Data: CoinbaseScannerUser";

                                    GlobalVariables.CoinbaseScannerPW = reader.GetString("CBScannerPW");
                                    StatusLabel.Text = "Loading Coinbase Asset Scanner Data: CoinbaseScannerPW";

                                    GlobalVariables.coinbaseAPIKey = reader.GetString("coinbaseAPIKey");
                                    StatusLabel.Text = "Loading Coinbase Asset Scanner Data: CoinbaseAPIKey";

                                    GlobalVariables.CoinbaseSecret = reader.GetString("CoinbaseSecret");
                                    StatusLabel.Text = "Loading Coinbase Asset Scanner Data: CoinbaseAPISecret";

                                    GlobalVariables.CoinbasePassphrase = reader.GetString("CoinbasePassphrase");
                                    StatusLabel.Text = "Loading Coinbase Asset Scanner Data: CoinbasePassphrase";

                                    GlobalVariables.CoinbaseScannerPath = reader.GetString("CBScannerPath");
                                    StatusLabel.Text = "Loading Coinbase Asset Scanner Data: CoinbaseScannerPath";

                                    GlobalVariables.CryptoGranularity = reader.GetInt32("CryptoGranularity");
                                    StatusLabel.Text = "Loading Crypto Global Asset: Cryptogranularity";

                                    GlobalVariables.CryptoTimeSpan = reader.GetFloat("CryptoTimeFrame");
                                    StatusLabel.Text = "Loading Crypto Global Asset: CryptoTimeFrame";

                                    GlobalVariables.CryptoDelay = reader.GetFloat("CryptoDelay");
                                    StatusLabel.Text = "Loading Crypto Global Asset: CryptoDelay";

                                    GlobalVariables.TelegramHost = reader.GetString("TelegramHost");
                                    StatusLabel.Text = "Loading Telegram Scanner Data: Telegram Host";

                                    GlobalVariables.TelegramUser = reader.GetString("TelegramUser");
                                    StatusLabel.Text = "Loading Telegram Scanner Data: Telegram Username";

                                    GlobalVariables.TelegramPW = reader.GetString("TelegramPW");
                                    StatusLabel.Text = "Loading Telegram Scanner Data: Telegram Password";

                                    GlobalVariables.ScriptsPath = reader.GetString("ScriptsPath");
                                    StatusLabel.Text = "Loading Telegram Scanner Data: Scripts File Path";

                                    GlobalVariables.TelegramAPIKey = reader.GetString("TelegramAPIID");
                                    StatusLabel.Text = "Loading Telegram Scanner Data: Telegram API";

                                    GlobalVariables.TelegramAPIHash = reader.GetString("TelegramAPIHash");
                                    StatusLabel.Text = "Loading Telegram Scanner Data: TelegramAPIHash";

                                    GlobalVariables.TelegramDelay = reader.GetInt32("TelegramDelay");
                                    StatusLabel.Text = "Loading Telegram Scanner Data: Telegram Delay";

                                    GlobalVariables.TelegramTimespan = reader.GetInt32("TelegramTimespan");
                                    StatusLabel.Text = "Loading Telegram Scanner Data: TelegramTimespan";

                                    GlobalVariables.PhoneNumber = reader.GetString("TelephoneNumber");
                                    StatusLabel.Text = "Loading Telegram Scanner Data: Telephone Number";

                                    GlobalVariables.CurationDelayTime = reader.GetInt32("CurationDelayTime");
                                    StatusLabel.Text = "Loading Dragnet Curator Data: CurationDelayTime";

                                    GlobalVariables.CurationHistoryTime = reader.GetDecimal("CurationHistoryTime");
                                    StatusLabel.Text = "Loading Dragnet Curator Data: CurationHistoryTime";

                                    GlobalVariables.CuratorPath = reader.GetString("CuratorPath");
                                    StatusLabel.Text = "Loading Dragnet Curator Data: Curator Filepath";

                                    GlobalVariables.KrakenScannerHost = reader.GetString("KrakenHost");
                                    StatusLabel.Text = "Loading Kraken Scanner Data: KrakenHost";

                                    GlobalVariables.KrakenScannerUser = reader.GetString("KrakenScannerUser");
                                    StatusLabel.Text = "Loading Kraken Scanner Data: KrakenScannerUser";

                                    GlobalVariables.KrakenScannerPW = reader.GetString("KrakenScannerPW");
                                    StatusLabel.Text = "Loading Kraken Scanner Data: KrakenScannerPW";

                                    GlobalVariables.KrakenScannerPort1 = reader.GetInt32("KrakenScannerPort1");
                                    StatusLabel.Text = "Loading Kraken Scanner Data: KrakenScannerPort1";

                                    GlobalVariables.KrakenScannerPort2 = reader.GetInt32("KrakenScannerPort2");
                                    StatusLabel.Text = "Loading Kraken Scanner Data: KrakenScannerPort2";

                                    GlobalVariables.KrakenPath = reader.GetString("KrakenPath");
                                    StatusLabel.Text = "Loading Kraken Scanner Data: Kraken Scanner Filepath";

                                    GlobalVariables.KrakenAPIKey = reader.GetString("KrakenAPI");
                                    StatusLabel.Text = "Loading Kraken Scanner Data: Kraken API Key";

                                    GlobalVariables.KrakenPrivateKey = reader.GetString("KrakenPrivateKey");
                                    StatusLabel.Text = "Loading Kraken Scanner Data: Kraken Private Key";

                                    GlobalVariables.KrakenPath = reader.GetString("KrakenPath");
                                    StatusLabel.Text = "Loading Kraken Scanner Data: Kraken Scanner Filepath";

                                    GlobalVariables.BinanceHost = reader.GetString("BinanceHost");
                                    StatusLabel.Text = "Loading Binance Scanner Data: Binance Scanner Hostname";

                                    GlobalVariables.BinanceUser = reader.GetString("BinanceUser");
                                    StatusLabel.Text = "Loading Binance Scanner Data: Binance Scanner Username";

                                    GlobalVariables.BinancePW = reader.GetString("BinancePW");
                                    StatusLabel.Text = "Loading Binance Scanner Data: Binance Scanner Password";

                                    GlobalVariables.BinancePort1 = reader.GetInt32("BinancePort1");
                                    StatusLabel.Text = "Loading Binance Scanner Data: Binance Scanner Port1";

                                    GlobalVariables.BinancePort2 = reader.GetInt32("BinancePort2");
                                    StatusLabel.Text = "Loading Binance Scanner Data: Binance Scanner Port2";

                                    GlobalVariables.BinancePath = reader.GetString("BinancePath");
                                    StatusLabel.Text = "Loading Binance Scanner Data: Binance File Path";

                                    GlobalVariables.BinanceAPI = reader.GetString("BinanceAPI");
                                    StatusLabel.Text = "Loading Binance Scanner Data: Binance Scanner API";

                                    GlobalVariables.BinanceSecret = reader.GetString("BinanceSecret");
                                    StatusLabel.Text = "Loading Binance Scanner Data: Binance Scanner Secret";

                                    GlobalVariables.CongressImagesFilepath = reader.GetString("CongressImagesFilePath");
                                    StatusLabel.Text = "Loading Congressional Database Filepath";

                                    GlobalVariables.LLMHost = reader.GetString("LLMHost");
                                    StatusLabel.Text = "Loading LLM Host Configuration: LLMHost";

                                    GlobalVariables.LLMPort = reader.GetString("LLMPort");
                                    StatusLabel.Text = "Loading LLM Host Configuration: LLMPort";

                                    GlobalVariables.LLMModel = reader.GetString("LLMModel");
                                    StatusLabel.Text = "Loading LLM Host Configuration: LLMModel";

                                    GlobalVariables.LLMContextWindow = reader.GetInt32("LLMContextWindow");
                                    StatusLabel.Text = "Loading LLM Host Configuration: LLMContextWindow";

                                    GlobalVariables.DataDumpIP = reader.GetString("DataDumpIP");
                                    StatusLabel.Text = "Loading Data Dump Configuration: DataDumpIP";

                                    GlobalVariables.DataDumpUser = reader.GetString("DataDumpUser");
                                    StatusLabel.Text = "Loading Data Dump Configuration: DataDumpUser";

                                    GlobalVariables.DataDumpPW = reader.GetString("DataDumpPassword");
                                    StatusLabel.Text = "Loading Data Dump Configuration: DataDumpPassword";

                                    GlobalVariables.ActiveLLMPrompt = reader.GetString("LLMPromptName");
                                    StatusLabel.Text = "Loading Active LLM Prompt Configuration: LLMPromptName";

                                    GlobalVariables.ActiveLLMPromptVersion = reader.GetString("LLMPromptVersion");
                                    StatusLabel.Text = "Loading Active LLM Prompt Configuration: LLMPromptVersion";

                                    GlobalVariables.DragnetDBConnect = $"server={GlobalVariables.DragnetDBIP};uid={GlobalVariables.DragnetDBUser};password={GlobalVariables.DragnetDBPassword};database={GlobalVariables.DragnetDBName}";
                                    GlobalVariables.AssetDBConnect = $"server={GlobalVariables.assetIP};uid={GlobalVariables.assetUser};password={GlobalVariables.assetPW};database={GlobalVariables.assetDBName}";
                                    GlobalVariables.NewsDBConnect = $"server={GlobalVariables.newsIP};uid={GlobalVariables.newsUser};password={GlobalVariables.newsPW};database=newsdata";
                                    GlobalVariables.ControlDBConnect =$"server={GlobalVariables.DragnetControlIP};uid={GlobalVariables.DragnetControlUser};password={GlobalVariables.DragnetControlPassword};database={GlobalVariables.DragnetControlName}";
                                }
                            }
                            else
                            {
                                StatusLabel.Text = "Required resources missing.";
                            }



                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error: {0}", ex.ToString());
                }
                this.Close();
                var mainControl = FormManager.MainControl;
                mainControl.Show();

            }

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}


    


    

