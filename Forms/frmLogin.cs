using iText.Layout.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;
using VaultLinkBankSystem.Forms;
using VaultLinkBankSystem.Forms.Admin;
using VaultLinkBankSystem.Forms.Customer;
using VaultLinkBankSystem.Forms.CustomersFolder;

namespace VaultLinkBankSystem
{
    public partial class frmLogin : Form
    {
        private AdminRepository adminRepo;

        public frmLogin()
        {
            InitializeComponent();

            adminRepo = new AdminRepository();

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
            string username = tbxUsername.Text;
            string password = tbxPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error");
                return;
            }

            try
            {
                Admin admin = adminRepo.Login(username, password);

                if (admin != null)
                {
                    frmLoadingScreen.Instance.ShowOverlay();

                    frmAdminDashboard dashboard = new frmAdminDashboard();
                    dashboard.Show();
                    this.Hide();

                    return;
                }

                if (username == "customer" && password == "customer123")
                {
                    this.Hide();

                    frmCustomerPIN pinForm = new frmCustomerPIN();
                    pinForm.StartPosition = FormStartPosition.CenterScreen;

                    if (pinForm.ShowDialog(this) == DialogResult.OK)
                    {
                        frmLoadingScreen.Instance.ShowOverlay();

                        frmCustomerDashboard dashboard = new frmCustomerDashboard();
                        dashboard.StartPosition = FormStartPosition.CenterScreen;
                        dashboard.Show();

                        ForceBringToFront(dashboard);
                    }
                    else
                    {
                        this.Show();
                    }

                    return;
                }

                MessageBox.Show("Invalid username or password.", "Login Failed");
                tbxUsername.Clear();
                tbxPassword.Clear();
            }
            catch
            {
                MessageBox.Show("An unexpected error occurred.", "Error");
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
