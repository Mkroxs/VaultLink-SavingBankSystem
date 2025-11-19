using System;
using System.Windows.Forms;
using VaultLinkBankSystem.UserControls.Registration;
using VaultLinkBankSystem.Helpers;

namespace VaultLinkBankSystem.Forms.Admin
{
    public partial class frmRegistration : Form
    {
        // Keep one instance of each registration step
        private UC_BasicInfo _ucBasicInfo;
        private UC_AddressInfo _ucAddressInfo;
        private UC_IdentityVerification _ucIdentityVerification;

        // Track current visible page
        private UserControl _currentPage;

        // Track current step (0 = BasicInfo, 1 = AddressInfo, 2 = IdentityVerification)
        private int _currentStep = 0;

        public frmRegistration()
        {
            InitializeComponent();

            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            // Enable double buffering on the form itself
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            // Best-effort: enable double buffering and composited style for this form
            UiHelpers.EnableDoubleBufferingRecursive(this);
            UiHelpers.TryEnableComposited(this);

            // PRELOAD ALL USER CONTROLS HERE IN CONSTRUCTOR (before form is shown)
            PreloadAllUserControls();

            // Wire up the Load event
            this.Load += FrmRegistration_Load;
        }

        /// <summary>
        /// Preload all user controls in the constructor before the form is visible
        /// This ensures they are fully initialized and laid out, eliminating flicker
        /// </summary>
        private void PreloadAllUserControls()
        {
            // Suspend layout during initialization
            this.SuspendLayout();
            panelMainRegister.SuspendLayout();

            try
            {
                // Instantiate all registration steps
                _ucBasicInfo = new UC_BasicInfo();
                _ucAddressInfo = new UC_AddressInfo();
                _ucIdentityVerification = new UC_IdentityVerification();

                // Apply double buffering recursively to all new controls
                UiHelpers.EnableDoubleBufferingRecursive(_ucBasicInfo);
                UiHelpers.EnableDoubleBufferingRecursive(_ucAddressInfo);
                UiHelpers.EnableDoubleBufferingRecursive(_ucIdentityVerification);

                // Add all controls to the panel and force layout calculations
                _ucBasicInfo.Dock = DockStyle.Fill;
                _ucBasicInfo.Visible = false;
                panelMainRegister.Controls.Add(_ucBasicInfo);

                _ucAddressInfo.Dock = DockStyle.Fill;
                _ucAddressInfo.Visible = false;
                panelMainRegister.Controls.Add(_ucAddressInfo);

                _ucIdentityVerification.Dock = DockStyle.Fill;
                _ucIdentityVerification.Visible = false;
                panelMainRegister.Controls.Add(_ucIdentityVerification);

                // Force all controls to render and calculate their layouts
                panelMainRegister.PerformLayout();
                _ucBasicInfo.CreateControl();
                _ucBasicInfo.PerformLayout();

                _ucAddressInfo.CreateControl();
                _ucAddressInfo.PerformLayout();

                _ucIdentityVerification.CreateControl();
                _ucIdentityVerification.PerformLayout();
            }
            finally
            {
                // Resume layout after all changes
                panelMainRegister.ResumeLayout(false);
                this.ResumeLayout(false);
            }
        }

        private void FrmRegistration_Load(object sender, EventArgs e)
        {
            try
            {
                // Show initial registration step (BasicInfo) - already preloaded
                _currentStep = 0;
                UiHelpers.ShowPage(panelMainRegister, _ucBasicInfo, ref _currentPage);

                // Set up button click events
                btnNext.Click += BtnNext_Click;
                btnPrevious.Click += BtnPrevious_Click;
                btnRegister.Click += BtnRegister_Click;

                // Initialize button visibility for first step
                UpdateButtonVisibility();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Update button visibility based on current step
        /// Step 0 (BasicInfo): Hide Previous, Show Next, Hide Register
        /// Step 1 (AddressInfo): Show Previous, Show Next, Hide Register
        /// Step 2 (IdentityVerification): Show Previous, Hide Next, Show Register
        /// </summary>
        private void UpdateButtonVisibility()
        {
            switch (_currentStep)
            {
                case 0: // BasicInfo
                    btnPrevious.Visible = false;
                    btnNext.Visible = true;
                    btnRegister.Visible = false;
                    break;

                case 1: // AddressInfo
                    btnPrevious.Visible = true;
                    btnNext.Visible = true;
                    btnRegister.Visible = false;
                    break;

                case 2: // IdentityVerification
                    btnPrevious.Visible = true;
                    btnNext.Visible = false;
                    btnRegister.Visible = true;
                    break;
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_currentStep < 2)
            {
                _currentStep++;
                ShowCurrentStep();
            }
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentStep > 0)
            {
                _currentStep--;
                ShowCurrentStep();
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // TODO: Implement registration logic here
            MessageBox.Show("Registration submitted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Close form or reset
            // this.Close();
        }

        /// <summary>
        /// Show the current step based on _currentStep value
        /// </summary>
        private void ShowCurrentStep()
        {
            switch (_currentStep)
            {
                case 0:
                    ShowBasicInfo();
                    break;
                case 1:
                    ShowAddressInfo();
                    break;
                case 2:
                    ShowIdentityVerification();
                    break;
            }

            UpdateButtonVisibility();
        }

        // Public methods to navigate between registration steps
        public void ShowBasicInfo()
        {
            UiHelpers.ShowPage(panelMainRegister, _ucBasicInfo, ref _currentPage);
        }

        public void ShowAddressInfo()
        {
            UiHelpers.ShowPage(panelMainRegister, _ucAddressInfo, ref _currentPage);
        }

        public void ShowIdentityVerification()
        {
            UiHelpers.ShowPage(panelMainRegister, _ucIdentityVerification, ref _currentPage);
        }

        private void iconPictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}