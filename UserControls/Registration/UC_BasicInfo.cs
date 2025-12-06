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
    public partial class UC_BasicInfo : UserControl
    {
        private string gender;
        public UC_BasicInfo()
        {
            InitializeComponent();
            rbtMale.Checked = true;

            cbxCivilStatus.SelectedIndex = 0;
            dtpBirthdate.MaxDate = DateTime.Now.AddYears(-18);
        }

        private void UC_BasicInfo_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
        }

        private void tbxName_TextChanged(object sender, EventArgs e)
        {

        }


        public string CustomerName
        {
            get { return tbxName.Text; }
        }
        public string CustomerEmail
        {
            get { return tbxEmail.Text; }
        }
        public DateTime CustomerBirthDate
        {
            get { return dtpBirthdate.Value; }
        }
        public string CustomerContactNumber
        {
            get { return tbxContactNumber.Text; }
        }
        public string CustomerCivilStatus
        {
            get { return cbxCivilStatus.SelectedItem?.ToString() ?? string.Empty; }
        }
        public string CustomerGender
        {
            get {
                return gender = rbtMale.Checked ? "Male" : "Female"; 
            }
        }


        private void guna2Panel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void rbtMale_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tbxContactNumber_TextChanged(object sender, EventArgs e)
        {
            string contact = tbxContactNumber.Text;

            if (!string.IsNullOrEmpty(contact) && contact.StartsWith("+"))
            {
                tbxContactNumber.MaxLength = 13; // +639XXXXXXXXX
            }
            else
            {
                tbxContactNumber.MaxLength = 11; // 09XXXXXXXXX
            }
        }

        private void tbxContactNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow Backspace and other controls
            if (char.IsControl(e.KeyChar))
                return;

            // Allow only one leading +
            if (e.KeyChar == '+')
            {
                // Reject + if already typed OR cursor not at first position
                if (tbxContactNumber.Text.Contains("+") || tbxContactNumber.SelectionStart != 0)
                    e.Handled = true;

                return;
            }

            // Digits only
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }
}
