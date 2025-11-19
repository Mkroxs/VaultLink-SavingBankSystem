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
        public UC_BasicInfo()
        {
            InitializeComponent();
        }

        private void UC_BasicInfo_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
        }
    }
}
