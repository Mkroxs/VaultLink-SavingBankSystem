using FontAwesome.Sharp;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VaultLinkBankSystem;
using static Syncfusion.Windows.Forms.TabBar;

namespace VaultLinkBankSystem.Forms.Admin
{
    public partial class frmViewDetails : Form
    {
        private VaultLinkBankSystem.Customer _customer;
        private CustomerRepository _customerRepo;
        private bool _isEditing;

        private Panel overlayPanel;
        private IconPictureBox editIcon;

        public frmViewDetails(VaultLinkBankSystem.Customer customer)
        {
            InitializeComponent();
            _customerRepo = new CustomerRepository();
            _isEditing = false;
            _customer = customer;

            overlayPanel = new Panel
            {
                BackColor = Color.FromArgb(120, 11, 30, 57),
                Visible = false,
                Dock = DockStyle.Fill,
                Cursor = Cursors.Hand,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Enabled = false
            };

            editIcon = new IconPictureBox
            {
                IconChar = IconChar.Edit,
                IconColor = Color.White,
                IconSize = 64,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(64, 64),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.None,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Enabled = false
            };

            overlayPanel.Controls.Add(editIcon);
            pbCustomerPicture.Controls.Add(overlayPanel);
            overlayPanel.BringToFront();

            overlayPanel.Resize += (s, e) =>
            {
                editIcon.Location = new Point(
                    (overlayPanel.Width - editIcon.Width) / 2,
                    (overlayPanel.Height - editIcon.Height) / 2
                );
            };

            pbCustomerPicture.MouseEnter += (s, e) =>
            {
                if (_isEditing) overlayPanel.Visible = true;
            };

            overlayPanel.MouseEnter += (s, e) =>
            {
                if (_isEditing) overlayPanel.Visible = true;
            };

            overlayPanel.MouseLeave += (s, e) =>
            {
                if (!_isEditing) return;
                Point cursor = pbCustomerPicture.PointToClient(Cursor.Position);
                if (!pbCustomerPicture.ClientRectangle.Contains(cursor))
                {
                    overlayPanel.Visible = false;
                }
            };

            pbCustomerPicture.MouseLeave += (s, e) =>
            {
                if (!_isEditing) return;
                Point cursor = pbCustomerPicture.PointToClient(Cursor.Position);
                if (!overlayPanel.ClientRectangle.Contains(cursor))
                {
                    overlayPanel.Visible = false;
                }
            };

            this.DoubleBuffered = true;
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, overlayPanel, new object[] { true });

            editIcon.Click += (s, e) =>
            {
                if (!_isEditing) return;

                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        pbCustomerPicture.Image = Image.FromFile(dlg.FileName);
                        pbCustomerPicture.SizeMode = PictureBoxSizeMode.StretchImage;

                        overlayPanel.Visible = false;
                    }
                }
            };
        }

        private void guna2Panel8_Paint(object sender, PaintEventArgs e) { }

        private void iconPictureBox2_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void guna2HtmlLabel19_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel20_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel21_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel22_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel23_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel18_Click(object sender, EventArgs e) { }
        private void guna2HtmlLabel12_Click(object sender, EventArgs e) { }

        private void enableTextBox()
        {
            tbxCivilStatus.Enabled = true;
            tbxGender.Enabled = true;
            tbxDateOfBirth.Enabled = true;
            tbxContactNumber.Enabled = true;
            tbxEmailAddress.Enabled = true;
            tbxStreetName.Enabled = true;
            tbxBarangay.Enabled = true;
            tbxCity.Enabled = true;
            tbxProvince.Enabled = true;
            tbxZipCode.Enabled = true;
        }

        private void disableTextBox()
        {
            tbxCivilStatus.Enabled = false;
            tbxGender.Enabled = false;
            tbxDateOfBirth.Enabled = false;
            tbxContactNumber.Enabled = false;
            tbxEmailAddress.Enabled = false;
            tbxStreetName.Enabled = false;
            tbxBarangay.Enabled = false;
            tbxCity.Enabled = false;
            tbxProvince.Enabled = false;
            tbxZipCode.Enabled = false;
        }

        private void iconEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_isEditing)
                {
                    _isEditing = true;
                    enableTextBox();

                    overlayPanel.Enabled = true;
                    editIcon.Enabled = true;
                    overlayPanel.Visible = false;

                    try
                    {
                        iconEdit.IconChar = IconChar.Save;
                        iconEdit.IconColor = Color.FromArgb(20, 140, 20);
                        btnResetPassword.Enabled = true;
                        btnResetPIN.Enabled = true;
                    }
                    catch { }

                    return;
                }

                if (Tag == null)
                {
                    MessageBox.Show("Customer identifier not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(Tag.ToString(), out int customerId))
                {
                    MessageBox.Show("Invalid customer identifier.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var customer = _customerRepo.GetCustomerById(customerId);
                if (customer == null)
                {
                    MessageBox.Show("Customer not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                customer.CivilStatus = tbxCivilStatus.Text?.Trim();
                customer.Gender = tbxGender.Text?.Trim();

                if (DateTime.TryParse(tbxDateOfBirth.Text?.Trim(), out DateTime dob))
                {
                    customer.BirthDate = dob;
                }

                customer.Phone = tbxContactNumber.Text?.Trim();
                customer.Email = tbxEmailAddress.Text?.Trim();

                var addressParts = new[] {
                    tbxStreetName.Text?.Trim(),
                    tbxBarangay.Text?.Trim(),
                    tbxCity.Text?.Trim(),
                    tbxProvince.Text?.Trim(),
                    tbxZipCode.Text?.Trim()
                }.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

                customer.Address = string.Join(", ", addressParts);

                bool updated = _customerRepo.UpdateCustomer(customer);

                if (updated)
                {
                    MessageBox.Show("Customer details saved.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _isEditing = false;
                    disableTextBox();

                    overlayPanel.Enabled = false;
                    editIcon.Enabled = false;
                    overlayPanel.Visible = false;

                    try
                    {
                        iconEdit.IconChar = IconChar.Edit;
                        iconEdit.IconColor = Color.DimGray;
                        btnResetPassword.Enabled = false;
                        btnResetPIN.Enabled = false;
                    }
                    catch { }
                }
                else
                {
                    MessageBox.Show("No changes were saved.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving customer details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmViewDetails_Load(object sender, EventArgs e)
        {
            if (_customer != null)
            {
                if (!_customer.IsKYCVerified)
                {
                    lblKYCStatus.Text = "Not Verified";
                    lblKYCStatus.ForeColor = Color.Red;
                }
                else
                {
                    lblKYCStatus.Text = "Verified";
                    lblKYCStatus.ForeColor = Color.Green;
                }

                lblCustomerCode.Text = _customer.CustomerCode;
                lblFullName.Text = _customer.FullName;

                tbxCivilStatus.Text = _customer.CivilStatus;
                tbxGender.Text = _customer.Gender;
                tbxDateOfBirth.Text = _customer.BirthDate.HasValue
                    ? _customer.BirthDate.Value.ToString("MMMM dd, yyyy")
                    : "N/A";

                tbxContactNumber.Text = _customer.Phone;
                tbxEmailAddress.Text = _customer.Email;

                if (!string.IsNullOrEmpty(_customer.Address))
                {
                    string[] parts = _customer.Address.Split(',');
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

                if (!string.IsNullOrEmpty(_customer.ImagePath) && System.IO.File.Exists(_customer.ImagePath))
                {
                    try
                    {
                        pbCustomerPicture.Image = Image.FromFile(_customer.ImagePath);
                        pbCustomerPicture.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch
                    {
                        pbCustomerPicture.Image = null;
                    }
                }

                lblRegisteredDate.Text = _customer.CreatedAt.ToString("MMMM dd, yyyy");
            }
        }

        private void pbCustomerPicture_Click(object sender, EventArgs e)
        {
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {

        }

        private void btnResetPIN_Click(object sender, EventArgs e)
        {

        }
    }
}
