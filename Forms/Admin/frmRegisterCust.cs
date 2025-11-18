using System;
using System.Windows.Forms;
using VaultLinkBankSystem.UserControls.Registration;
using VaultLinkBankSystem.Helpers;

namespace VaultLinkBankSystem.Forms.Admin
{
    public partial class frmRegisterCust : Form
    {
        // Keep one instance of each registration step
        private UC_BasicInfo _ucBasicInfo;
        private UC_AddressInfo _ucAddressInfo;
        private UC_IdentityVerification _ucIdentityVerification;

        // Track current visible page
        private UserControl _currentPage;

        public frmRegisterCust()
        {
            InitializeComponent();
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            // Best-effort: enable double buffering and composited style for this form
            UiHelpers.EnableDoubleBufferingRecursive(this);
            UiHelpers.TryEnableComposited(this);
        }

        private void frmRegisterCust_Load(object sender, EventArgs e)
        {
            // Instantiate pages once and preload into panels
            CreateAndPreloadUserControls();
            // Show initial registration step
            UiHelpers.ShowPage(panelMainRegister, _ucBasicInfo, ref _currentPage);
        }

        private void CreateAndPreloadUserControls()
        {
            // Instantiate all registration steps
            _ucBasicInfo = new UC_BasicInfo();
            _ucAddressInfo = new UC_AddressInfo();
            _ucIdentityVerification = new UC_IdentityVerification();

            // Apply double buffering recursively to all new controls (best-effort, includes 3rd-party controls)
            UiHelpers.EnableDoubleBufferingRecursive(_ucBasicInfo);
            UiHelpers.EnableDoubleBufferingRecursive(_ucAddressInfo);
            UiHelpers.EnableDoubleBufferingRecursive(_ucIdentityVerification);

            // Preload into panel (keeps them in Controls collection and hides them)
            UiHelpers.PreloadPages(panelMainRegister,
                _ucBasicInfo,
                _ucAddressInfo,
                _ucIdentityVerification
            );
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
            this.Hide();
        }
    }
}