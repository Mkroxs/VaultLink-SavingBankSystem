using System;
using System.Windows.Forms;
using VaultLinkBankSystem.Helpers;
using VaultLinkBankSystem.UserControls.Customer;
using VaultLinkBankSystem.UserControls.Customers;
using VaultLinkBankSystem.Forms;

namespace VaultLinkBankSystem.Forms.Customer
{
    public partial class frmCustomerDashboard : Form
    {
        private UC_CustomerDashboard _ucDashboard;
        private UC_CustomerMySavings _ucMySavings;
        private UC_CustomerTransactionHistory _ucTransactionHistory;
        private UC_CustomerProfile _ucProfile;
        private UserControl _currentPage;

        public frmCustomerDashboard()
        {
            InitializeComponent();

            frmLoadingScreen.Instance.ShowOverlay();

            this.WindowState = FormWindowState.Normal;
            this.WindowState = FormWindowState.Minimized;

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);

            this.UpdateStyles();

            UiHelpers.EnableDoubleBufferingRecursive(this);
            UiHelpers.TryEnableComposited(this);
        }

        private void frmCustomerDashboard_Load(object sender, EventArgs e)
        {
            CreateAndPreloadUserControls();

            UiHelpers.ForceRender(_ucDashboard);
            UiHelpers.ForceRender(_ucMySavings);
            UiHelpers.ForceRender(_ucTransactionHistory);
            UiHelpers.ForceRender(_ucProfile);

            _currentPage = UiHelpers.ShowPage(panelMain, _ucDashboard, _currentPage);

            UiHelpers.FixGuna2TextBoxVisibility(this);

            panelMain.PerformLayout();
            panelMain.Refresh();
            this.PerformLayout();
            this.Refresh();
        }

        private void CreateAndPreloadUserControls()
        {
            _ucDashboard = new UC_CustomerDashboard();
            _ucMySavings = new UC_CustomerMySavings();
            _ucTransactionHistory = new UC_CustomerTransactionHistory();
            _ucProfile = new UC_CustomerProfile();

            UiHelpers.PreloadPages(
                panelMain,
                _ucDashboard,
                _ucMySavings,
                _ucTransactionHistory,
                _ucProfile
            );
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Activate();
            this.BringToFront();
        }

        public void ShowMySavings()
        {
            _currentPage = UiHelpers.ShowPage(panelMain, _ucMySavings, _currentPage);
        }

        public void ShowDashboard()
        {
            _currentPage = UiHelpers.ShowPage(panelMain, _ucDashboard, _currentPage);
        }

        public void ShowTransactionHistory()
        {
            _currentPage = UiHelpers.ShowPage(panelMain, _ucTransactionHistory, _currentPage);
        }

        public void ShowProfile()
        {
            _currentPage = UiHelpers.ShowPage(panelMain, _ucProfile, _currentPage);
        }
    }
}
