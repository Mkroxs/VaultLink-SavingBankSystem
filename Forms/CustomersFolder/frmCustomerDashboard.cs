using System;
using System.Windows.Forms;
using VaultLinkBankSystem.Helpers;
using VaultLinkBankSystem.UserControls.Customer;
using VaultLinkBankSystem.UserControls.Customers;

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

            UiHelpers.ShowPage(panelMain, _ucDashboard, ref _currentPage);
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

            UiHelpers.PreloadPages(panelMain,
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
            UiHelpers.ShowPage(panelMain, _ucMySavings, ref _currentPage);
        }

        public void ShowDashboard()
        {
            UiHelpers.ShowPage(panelMain, _ucDashboard, ref _currentPage);
        }

        public void ShowTransactionHistory()
        {
            UiHelpers.ShowPage(panelMain, _ucTransactionHistory, ref _currentPage);
        }

        public void ShowProfile()
        {
            UiHelpers.ShowPage(panelMain, _ucProfile, ref _currentPage);
        }
    }
}
