using System;
using System.Windows.Forms;
using DragnetControl.Configuration;

namespace DragnetControl
{
    public partial class AssetLoadingScreen : Form
    {
        private readonly ConfigurationLoader _loader;
        private readonly string _username;
        private RuntimeSessionState? _sessionState;

        public AssetLoadingScreen(ConfigurationLoader loader, string username)
        {
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
            _username = username ?? throw new ArgumentNullException(nameof(username));

            InitializeComponent();
            Load += AssetLoadingScreen_Load;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
        }

        public RuntimeSessionState SessionState => _sessionState ?? throw new InvalidOperationException("Configuration has not completed loading.");

        private async void AssetLoadingScreen_Load(object sender, EventArgs e)
        {
            var progress = new Progress<ConfigurationProgress>(UpdateProgress);

            try
            {
                _sessionState = await _loader.LoadAsync(_username, progress).ConfigureAwait(true);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                StatusLabel.Text = "Failed to load configuration.";
                MessageBox.Show(
                    this,
                    $"Unable to load configuration data.\n\nDetails:\n{ex.Message}",
                    "Configuration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                DialogResult = DialogResult.Abort;
            }
            finally
            {
                BeginInvoke(new Action(Close));
            }
        }

        private void UpdateProgress(ConfigurationProgress progress)
        {
            StatusLabel.Text = progress.Message;
            progressBar1.Value = Math.Max(progressBar1.Minimum, Math.Min(progressBar1.Maximum, progress.Value));
        }
    }
}
