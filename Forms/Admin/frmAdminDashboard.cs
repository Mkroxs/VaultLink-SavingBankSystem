using System;
using System.Windows.Forms;
using VaultLinkBankSystem.UserControls.Admin;
using VaultLinkBankSystem.Helpers;

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

            this.SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint
                        | System.Windows.Forms.ControlStyles.UserPaint
                        | System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            UiHelpers.EnableDoubleBufferingRecursive(this);
        }

        private void frmAdminDashboard_Load(object sender, EventArgs e)
        {
            UiHelpers.EnableDoubleBufferingRecursive(panelMain);
            UiHelpers.EnableDoubleBufferingRecursive(panelSidebar);

            UiHelpers.TryEnableComposited(this);

            CreateAndPreloadUserControls();

            UiHelpers.ShowPage(panelMain, _ucDashboard, ref _currentPage);
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

            // Set Dock = Fill for all user controls so they respect the panel's padding
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
            UiHelpers.PreloadPages(panelMain,
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

            // Wire up sidebar navigation events
            _sidebar.DashboardClicked += (s, e) => UiHelpers.ShowPage(panelMain, _ucDashboard, ref _currentPage);
            _sidebar.CustomerManagementClicked += (s, e) => UiHelpers.ShowPage(panelMain, _ucCustomerManagement, ref _currentPage);
            _sidebar.AccountManagementClicked += (s, e) => UiHelpers.ShowPage(panelMain, _ucAccountManagement, ref _currentPage);
            _sidebar.WithdrawClicked += (s, e) => UiHelpers.ShowPage(panelMain, _ucWithdraw, ref _currentPage);
            _sidebar.DepositClicked += (s, e) => UiHelpers.ShowPage(panelMain, _ucDeposits, ref _currentPage);
            _sidebar.TransferClicked += (s, e) => UiHelpers.ShowPage(panelMain, _ucTransfer, ref _currentPage);
            _sidebar.ReportsClicked += (s, e) => UiHelpers.ShowPage(panelMain, _ucReports, ref _currentPage);
            _sidebar.VerifyKYCClicked += (s, e) => UiHelpers.ShowPage(panelMain, _ucVerifyKYC, ref _currentPage);
            _sidebar.InterestComputationClicked += (s, e) => UiHelpers.ShowPage(panelMain, _ucInterestComputation, ref _currentPage);
        }

        public void ShowPage(UserControl page)
        {
            UiHelpers.ShowPage(panelMain, page, ref _currentPage);
        }


    }
}