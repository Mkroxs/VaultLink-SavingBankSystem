using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaultLinkBankSystem
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void iconPictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        private void iconPictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (iconPassword.IconChar == FontAwesome.Sharp.IconChar.EyeSlash)
            {
                iconPassword.IconChar = FontAwesome.Sharp.IconChar.Eye;
                iconPassword.IconSize = 41;
                tbxPassword.PasswordChar = '\0';
                tbxPassword.TextOffset = new Point(0, -2);
            }
            else
            {
                iconPassword.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
                iconPassword.IconSize = 42;
                tbxPassword.PasswordChar = '*';
                tbxPassword.TextOffset = new Point(0, 3);

            }
        }
    }
}
