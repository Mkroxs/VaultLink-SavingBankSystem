using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using VaultLinkBankSystem.UserControls.Admin;

namespace VaultLinkBankSystem.Forms.Admin
{
    public partial class frmAdminDashboard : Form
    {
        public frmAdminDashboard()
        {
            InitializeComponent();
        }

        private void frmAdminDashboard_Load(object sender, EventArgs e)
        {
            LoadSidebarUC();
            LoadPage(new UC_AdminDashboard());
        }

       
        public void LoadSidebarUC()
        {
            panelSidebar.Controls.Clear();
            var sidebar = new UC_AdminSidebar();
            sidebar.Dock = DockStyle.Fill;
            sidebar.DashboardClicked += (s, e) => LoadPage(new UC_AdminDashboard());
            sidebar.CustomerManagementClicked += (s, e) => LoadPage(new UC_CustomerManagement());
            sidebar.AccountManagementClicked += (s, e) => LoadPage(new UC_AccountManagement());
            sidebar.WithdrawClicked += (s, e) => LoadPage(new UC_Withdraw());
            sidebar.ReportsClicked += (s, e) => LoadPage(new UC_Reports());
            sidebar.VerifyKYCClicked += (s, e) => LoadPage(new UC_VerifyKYC());

            panelSidebar.Controls.Add(sidebar);
        }
  

        public void LoadPage(UserControl uc)
        {
            panelMain.SuspendLayout();
            panelMain.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            panelMain.Controls.Add(uc);
            panelMain.ResumeLayout();
        }


    }
}
