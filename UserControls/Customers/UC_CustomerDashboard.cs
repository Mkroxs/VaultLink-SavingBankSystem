using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VaultLinkBankSystem.Forms.Customer;

namespace VaultLinkBankSystem.UserControls.Customers
{
    public partial class UC_CustomerDashboard : UserControl
    {
        public UC_CustomerDashboard()
        {
            InitializeComponent();
        }

        private void guna2HtmlLabel8_Click(object sender, EventArgs e)
        {
        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnMySavings_Click(object sender, EventArgs e)
        {
            frmCustomerDashboard form = this.FindForm() as frmCustomerDashboard;

            if (form != null)
            {
                form.ShowMySavings();
            }
        }

        private void btnTransactionHistory_Click(object sender, EventArgs e)
        {
            frmCustomerDashboard form = this.FindForm() as frmCustomerDashboard;

            if (form != null)
            {
                form.ShowTransactionHistory();
            }
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            frmCustomerDashboard form = this.FindForm() as frmCustomerDashboard;

            if (form != null)
            {
                form.ShowProfile();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            frmLogin login = new frmLogin();
            login.Show();

            Form parent = this.FindForm();
            if (parent != null)
                parent.Close();
        }
    }
}
