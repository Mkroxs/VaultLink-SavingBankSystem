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

namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_InterestComputation : UserControl
    {
        public UC_InterestComputation()
        {
            InitializeComponent();
        }

        private void UC_InterestComputation_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
        }
    }
}
