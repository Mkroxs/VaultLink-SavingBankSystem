using System;
using System.Drawing;
using System.Windows.Forms;

namespace VaultLinkBankSystem.Forms.Admin
{
    public partial class UC_AdminSidebar : UserControl
    {
        public event EventHandler DashboardClicked;
        public event EventHandler CustomerManagementClicked;
        public event EventHandler AccountManagementClicked;
        public event EventHandler WithdrawClicked;
        public event EventHandler DepositClicked;
        public event EventHandler TransferClicked;
        public event EventHandler ReportsClicked;
        public event EventHandler VerifyKYCClicked;

        private bool isExpanded = false;
        private bool isAnimating = false;
        private int step = 0;

        public UC_AdminSidebar()
        {
            InitializeComponent();

            try
            {
                btnWithdraw.Visible = false;
                btnDeposit.Visible = false;
                btnTransfer.Visible = false;

                btnWithdraw.Dock = DockStyle.Top;
                btnDeposit.Dock = DockStyle.Top;
                btnTransfer.Dock = DockStyle.Top;

                timerSlide.Interval = 20;
                timerSlide.Tick -= timerSlide_Tick;
                timerSlide.Tick += timerSlide_Tick;

                btnTransactions.Click -= btnTransactions_Click;
                btnTransactions.Click += btnTransactions_Click;

                btnDashboard.Click -= BtnDashboard_Click;
                btnDashboard.Click += BtnDashboard_Click;

                btnCustomerManagement.Click -= BtnCustomerManagement_Click;
                btnCustomerManagement.Click += BtnCustomerManagement_Click;

                btnAccountManagement.Click -= BtnAccountManagement_Click;
                btnAccountManagement.Click += BtnAccountManagement_Click;

                btnWithdraw.Click -= BtnWithdraw_Click;
                btnWithdraw.Click += BtnWithdraw_Click;

                btnDeposit.Click -= BtnDeposit_Click;
                btnDeposit.Click += BtnDeposit_Click;

                btnTransfer.Click -= BtnTransfer_Click;
                btnTransfer.Click += BtnTransfer_Click;

                btnReports.Click -= BtnReports_Click;
                btnReports.Click += BtnReports_Click;

                btnVerifyKYC.Click -= BtnVerifyKYC_Click;
                btnVerifyKYC.Click += BtnVerifyKYC_Click;
            }
            catch
            {
            }
        }

        private void timerSlide_Tick(object sender, EventArgs e)
        {
            isAnimating = true;

            if (!isExpanded)
            {
                if (step == 0) btnWithdraw.Visible = true;
                else if (step == 1) btnDeposit.Visible = true;
                else if (step == 2) btnTransfer.Visible = true;

                step++;

                if (step > 2)
                {
                    timerSlide.Stop();
                    isExpanded = true;
                    isAnimating = false;
                    step = 0;

                    try
                    {
                        btnTransactions.CustomImages.Image = Properties.Resources.arrow_drop_down__1_;
                    }
                    catch { }
                }
            }
            else
            {
                if (step == 0) btnTransfer.Visible = false;
                else if (step == 1) btnDeposit.Visible = false;
                else if (step == 2) btnWithdraw.Visible = false;

                step++;

                if (step > 2)
                {
                    timerSlide.Stop();
                    isExpanded = false;
                    isAnimating = false;
                    step = 0;

                    try
                    {
                        btnTransactions.CustomImages.Image = Properties.Resources.arrow_right__1_1;
                    }
                    catch { }
                }
            }
        }

        private void btnTransactions_Click(object sender, EventArgs e)
        {
            if (isAnimating) return;
            step = 0;
            timerSlide.Start();
        }

        private void BtnDashboard_Click(object sender, EventArgs e)
        {
            DashboardClicked?.Invoke(this, EventArgs.Empty);
        }

        private void BtnCustomerManagement_Click(object sender, EventArgs e)
        {
            CustomerManagementClicked?.Invoke(this, EventArgs.Empty);
        }

        private void BtnAccountManagement_Click(object sender, EventArgs e)
        {
            AccountManagementClicked?.Invoke(this, EventArgs.Empty);
        }

        private void BtnWithdraw_Click(object sender, EventArgs e)
        {
            WithdrawClicked?.Invoke(this, EventArgs.Empty);
        }

        private void BtnDeposit_Click(object sender, EventArgs e)
        {
            DepositClicked?.Invoke(this, EventArgs.Empty);
        }

        private void BtnTransfer_Click(object sender, EventArgs e)
        {
            TransferClicked?.Invoke(this, EventArgs.Empty);
        }

        private void BtnReports_Click(object sender, EventArgs e)
        {
            ReportsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void BtnVerifyKYC_Click(object sender, EventArgs e)
        {
            VerifyKYCClicked?.Invoke(this, EventArgs.Empty);
        }

        private void timerSlide_Tick_1(object sender, EventArgs e) {  }
        private void btnTransactions_MouseClick(object sender, MouseEventArgs e) {  }
        private void guna2Button1_Click(object sender, EventArgs e) {  }
        private void guna2Button2_Click(object sender, EventArgs e) {  }
        private void btnDashboard_MouseClick(object sender, MouseEventArgs e) { DashboardClicked?.Invoke(this, EventArgs.Empty); }
        private void btnCustomerManagement_MouseClick(object sender, MouseEventArgs e) { CustomerManagementClicked?.Invoke(this, EventArgs.Empty); }
        private void btnAccountManagement_MouseClick(object sender, MouseEventArgs e) { AccountManagementClicked?.Invoke(this, EventArgs.Empty); }

        private void btnAccountManagement_Click(object sender, EventArgs e)
        {
            AccountManagementClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            DashboardClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            frmAdminDashboard frmAdminDashboard = (frmAdminDashboard)this.FindForm();
            frmLogin loginForm = new frmLogin();
            frmAdminDashboard.Hide();
            loginForm.Show();
            loginForm.BringToFront();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            ReportsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnReports_MouseClick(object sender, MouseEventArgs e)
        {
            ReportsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnVerifyKYC_Click(object sender, EventArgs e)
        {
            VerifyKYCClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnVerifyKYC_MouseClick(object sender, MouseEventArgs e)
        {
            VerifyKYCClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            WithdrawClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnWithdraw_MouseClick(object sender, MouseEventArgs e)
        {
            WithdrawClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            DepositClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnDeposit_MouseClick(object sender, MouseEventArgs e)
        {
            DepositClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            TransferClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnTransfer_MouseClick(object sender, MouseEventArgs e)
        {
            TransferClicked?.Invoke(this, EventArgs.Empty);
        }

    }
}
