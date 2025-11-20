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
    }
}
