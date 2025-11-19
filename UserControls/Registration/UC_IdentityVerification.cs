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
    public partial class UC_IdentityVerification : UserControl
    {
        public UC_IdentityVerification()
        {
            InitializeComponent();
        }

        private void guna2Panel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void UC_IdentityVerification_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
        }
    }
}
