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

namespace VaultLinkBankSystem.UserControls.Customer
{
    public partial class UC_CustomerMySavings : UserControl
    {
        public UC_CustomerMySavings()
        {
            InitializeComponent();
        }

        private void guna2HtmlLabel8_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconPictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            frmCustomerDashboard form = this.FindForm() as frmCustomerDashboard;

            if (form != null)
            {
                form.ShowDashboard();
            }
        }

        private void showBalance_Click(object sender, EventArgs e)
        {
            if (showBalance.IconChar == FontAwesome.Sharp.IconChar.EyeSlash)
            {
                showBalance.IconChar = FontAwesome.Sharp.IconChar.Eye;
                showBalance.IconSize = 45;
                
            }
            else
            {
                showBalance.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
                showBalance.IconSize = 46;
                
            }
        }

        private void showAccountNumber_Click(object sender, EventArgs e)
        {
            if (showAccountNumber.IconChar == FontAwesome.Sharp.IconChar.EyeSlash)
            {
                showAccountNumber.IconChar = FontAwesome.Sharp.IconChar.Eye;
                showAccountNumber.IconSize = 45;
            }
            else
            {
                showAccountNumber.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
                showAccountNumber.IconSize = 46;
            }
        }
    }
}
