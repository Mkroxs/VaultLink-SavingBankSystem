using System;
using System.Windows.Forms;

namespace VaultLinkBankSystem.Forms
{
    public partial class frmLoadingScreen : Form
    {
        private static frmLoadingScreen _instance;
        public static frmLoadingScreen Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new frmLoadingScreen();
                return _instance;
            }
        }

        Timer progressTimer;

        public frmLoadingScreen()
        {
            InitializeComponent();

            if (_instance == null)
                _instance = this;

            this.Opacity = 0;
            this.TopMost = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            progressBar1.Value = 0;
            progressBar1.Maximum = 100;

            progressTimer = new Timer();
            progressTimer.Interval = 15;             // smooth speed
            progressTimer.Tick += ProgressTimer_Tick;
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < 100)
            {
                progressBar1.Value += 2;            // speed
            }
            else
            {
                progressTimer.Stop();
                HideOverlaySynced();
            }
        }

        private void HideOverlaySynced()
        {
            this.Opacity = 0;
            this.TopMost = false;
            this.SendToBack();
            progressBar1.Value = 0;
        }

        public void ShowOverlay()
        {
            this.Opacity = 1;
            this.TopMost = true;
            this.BringToFront();
            this.Show();

            progressBar1.Value = 0;

            progressTimer.Stop();
            progressTimer.Start();
        }
    }
}
