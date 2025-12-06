using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VaultLinkBankSystem.Helpers;
using static Syncfusion.Windows.Forms.TabBar;

// ✅ ALIAS FIX
using CustomerModel = VaultLinkBankSystem.Customer;

namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_AccountManagement : UserControl
    {
        private AccountRepository _accountRepo;
        private CustomerRepository _customerRepo;
        public UC_AccountManagement()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            _accountRepo = new AccountRepository();
            _customerRepo = new CustomerRepository();
        }

        private void UC_AccountManagement_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
            LoadVerifiedCustomers();
            SetupGridStyle(dgvCustomerAccounts);
            ClearCustomerInfo();
            txtInitialDeposit.PlaceholderText = "Enter amount";

            txtInitialDeposit.Enabled = false;
            btnCreateAccount.Enabled = false;
            btnCloseAccount.Enabled = false;
            btnReactiveAccount.Enabled = false;

        }

        

        private void ClearCustomerInfo()
        {
            txtCustomerCode.Text = "";
            txtCustomerName.Text = "";
            txtCustomerEmail.Text = "";
            txtCustomerPhoneNumber.Text = "";
            txtCustomerCode.Tag = null;

            dgvCustomerAccounts.DataSource = null;
            lblAccountCount.Text = "Total Accounts: 0";

            txtInitialDeposit.Enabled = false;
            btnCreateAccount.Enabled = false;
            btnCloseAccount.Enabled = false;
            btnReactiveAccount.Enabled = false;
        }


        private void LoadVerifiedCustomers()
        {
            try
            {
                var allCustomers = _customerRepo.GetAllCustomers();
                var verifiedCustomers = allCustomers.Where(c => c.IsKYCVerified).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void SetupGridStyle(DataGridView dgv)
        {
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.BackgroundColor = Color.White;
            dgv.GridColor = Color.FromArgb(230, 230, 230);
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 62, 84);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 144, 255);
            dgv.DefaultCellStyle.SelectionForeColor =  Color.White;
            dgv.RowHeadersVisible = false;
            dgv.RowTemplate.Height = 28;
            dgv.RowTemplate.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string search = txbCustomerSearch.Text.Trim();

            if (string.IsNullOrEmpty(search))
            {
                MessageBox.Show("Enter a customer name or code");
                return;
            }

            var results = _customerRepo.GetAllCustomers()
                .Where(c => c.CustomerCode.ToLower().Contains(search.ToLower()) ||
                            c.FullName.ToLower().Contains(search.ToLower()))
                .ToList();

            if (results.Count == 0)
            {
                MessageBox.Show("No customer found");
                ClearCustomerInfo();
                return;
            }


            if(results.Count > 1)
            {
                ShowCustomerSelectionDialog(results);
                return;
            }



            DisplayCustomerInfo(results[0]);
        }

        private void ShowCustomerSelectionDialog(System.Collections.Generic.List<VaultLinkBankSystem.Customer> customers)
        {
            Form selectionForm = new Form
            {
                Text = "Select Customer",
                Size = new Size(600, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            DataGridView dgv = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(560, 300),
                DataSource = customers,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill


                //Styling

            };


            SetupGridStyle(dgv);

            // FIX: apply column filtering after binding completes
            dgv.DataBindingComplete += (s, ev) =>
            {
                string[] allowedColumns = { "CustomerID", "FullName", "Email", "Phone" };

                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.Visible = allowedColumns.Contains(column.Name);
                }
            };




            Button btnSelect = new Button
            {
                Text = "Select Customer",
                Location = new Point(390, 320),
                Size = new Size(120, 30),
                DialogResult = DialogResult.OK
            };

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(520, 320),
                Size = new Size(60, 30),
                DialogResult = DialogResult.Cancel
            };

            selectionForm.Controls.Add(dgv);
            selectionForm.Controls.Add(btnSelect);
            selectionForm.Controls.Add(btnCancel);

            if (selectionForm.ShowDialog() == DialogResult.OK && dgv.SelectedRows.Count > 0)
            {
                VaultLinkBankSystem.Customer selectedCustomer = dgv.SelectedRows[0].DataBoundItem as VaultLinkBankSystem.Customer;
                if (selectedCustomer != null)
                {
                    DisplayCustomerInfo(selectedCustomer);
                }
            }
        }


        private void DisplayCustomerInfo(VaultLinkBankSystem.Customer customer)
        {
            txtCustomerCode.Text = customer.CustomerCode;
            txtCustomerName.Text = customer.FullName;
            txtCustomerEmail.Text = customer.Email ?? "N/A";
            txtCustomerPhoneNumber.Text = customer.Phone ?? "N/A";

            txtCustomerCode.Tag = customer.CustomerID;

            // ENABLE controls when customer is selected
            txtInitialDeposit.Enabled = true;
            btnCreateAccount.Enabled = true;

            btnCloseAccount.Enabled = false;      // Enabled only after clicking an account row
            btnReactiveAccount.Enabled = false;   // Same thing

            LoadCustomerAccounts(customer.CustomerID);
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if (txtCustomerCode.Tag == null)
            {
                MessageBox.Show("Select customer first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtInitialDeposit.Text))
            {
                MessageBox.Show("Amount is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtInitialDeposit.Text, out decimal amount))
            {
                MessageBox.Show("Invalid deposit.");
                return;
            }

            // Minimum initial deposit rule
            if (amount < 500)
            {
                MessageBox.Show("Minimum initial deposit is ₱500.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int customerId = (int)txtCustomerCode.Tag;

            _accountRepo.CreateAccount(customerId, amount, "Savings");

            LoadCustomerAccounts(customerId);
            txtInitialDeposit.Text = "";
        }


        private void LoadCustomerAccounts(int customerId)
        {
            var accounts = _accountRepo.GetAccountsByCustomerId(customerId);
            dgvCustomerAccounts.DataSource = accounts;

            if (dgvCustomerAccounts.Columns.Count > 0)
            {
                dgvCustomerAccounts.Columns["AccountID"].Visible = false;
                dgvCustomerAccounts.Columns["CustomerID"].Visible = false;

                dgvCustomerAccounts.Columns["Status"].Visible = true;
                if (dgvCustomerAccounts.Columns.Contains("ClosedDate"))
                    dgvCustomerAccounts.Columns["ClosedDate"].Visible = true;
            }

            lblAccountCount.Text = $"Total Accounts: {accounts.Count}";
        }

        

        private void txtCustomerEmail_TextChanged(object sender, EventArgs e)
        {

        }
        private void btnCloseAccount_Click(object sender, EventArgs e)
        {
            if (dgvCustomerAccounts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an account first.");
                return;
            }

            int accountId = Convert.ToInt32(dgvCustomerAccounts.SelectedRows[0].Cells["AccountID"].Value);
            string status = dgvCustomerAccounts.SelectedRows[0].Cells["Status"].Value.ToString();

            if (status == "Closed")
            {
                MessageBox.Show("This account is already closed.");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to CLOSE this account?",
                "Confirm Close Account",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                bool success = _accountRepo.CloseAccount(accountId);

                if (success)
                {
                    MessageBox.Show("Account has been successfully CLOSED.");

                    // Reload accounts
                    int customerId = (int)txtCustomerCode.Tag;
                    LoadCustomerAccounts(customerId);
                }
                else
                {
                    MessageBox.Show("Failed to close account.");
                }
            }
        }

        private void btnReactiveAccount_Click(object sender, EventArgs e)
        {
            if (dgvCustomerAccounts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an account first.");
                return;
            }

            int accountId = Convert.ToInt32(dgvCustomerAccounts.SelectedRows[0].Cells["AccountID"].Value);
            string status = dgvCustomerAccounts.SelectedRows[0].Cells["Status"].Value.ToString();

            if (status == "Active")
            {
                MessageBox.Show("This account is already active.");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Do you want to REACTIVATE this account?",
                "Confirm Reactivation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                bool success = _accountRepo.ReactivateAccount(accountId);

                if (success)
                {
                    MessageBox.Show("Account has been successfully REACTIVATED.");

                    // Reload accounts
                    int customerId = (int)txtCustomerCode.Tag;
                    LoadCustomerAccounts(customerId);
                }
                else
                {
                    MessageBox.Show("Failed to reactivate account.");
                }
            }
        }

        private void dgvCustomerAccounts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvCustomerAccounts_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txbCustomerSearch_Click(object sender, EventArgs e)
        {
            txbCustomerSearch.Clear();
        }

        private void txbCustomerSearch_Leave(object sender, EventArgs e)
        {

        }

        private void txbCustomerSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private bool _formatting = false;

        private void txtInitialDeposit_TextChanged(object sender, EventArgs e)
        {
            if (_formatting) return;

            string text = txtInitialDeposit.Text;

            // Allow empty
            if (string.IsNullOrWhiteSpace(text))
            {
                txtInitialDeposit.BorderColor = Color.Red;
                return;
            }

            // Save cursor position
            int cursor = txtInitialDeposit.SelectionStart;

            // Split integer and decimal parts
            string[] parts = text.Split('.');

            string intPart = parts[0].Replace(",", "");

            // Validate integer part only
            if (!decimal.TryParse(intPart == "" ? "0" : intPart, out decimal intValue))
            {
                txtInitialDeposit.BorderColor = Color.Red;
                return;
            }

            _formatting = true;

            // Reformat integer part with commas
            string formattedInt = string.Format("{0:N0}", intValue);

            // Rebuild final text
            string finalText = formattedInt;

            // Add decimal part back
            if (parts.Length > 1)
                finalText += "." + parts[1];

            txtInitialDeposit.Text = finalText;

            // Fix cursor position intelligently
            txtInitialDeposit.SelectionStart = Math.Min(cursor + (finalText.Length - text.Length), finalText.Length);

            _formatting = false;

            // Validate min deposit (500)
            if (decimal.TryParse(finalText, out decimal amount))
            {
                txtInitialDeposit.BorderColor = amount < 500 ? Color.Red : Color.FromArgb(76, 175, 80);
            }
        }





        private void txtInitialDeposit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            // Allow digits
            if (char.IsDigit(e.KeyChar))
                return;

            // Allow one decimal point
            if (e.KeyChar == '.' && !txtInitialDeposit.Text.Contains("."))
                return;

            // Otherwise block
            e.Handled = true;
        }


        private void txtInitialDeposit_Leave(object sender, EventArgs e)
        {
            string raw = txtInitialDeposit.Text.Replace(",", "");

            if (decimal.TryParse(raw, out decimal num))
            {
                txtInitialDeposit.Text = num.ToString("N2");
            }
        }

        private void dgvCustomerAccounts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string status = dgvCustomerAccounts.Rows[e.RowIndex].Cells["Status"].Value.ToString();

            btnCloseAccount.Enabled = status == "Active";
            btnReactiveAccount.Enabled = status == "Closed";
        }

    }
}
