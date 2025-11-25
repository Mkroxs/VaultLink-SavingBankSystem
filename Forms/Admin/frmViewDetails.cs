using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VaultLinkBankSystem;
using FontAwesome.Sharp;

namespace VaultLinkBankSystem.Forms.Admin
{
    public partial class frmViewDetails : Form
    {
        private CustomerRepository _customerRepo;
        private bool _isEditing;

        public frmViewDetails()
        {
            InitializeComponent();
            _customerRepo = new CustomerRepository();
            _isEditing = false;
        }

        private void guna2Panel8_Paint(object sender, PaintEventArgs e)
        {
        }

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
                    try
                    {
                        iconEdit.IconChar = IconChar.Save;
                        iconEdit.IconColor = Color.FromArgb(20, 140, 20);
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
                    try
                    {
                        iconEdit.IconChar = IconChar.Edit;
                        iconEdit.IconColor = Color.DimGray;
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
    }
}