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

        public string Street => tbxStreetName.Text.Trim();
        public string Barangay => tbxBarangay.Text.Trim();
        public string City => tbxCity.Text.Trim();
        public string Province => tbxProvince.Text.Trim();
        public string ZipCode => tbxZipCode.Text.Trim();

        public string CustomerAddress
        {
            get
            {
                return Street + ", " + Barangay + ", " + City + ", " + Province + ", " + ZipCode;
            }
        }

        private void UC_AddressInfo_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
        }

        private void tbxZipCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbxZipCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void guna2Shapes1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel15_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
