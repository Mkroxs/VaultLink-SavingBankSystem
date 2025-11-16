using System;
using System.Drawing;
using System.Windows.Forms;

namespace VaultLinkBankSystem.Forms.Admin
{
    public partial class UC_AdminSidebar : UserControl
    {
        private bool isExpanded = false;
        private bool isAnimating = false;
        private int step = 0;

      
        public event EventHandler DashboardClicked;
        public event EventHandler CustomerManagementClicked;
        public event EventHandler AccountManagementClicked;
        public event EventHandler WithdrawClicked;
        public event EventHandler ReportsClicked;
        public event EventHandler VerifyKYCClicked;
        public UC_AdminSidebar()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            timerSlide.Tick -= timerSlide_Tick;
            timerSlide.Tick += timerSlide_Tick;

       
            btnWithdraw.Visible = false;
            btnDeposit.Visible = false;
            btnTransfer.Visible = false;
            btnVerifyKYC.Visible = false;

            btnWithdraw.Dock = DockStyle.Top;
            btnDeposit.Dock = DockStyle.Top;
            btnTransfer.Dock = DockStyle.Top;

        
            timerSlide.Interval = 20;

          
            btnTransactions.Click += btnTransactions_Click;

            btnDashboard.Click += BtnDashboard_Click;
            btnDashboard.MouseClick += BtnDashboard_MouseClick;

            btnCustomerManagement.Click += BtnCustomerManagement_Click;
            btnCustomerManagement.MouseClick += BtnCustomerManagement_MouseClick;

            
            btnAccountManagement.Click += btnAccountManagement_Click;
            btnAccountManagement.MouseClick += btnAccountManagement_MouseClick;

            btnWithdraw.Click += btnWithdraw_Click;
            btnWithdraw.MouseClick += btnWithdraw_MouseClick;

            btnReports.Click += btnReports_Click;
            btnReports.MouseClick += btnReports_MouseClick;

            btnVerifyKYC.Click += btnVerifyKYC_Click;
            btnVerifyKYC.MouseClick += btnVerifyKYC_MouseClick;
        }



        private void timerSlide_Tick(object sender, EventArgs e)
        {
            isAnimating = true;

            if (!isExpanded)
            {
              
                if (step == 0) btnWithdraw.Visible = true;
                else if (step == 1) btnDeposit.Visible = true;
                else if (step == 2) btnTransfer.Visible = true;
                else if (step == 3) btnVerifyKYC.Visible = true;

                step++;

                if (step > 3)
                {
                    timerSlide.Stop();
                    isExpanded = true;
                    isAnimating = false;
                    step = 0;

                    btnTransactions.CustomImages.Image =
                        Properties.Resources.arrow_drop_down__1_;
                }
            }
            else
            {
            
                if (step == 0) btnTransfer.Visible = false;
                else if (step == 1) btnDeposit.Visible = false;
                else if (step == 2) btnWithdraw.Visible = false;
                else if (step == 3) btnVerifyKYC.Visible = false;


                step++;

                if (step > 3)
                {
                    timerSlide.Stop();
                    isExpanded = false;
                    isAnimating = false;
                    step = 0;

                    btnTransactions.CustomImages.Image =
                        Properties.Resources.arrow_right__1_1;
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

        private void BtnDashboard_MouseClick(object sender, MouseEventArgs e)
        {
            DashboardClicked?.Invoke(this, EventArgs.Empty);
        }


     
        private void BtnCustomerManagement_Click(object sender, EventArgs e)
        {
            CustomerManagementClicked?.Invoke(this, EventArgs.Empty);
        }

        private void BtnCustomerManagement_MouseClick(object sender, MouseEventArgs e)
        {
            CustomerManagementClicked?.Invoke(this, EventArgs.Empty);
        }


        private void timerSlide_Tick_1(object sender, EventArgs e) { }
        private void btnTransactions_MouseClick(object sender, MouseEventArgs e) { }
        private void guna2Button1_Click(object sender, EventArgs e) { }
        private void guna2Button2_Click(object sender, EventArgs e) { }
        private void btnDashboard_Click_1(object sender, EventArgs e) { }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Form parent = this.FindForm();
            parent.Close();
            frmLogin login = new frmLogin();
            login.Show();
        }

        private void btnAccountManagement_Click(object sender, EventArgs e)
        {
            AccountManagementClicked?.Invoke(this, EventArgs.Empty);

        }

        private void btnAccountManagement_MouseClick(object sender, MouseEventArgs e)
        {
            AccountManagementClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            WithdrawClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnWithdraw_MouseClick(object sender, MouseEventArgs e)
        {
            WithdrawClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnReports_MouseClick(object sender, MouseEventArgs e)
        {
            ReportsClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnReports_Click(object sender, EventArgs e)
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
    }
}
