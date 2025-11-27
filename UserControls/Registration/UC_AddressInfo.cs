using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VaultLinkBankSystem.Helpers;

namespace VaultLinkBankSystem.UserControls.Registration
{
    public partial class UC_AddressInfo : UserControl
    {
        public UC_AddressInfo()
        {
            InitializeComponent();
        }

        private void guna2Shapes1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel15_Paint(object sender, PaintEventArgs e)
        {

        }

        public string CustomerAddress
        {
            get {
                return tbxStreetName.Text + ", " + tbxBarangay.Text + ", "
                    + tbxCity.Text + ", " + tbxProvince.Text + ", " + tbxZipCode.Text;
            }

        }

        private void UC_AddressInfo_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
        }
    }
}
