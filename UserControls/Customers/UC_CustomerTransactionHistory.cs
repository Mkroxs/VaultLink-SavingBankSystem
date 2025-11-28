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
    public partial class UC_CustomerTransactionHistory : UserControl
    {
        public UC_CustomerTransactionHistory()
        {
            InitializeComponent();

            if (btnProfile != null)
            {
                btnProfile.Click += btnProfile_Click;
            }
        }


        private void UC_CustomerTransactionHistory_Load(object sender, EventArgs e)
        {

        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
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
    }
}
