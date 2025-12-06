using FontAwesome.Sharp;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VaultLinkBankSystem;
using VaultLinkBankSystem.UserControls.Registration;
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
        private string _newImagePath = null;

        public frmViewDetails(VaultLinkBankSystem.Customer customer)
        {
            InitializeComponent();
            _customerRepo = new CustomerRepository();
            _isEditing = false;
            _customer = customer;

            this.Tag = customer.CustomerID;

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
            tbxDateOfBirth.MaxDate = DateTime.Today.AddYears(-18);

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

                    iconEdit.IconChar = IconChar.Save;
                    iconEdit.IconColor = Color.FromArgb(20, 140, 20);

                    btnResetPassword.Enabled = true;

                    bool firstTimeLogin = _customerRepo.IsFirstTimeLogin(_customer.CustomerID);
                    btnResetPIN.Enabled = !firstTimeLogin; // Enable ONLY if customer already has a PIN

                    return;
                }


                // SAVE MODE
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

                // Validate required fields
                if (string.IsNullOrWhiteSpace(tbxContactNumber.Text))
                {
                    MessageBox.Show("Contact number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(tbxEmailAddress.Text))
                {
                    MessageBox.Show("Email address is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update customer object
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

                // Update image path if new image was selected
                if (!string.IsNullOrEmpty(_newImagePath))
                {
                    customer.ImagePath = _newImagePath;
                }


                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(tbxEmailAddress.Text, emailPattern))
                {
                    MessageBox.Show("Invalid Email Address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string phonePattern = @"^(09|\+639)\d{9}$";
                if (!Regex.IsMatch(tbxContactNumber.Text, phonePattern))
                {
                    MessageBox.Show("Invalid Phone Number. Use format: 09123456789", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }



                bool updated = _customerRepo.UpdateCustomer(customer);

                if (updated)
                {
                    MessageBox.Show("Customer details saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _isEditing = false;
                    _newImagePath = null;
                    disableTextBox();

                    overlayPanel.Enabled = false;
                    editIcon.Enabled = false;
                    overlayPanel.Visible = false;

                    iconEdit.IconChar = IconChar.Edit;
                    iconEdit.IconColor = Color.DimGray;
                    btnResetPassword.Enabled = false;
                    btnResetPIN.Enabled = false;

                    // Refresh customer data
                    _customer = customer;
                }
                else
                {
                    MessageBox.Show("No changes were made.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                tbxDateOfBirth.MaxDate = DateTime.Today.AddYears(-18);

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

            // Initially disable reset buttons
            btnResetPassword.Enabled = false;
            btnResetPIN.Enabled = false;
        }

        private void pbCustomerPicture_Click(object sender, EventArgs e)
        {
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to reset the password for {_customer.FullName}?\n\n" +
                    "A temporary password will be generated and displayed.",
                    "Confirm Password Reset",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string idNumberPart = _customer.CustomerID.ToString().PadLeft(4, '0');

                    string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    Random rnd = new Random();
                    string randomPart = new string(Enumerable.Repeat(chars, 5)
                        .Select(s => s[rnd.Next(s.Length)]).ToArray());

                    string tempPassword = idNumberPart + randomPart;

                    bool updated = _customerRepo.UpdateCustomerPassword(_customer.CustomerID, tempPassword);
                    if (!updated)
                    {
                        MessageBox.Show("Failed to reset password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var passwordForm = new Form
                    {
                        Text = "Temporary Password",
                        Width = 450,
                        Height = 250,
                        StartPosition = FormStartPosition.CenterParent,
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        MaximizeBox = false,
                        MinimizeBox = false
                    };

                    var lblMessage = new Label
                    {
                        Text = $"Temporary password for {_customer.FullName}:",
                        Location = new Point(20, 20),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10)
                    };

                    var txtPassword = new TextBox
                    {
                        Text = tempPassword,
                        Location = new Point(20, 50),
                        Width = 390,
                        Font = new Font("Consolas", 14, FontStyle.Bold),
                        ReadOnly = true,
                        TextAlign = HorizontalAlignment.Center
                    };

                    var lblNote = new Label
                    {
                        Text = "⚠ Please write this down. The customer must change it on first login.",
                        Location = new Point(20, 90),
                        Width = 390,
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.DarkRed
                    };

                    var btnCopy = new Button
                    {
                        Text = "Copy to Clipboard",
                        Location = new Point(90, 140),
                        Width = 120,
                        Height = 35
                    };

                    var btnClose = new Button
                    {
                        Text = "Close",
                        Location = new Point(220, 140),
                        Width = 120,
                        Height = 35
                    };

                    btnCopy.Click += (s, ev) =>
                    {
                        Clipboard.SetText(tempPassword);
                        MessageBox.Show("Password copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    };

                    btnClose.Click += (s, ev) => passwordForm.Close();

                    passwordForm.Controls.Add(lblMessage);
                    passwordForm.Controls.Add(txtPassword);
                    passwordForm.Controls.Add(lblNote);
                    passwordForm.Controls.Add(btnCopy);
                    passwordForm.Controls.Add(btnClose);

                    passwordForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting password: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnResetPIN_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to reset the PIN for {_customer.FullName}?\n\n" +
                    "A temporary 6-digit PIN will be generated and displayed.",
                    "Confirm PIN Reset",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string tempPIN = _customerRepo.ResetCustomerPIN(_customer.CustomerID);

                    if (!string.IsNullOrEmpty(tempPIN))
                    {
                        // Show temporary PIN to admin
                        var pinForm = new Form
                        {
                            Text = "Temporary PIN",
                            Width = 450,
                            Height = 250,
                            StartPosition = FormStartPosition.CenterParent,
                            FormBorderStyle = FormBorderStyle.FixedDialog,
                            MaximizeBox = false,
                            MinimizeBox = false
                        };

                        var lblMessage = new Label
                        {
                            Text = $"Temporary PIN for {_customer.FullName}:",
                            Location = new Point(20, 20),
                            AutoSize = true,
                            Font = new Font("Segoe UI", 10)
                        };

                        var txtPIN = new TextBox
                        {
                            Text = tempPIN,
                            Location = new Point(20, 50),
                            Width = 390,
                            Font = new Font("Consolas", 24, FontStyle.Bold),
                            ReadOnly = true,
                            TextAlign = HorizontalAlignment.Center
                        };

                        var lblNote = new Label
                        {
                            Text = "⚠ Please write this down. The customer must change it on first kiosk login.",
                            Location = new Point(20, 100),
                            Width = 390,
                            Font = new Font("Segoe UI", 9),
                            ForeColor = Color.DarkRed
                        };

                        var btnCopy = new Button
                        {
                            Text = "Copy to Clipboard",
                            Location = new Point(90, 150),
                            Width = 120,
                            Height = 35
                        };

                        var btnClose = new Button
                        {
                            Text = "Close",
                            Location = new Point(220, 150),
                            Width = 120,
                            Height = 35
                        };

                        btnCopy.Click += (s, ev) =>
                        {
                            Clipboard.SetText(tempPIN);
                            MessageBox.Show("PIN copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        };

                        btnClose.Click += (s, ev) => pinForm.Close();

                        pinForm.Controls.Add(lblMessage);
                        pinForm.Controls.Add(txtPIN);
                        pinForm.Controls.Add(lblNote);
                        pinForm.Controls.Add(btnCopy);
                        pinForm.Controls.Add(btnClose);

                        pinForm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Failed to reset PIN.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting PIN: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbxContactNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '+')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '+' && tbxContactNumber.Text.Contains("+"))
            {
                e.Handled = true;
            }
        }

        private void tbxContactNumber_TextChanged(object sender, EventArgs e)
        {
            string contact = tbxContactNumber.Text;

            if (!string.IsNullOrEmpty(contact) && contact.StartsWith("+"))
            {
                tbxContactNumber.MaxLength = 13; // With + sign
            }
            else
            {
                tbxContactNumber.MaxLength = 11; // Without + sign
            }
        }
    }
}
