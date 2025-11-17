using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VaultLinkBankSystem.Forms.Admin;

namespace VaultLinkBankSystem
{
    public partial class frmLogin : Form
    {
        AdminRepository adminRepo;
        public frmLogin()
        {
            InitializeComponent();
            adminRepo = new AdminRepository();
        }

        private void iconPictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        private void iconPictureBox1_MouseClick(object sender, MouseEventArgs e)
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = tbxUsername.Text;
            string password = tbxPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                /*if (username == "admin" && password == "admin")
                {
                    frmAdminDashboard dashboard = new frmAdminDashboard();
                    dashboard.Show();
                    this.Hide();
                }*/
                Admin admin = adminRepo.Login(username, password);
                if (admin != null)
                {
                    MessageBox.Show("Login successful! Welcome " + username,
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    /*                    frmDashBoard dashboard = new frmDashBoard(admin);
                    */
                    frmAdminDashboard dashboard = new frmAdminDashboard();

                    dashboard.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.",
                        "Login Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    tbxUsername.Clear();
                    tbxPassword.Clear();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void lblUsername_Click(object sender, EventArgs e)
        {

        }
    }
}
