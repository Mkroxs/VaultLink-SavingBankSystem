using System;
using System.Windows.Forms;
using System.Drawing;

namespace VaultLinkBankSystem.UserControls.Customers
{
    public partial class UC_CustomerProfile : UserControl
    {
        public UC_CustomerProfile()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.UserPaint
                     | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.ResizeRedraw, true);

            typeof(Control).GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(this, true, null);

            


            // THIS was your bug — incorrect braces.
            // FIXED:
            foreach (Control c in this.Controls)
                DisableGunaEffects(c);
        }

        private void DisableGunaEffects(Control c)
        {
            // Panels
            if (c is Guna.UI2.WinForms.Guna2Panel p)
            {
                p.ShadowDecoration.Enabled = false;
                p.UseTransparentBackground = false;
            }

            // Textboxes
            if (c is Guna.UI2.WinForms.Guna2TextBox t)
            {
                t.ShadowDecoration.Enabled = false;
                t.Animated = false;
            }

            // Buttons
            if (c is Guna.UI2.WinForms.Guna2Button b)
            {
                b.ShadowDecoration.Enabled = false;
                b.Animated = false;
            }

            // Labels — FIX for the white background
            if (c is Guna.UI2.WinForms.Guna2HtmlLabel h)
            {
                // Remove white background
                h.BackColor = Color.Transparent;

                // Guna2HtmlLabel has an internal FillColor — we turn it off by using Style
                h.ForeColor = h.ForeColor; // keep text color
                h.Enabled = true;
                h.AutoSize = true;

                // This forces Guna2HtmlLabel to actually respect transparency
                h.UseGdiPlusTextRendering = true;
            }

            // Recurse to children
            foreach (Control child in c.Controls)
                DisableGunaEffects(child);
        }



        private void iconButton1_Click(object sender, EventArgs e)
        {
            var f = new VaultLinkBankSystem.Forms.CustomersFolder.frmCustomerChangePassword();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            var f = new VaultLinkBankSystem.Forms.CustomersFolder.frmCustomerPIN();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();
        }

        private void btnBackToDashboard_Click(object sender, EventArgs e)
        {
            var form = this.FindForm() as VaultLinkBankSystem.Forms.Customer.frmCustomerDashboard;
            if (form != null) form.ShowDashboard();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
        }
    }
}
