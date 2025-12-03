using iText.Layout.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;
using VaultLinkBankSystem.Forms;
using VaultLinkBankSystem.Forms.Admin;
using VaultLinkBankSystem.Forms.Customer;
using VaultLinkBankSystem.Forms.CustomersFolder;
using static Syncfusion.Windows.Forms.TabBar;

namespace VaultLinkBankSystem
{

    public partial class frmLogin : Form
    {
        private AdminRepository adminRepo;
        private CustomerRepository _customerRepo;
        public frmLogin()
        {
            InitializeComponent();
            adminRepo = new AdminRepository();
            _customerRepo = new CustomerRepository();


            frmBackground bg = new frmBackground();
            bg.Show();
            bg.SendToBack();

            if (frmLoadingScreen.Instance == null)
            {
                // Create single loading screen
                new frmLoadingScreen().Show();
                frmLoadingScreen.Instance.SendToBack();
            }
            else
            {
                frmLoadingScreen.Instance.Show();
                frmLoadingScreen.Instance.SendToBack();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = tbxUsername.Text.Trim();
            string password = tbxPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email/username and password.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // TRY ADMIN LOGIN FIRST
                Admin admin = adminRepo.Login(username, password);

                if (admin != null)
                {
                    frmLoadingScreen.Instance.ShowOverlay();

                    frmAdminDashboard dashboard = new frmAdminDashboard();
                    dashboard.Show();
                    this.Hide();
                    return;
                }

                // TRY CUSTOMER LOGIN (Email + Password)
                Customer customer = _customerRepo.KioskPasswordLogin(username, password);

                if (customer != null)
                {
                    this.Hide();

                    // Check if first-time login (PIN not set)
                    bool isFirstTime = _customerRepo.IsFirstTimeLogin(customer.CustomerID);

                    // Show PIN screen
                    frmCustomerPIN pinForm = new frmCustomerPIN(customer, isFirstTime);
                    pinForm.StartPosition = FormStartPosition.CenterScreen;

                    if (pinForm.ShowDialog(this) == DialogResult.OK)
                    {
                        frmLoadingScreen.Instance.ShowOverlay();

                        frmCustomerDashboard dashboard = new frmCustomerDashboard(customer);
                        dashboard.StartPosition = FormStartPosition.CenterScreen;
                        dashboard.Show();

                        ForceBringToFront(dashboard);
                    }

                    return;
                }

                // NEITHER ADMIN NOR CUSTOMER
                MessageBox.Show("Invalid email or password.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxPassword.Clear();
                tbxPassword.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }
        }

        public void ShowLoginForm()
        {
            tbxUsername.Clear();
            tbxPassword.Clear();
            this.Show();
        }

        public void ExitApplication()
        {
            Application.Exit();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void lblUsername_Click(object sender, EventArgs e)
        {
        }

        private void iconPictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void iconPassword_Click(object sender, EventArgs e)
        {
            if (iconPassword.IconChar == FontAwesome.Sharp.IconChar.EyeSlash)
            {
                iconPassword.IconChar = FontAwesome.Sharp.IconChar.Eye;
                iconPassword.IconSize = 41;
                tbxPassword.PasswordChar = '\0';
                tbxPassword.TextOffset = new Point(0, -2);
            }
            else
            {
                iconPassword.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
                iconPassword.IconSize = 42;
                tbxPassword.PasswordChar = '*';
                tbxPassword.TextOffset = new Point(0, 3);
            }
        }


        private void iconPictureBox2_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }

        private void iconPictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        private void ForceBringToFront(Form form)
        {
            if (form == null) return;

            ShowWindow(form.Handle, SW_RESTORE);
            SetForegroundWindow(form.Handle);
            form.Activate();
            form.BringToFront();
            form.Focus();
        }
    }
}
