using System;
using System.Windows.Forms;
using VaultLinkBankSystem.Forms.Customer;
using VaultLinkBankSystem.Forms.CustomersFolder;

namespace VaultLinkBankSystem.UserControls.Customers
{
    public partial class UC_CustomerProfile : UserControl
    {
        public UC_CustomerProfile()
        {
            InitializeComponent();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            frmCustomerChangePassword changePassword = new frmCustomerChangePassword();
            changePassword.StartPosition = FormStartPosition.CenterScreen;
            changePassword.ShowDialog();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            frmCustomerPIN changePin = new frmCustomerPIN();
            changePin.StartPosition = FormStartPosition.CenterScreen;
            changePin.ShowDialog();
        }

        private void btnBackToDashboard_Click(object sender, EventArgs e)
        {
            frmCustomerDashboard form = this.FindForm() as frmCustomerDashboard;

            if (form != null)
            {
                form.ShowDashboard();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
        }
    }
}
