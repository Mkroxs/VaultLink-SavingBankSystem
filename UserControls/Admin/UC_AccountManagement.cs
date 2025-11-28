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

        // ✅ FIXED PARAMETER TYPE
        private void DisplayCustomerInfo(CustomerModel customer)
        {
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

        // ===== EMPTY REQUIRED HANDLERS =====

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void txbCustomerSearch_TextChanged(object sender, EventArgs e) { }
        private void guna2Panel5_Paint(object sender, PaintEventArgs e) { }
        private void guna2Panel2_Paint(object sender, PaintEventArgs e) { }
        private void txtCustomerCode_TextChanged(object sender, EventArgs e) { }
        private void txtCustomerName_TextChanged(object sender, EventArgs e) { }
        private void txtCustomerEmail_TextChanged(object sender, EventArgs e) { }
    }
}
