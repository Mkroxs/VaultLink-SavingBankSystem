using System;
using System.Windows.Forms;
using VaultLinkBankSystem.UserControls.Admin;
using VaultLinkBankSystem.Helpers;
using VaultLinkBankSystem.Forms;

namespace VaultLinkBankSystem.Forms.Admin
{
    public partial class frmAdminDashboard : Form
    {
        private UC_AdminSidebar _sidebar;
        private UC_AdminDashboard _ucDashboard;
        private UC_CustomerManagement _ucCustomerManagement;
        private UC_AccountManagement _ucAccountManagement;
        private UC_Withdraw _ucWithdraw;
        private UC_Deposits _ucDeposits;
        private UC_Transfer _ucTransfer;
        private UC_Reports _ucReports;
        private UC_VerifyKYC _ucVerifyKYC;
        private UC_InterestComputation _ucInterestComputation;

        private UserControl _currentPage;

        public frmAdminDashboard()
        {
            InitializeComponent();

            frmLoadingScreen.Instance.ShowOverlay();

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);

            this.UpdateStyles();

            UiHelpers.EnableDoubleBufferingRecursive(this);
        }

        private void frmAdminDashboard_Load(object sender, EventArgs e)
        {
            UiHelpers.EnableDoubleBufferingRecursive(panelMain);
            UiHelpers.EnableDoubleBufferingRecursive(panelSidebar);

            UiHelpers.TryEnableComposited(this);

            CreateAndPreloadUserControls();
            UiHelpers.ForceRender(_ucDashboard);
            UiHelpers.ForceRender(_ucCustomerManagement);
            UiHelpers.ForceRender(_ucAccountManagement);
            UiHelpers.ForceRender(_ucWithdraw);
            UiHelpers.ForceRender(_ucDeposits);
            UiHelpers.ForceRender(_ucTransfer);
            UiHelpers.ForceRender(_ucReports);
            UiHelpers.ForceRender(_ucVerifyKYC);
            UiHelpers.ForceRender(_ucInterestComputation);

            _currentPage = UiHelpers.ShowPage(panelMain, _ucDashboard, _currentPage);
        }

        private void CreateAndPreloadUserControls()
        {
            _sidebar = new UC_AdminSidebar();
            _ucDashboard = new UC_AdminDashboard();
            _ucCustomerManagement = new UC_CustomerManagement();
            _ucAccountManagement = new UC_AccountManagement();
            _ucWithdraw = new UC_Withdraw();
            _ucDeposits = new UC_Deposits();
            _ucTransfer = new UC_Transfer();
            _ucReports = new UC_Reports();
            _ucVerifyKYC = new UC_VerifyKYC();
            _ucInterestComputation = new UC_InterestComputation();

            UiHelpers.EnableDoubleBufferingRecursive(_sidebar);
            UiHelpers.EnableDoubleBufferingRecursive(_ucDashboard);
            UiHelpers.EnableDoubleBufferingRecursive(_ucCustomerManagement);
            UiHelpers.EnableDoubleBufferingRecursive(_ucAccountManagement);
            UiHelpers.EnableDoubleBufferingRecursive(_ucWithdraw);
            UiHelpers.EnableDoubleBufferingRecursive(_ucDeposits);
            UiHelpers.EnableDoubleBufferingRecursive(_ucTransfer);
            UiHelpers.EnableDoubleBufferingRecursive(_ucReports);
            UiHelpers.EnableDoubleBufferingRecursive(_ucVerifyKYC);
            UiHelpers.EnableDoubleBufferingRecursive(_ucInterestComputation);

            _sidebar.Dock = DockStyle.Fill;
            _ucDashboard.Dock = DockStyle.Fill;
            _ucCustomerManagement.Dock = DockStyle.Fill;
            _ucAccountManagement.Dock = DockStyle.Fill;
            _ucWithdraw.Dock = DockStyle.Fill;
            _ucDeposits.Dock = DockStyle.Fill;
            _ucTransfer.Dock = DockStyle.Fill;
            _ucReports.Dock = DockStyle.Fill;
            _ucVerifyKYC.Dock = DockStyle.Fill;
            _ucInterestComputation.Dock = DockStyle.Fill;

            UiHelpers.PreloadPages(panelSidebar, _sidebar);
            UiHelpers.PreloadPages(
                panelMain,
                _ucDashboard,
                _ucCustomerManagement,
                _ucAccountManagement,
                _ucWithdraw,
                _ucDeposits,
                _ucTransfer,
                _ucReports,
                _ucVerifyKYC,
                _ucInterestComputation
            );

            _sidebar.Visible = true;

            _sidebar.DashboardClicked += (s, e) => _currentPage = UiHelpers.ShowPage(panelMain, _ucDashboard, _currentPage);
            _sidebar.CustomerManagementClicked += (s, e) => _currentPage = UiHelpers.ShowPage(panelMain, _ucCustomerManagement, _currentPage);
            _sidebar.AccountManagementClicked += (s, e) => _currentPage = UiHelpers.ShowPage(panelMain, _ucAccountManagement, _currentPage);
            _sidebar.WithdrawClicked += (s, e) => _currentPage = UiHelpers.ShowPage(panelMain, _ucWithdraw, _currentPage);
            _sidebar.DepositClicked += (s, e) => _currentPage = UiHelpers.ShowPage(panelMain, _ucDeposits, _currentPage);
            _sidebar.TransferClicked += (s, e) => _currentPage = UiHelpers.ShowPage(panelMain, _ucTransfer, _currentPage);
            _sidebar.ReportsClicked += (s, e) => _currentPage = UiHelpers.ShowPage(panelMain, _ucReports, _currentPage);
            _sidebar.VerifyKYCClicked += (s, e) => _currentPage = UiHelpers.ShowPage(panelMain, _ucVerifyKYC, _currentPage);
            _sidebar.InterestComputationClicked += (s, e) => _currentPage = UiHelpers.ShowPage(panelMain, _ucInterestComputation, _currentPage);
        }

        public void ShowPage(UserControl page)
        {
            _currentPage = UiHelpers.ShowPage(panelMain, page, _currentPage);
        }

        private void panelTopbar_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
