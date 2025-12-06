using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Syncfusion.Windows.Forms.TabBar;

namespace VaultLinkBankSystem.Forms.CustomersFolder
{
    public partial class frmCustomerChangePassword : Form
    {
        private VaultLinkBankSystem.Customer _currentCustomer;
        private CustomerRepository _customerRepo;
        public frmCustomerChangePassword(VaultLinkBankSystem.Customer customer)
        {
            InitializeComponent();
            _currentCustomer = customer;
            _customerRepo = new CustomerRepository();
        }

        private void frmCustomerChangePassword_Load(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string oldPass = txtPreviousPassword.Text.Trim();
            string newPass = txtNewPassword.Text.Trim();
            string confirmPass = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(oldPass) ||
                string.IsNullOrWhiteSpace(newPass) ||
                string.IsNullOrWhiteSpace(confirmPass))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (newPass.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("New password and confirm password do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                bool result = _customerRepo.UpdateCustomerPassword(_currentCustomer.CustomerID, oldPass, newPass);

                if (result)
                {
                    _customerRepo.ClearMustChangePassword(_currentCustomer.CustomerID);
                    MessageBox.Show("Password updated successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update password:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtConfirmPassword.Clear();
            txtNewPassword.Clear();
            txtPreviousPassword.Clear();
        }

        private void btnConfirm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevent the 'ding' sound
                btnConfirm.PerformClick();    // Trigger the button click
            }
        }

        private void txtNewPassword_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void iconPictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void TogglePassword(Guna.UI2.WinForms.Guna2TextBox textbox, FontAwesome.Sharp.IconPictureBox iconButton)
        {
            if (textbox.PasswordChar == '*')
            {
                // SHOW PASSWORD
                textbox.PasswordChar = '\0';  // removes masking
                iconButton.IconChar = FontAwesome.Sharp.IconChar.Eye;
            }
            else
            {
                // HIDE PASSWORD
                textbox.PasswordChar = '*';
                iconButton.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            }
        }


        private void showPrevPass_Click(object sender, EventArgs e)
        {
            TogglePassword(txtPreviousPassword, showPrevPass);

        }

        private void showNewPass_Click(object sender, EventArgs e)
        {
            TogglePassword(txtNewPassword, showNewPass);


        }

        private void showConfirmPass_Click(object sender, EventArgs e)
        {
            TogglePassword(txtConfirmPassword, showConfirmPass);
        }
    }
}
