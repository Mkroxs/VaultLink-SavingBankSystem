using System;
using System.Drawing;
using System.Windows.Forms;
using static Syncfusion.Windows.Forms.TabBar;

namespace VaultLinkBankSystem.UserControls.Customers
{
    public partial class UC_CustomerProfile : UserControl
    {
        private VaultLinkBankSystem.Customer _currentCustomer;
        public UC_CustomerProfile(VaultLinkBankSystem.Customer customer)
        {
            InitializeComponent();
            _currentCustomer = customer;

            SetStyle(ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.UserPaint
                     | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.ResizeRedraw, true);

            typeof(Control).GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(this, true, null);

           
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
            var f = new VaultLinkBankSystem.Forms.CustomersFolder.frmCustomerChangePassword(_currentCustomer);
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            var f = new VaultLinkBankSystem.Forms.CustomersFolder.frmCustomerPIN(_currentCustomer);
            f.StartPosition = FormStartPosition.CenterScreen;

            if (f.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("PIN changed successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnBackToDashboard_Click(object sender, EventArgs e)
        {
            var form = this.FindForm() as VaultLinkBankSystem.Forms.Customer.frmCustomerDashboard;
            if (form != null) form.ShowDashboard();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
        }

        private void UC_CustomerProfile_Load(object sender, EventArgs e)
        {
            if(_currentCustomer != null)
            {
                if (!string.IsNullOrEmpty(_currentCustomer.ImagePath) && System.IO.File.Exists(_currentCustomer.ImagePath))
                {
                    try
                    {
                        pbCustomerPicture.Image = Image.FromFile(_currentCustomer.ImagePath);
                        pbCustomerPicture.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch
                    {
                        // If image fails to load, keep default
                        pbCustomerPicture.Image = null;
                    }
                }

                lblRegisteredDate.Text = _currentCustomer.CreatedAt.ToString("MMMM dd, yyyy");

                lblFullName.Text = _currentCustomer.FullName;
                
                tbxCivilStatus.Text = _currentCustomer.CivilStatus;
                tbxDateOfBirth.Text = _currentCustomer.BirthDate.HasValue
                    ? _currentCustomer.BirthDate.Value.ToString("MMMM dd, yyyy")
                    : "N/A";


                tbxGenders.Text = _currentCustomer.Gender;


                tbxContactNumber.Text = _currentCustomer.Phone;
                tbxEmailAddress.Text = _currentCustomer.Email;

                if (!string.IsNullOrEmpty(_currentCustomer.Address))
                {
                    string[] parts = _currentCustomer.Address.Split(',');
                    for (int i = 0; i < parts.Length; i++)
                        parts[i] = parts[i].Trim();

                   
                    if (parts.Length >= 5)
                    {
                        tbxStreetName.Text = parts[0];      
                        tbxBarangay.Text = parts[1];        
                        tbxCity.Text = parts[2];            
                        tbxProvince.Text = parts[3];        
                        tbxZipCode.Text = parts[4];         
                    }
                    else
                    {
                        tbxStreetName.Text = parts.Length > 0 ? parts[0] : "N/A";
                        tbxBarangay.Text = parts.Length > 1 ? parts[1] : "N/A";
                        tbxCity.Text = parts.Length > 2 ? parts[2] : "N/A";
                        tbxProvince.Text = parts.Length > 3 ? parts[3] : "N/A";
                        tbxZipCode.Text = parts.Length > 4 ? parts[4] : "N/A";
                    }
                }
                else
                {
                    tbxStreetName.Text = "N/A";
                    tbxBarangay.Text = "N/A";
                    tbxCity.Text = "N/A";
                    tbxProvince.Text = "N/A";
                    tbxZipCode.Text = "N/A";
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
