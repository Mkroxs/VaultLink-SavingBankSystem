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
            SetupGridStyle();
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
            dgv.GridColor =Color.FromArgb(230, 230, 230);
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 62, 84);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 144, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
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

            DisplayCustomerInfo(results[0]);
        }

        private void ShowCustomerSelectionDialog(System.Collections.Generic.List<Customers> customers)
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
            };

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
                Customers selectedCustomer = dgv.SelectedRows[0].DataBoundItem as Customers;
                if (selectedCustomer != null)
                {
                    DisplayCustomerInfo(selectedCustomer);
                }
            }
        }


        private void DisplayCustomerInfo(Customers customer)
        {
            // Display customer details in your textboxes/labels
            txtCustomerCode.Text = customer.CustomerCode;
            txtCustomerName.Text = customer.FullName;
            txtCustomerEmail.Text = customer.Email ?? "N/A";
            txtCustomerPhoneNumber.Text = customer.Phone ?? "N/A";

            txtCustomerCode.Tag = customer.CustomerID;
            LoadCustomerAccounts(customer.CustomerID);
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
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if (txtCustomerCode.Tag == null)
            {
                MessageBox.Show("Select customer first.");
                return;
            }

            if (!decimal.TryParse(txtInitialDeposit.Text, out decimal amount))
            {
                MessageBox.Show("Invalid deposit");
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
            }

            lblAccountCount.Text = $"Total Accounts: {accounts.Count}";
        }

        private void SetupGridStyle()
        {
            dgvCustomerAccounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomerAccounts.MultiSelect = false;
            dgvCustomerAccounts.RowHeadersVisible = false;
        }

        private void txtCustomerEmail_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
