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

namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_AccountManagement : UserControl
    {
        private AccountRepository _accountRepo;
        private CustomerRepository _customerRepo;
        private TransactionRepository _transactionRepo;
        public UC_AccountManagement()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            _accountRepo = new AccountRepository();
            _customerRepo = new CustomerRepository();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

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
                // Get all customers with verified KYC
                var allCustomers = _customerRepo.GetAllCustomers();
                var verifiedCustomers = allCustomers.Where(c => c.IsKYCVerified).ToList();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate customer is selected
                if (txtCustomerCode.Tag == null)
                {
                    MessageBox.Show("Please search and select a customer first.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int customerId = (int)txtCustomerCode.Tag;

                // Validate initial deposit
                if (string.IsNullOrWhiteSpace(txtInitialDeposit.Text))
                {
                    MessageBox.Show("Please enter initial deposit amount.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtInitialDeposit.Focus();
                    return;
                }

                if (!decimal.TryParse(txtInitialDeposit.Text, out decimal initialDeposit))
                {
                    MessageBox.Show("Please enter a valid deposit amount.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtInitialDeposit.Focus();
                    return;
                }

                if (initialDeposit < 0)
                {
                    MessageBox.Show("Initial deposit cannot be negative.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtInitialDeposit.Focus();
                    return;
                }

                // Optional: Set minimum deposit requirement
                if (initialDeposit < 100)
                {
                    DialogResult minResult = MessageBox.Show(
                        "Initial deposit is less than the recommended minimum of ₱100.00.\n\n" +
                        "Do you want to continue?",
                        "Low Initial Deposit",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (minResult == DialogResult.No)
                    {
                        return;
                    }
                }

                // Get account type
                string accountType = "Savings"; // Default
                

                // Confirmation dialog
                Customers customer = _customerRepo.GetCustomerById(customerId);
                DialogResult result = MessageBox.Show(
                    $"Create new savings account for:\n\n" +
                    $"Customer: {customer.FullName}\n" +
                    $"Customer Code: {customer.CustomerCode}\n" +
                    $"Initial Deposit: {initialDeposit:C2}\n" +
                    $"Account Type: {accountType}\n\n" +
                    $"Proceed with account creation?",
                    "Confirm Account Creation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // Create the account
                    int accountId = _accountRepo.CreateAccount(customerId, initialDeposit, accountType);

                    // Get the new account details
                    var newAccount = _accountRepo.GetAccountsByCustomerId(customerId)
                        .FirstOrDefault(a => a.AccountID == accountId);

                    MessageBox.Show(
                        $"✅ ACCOUNT CREATED SUCCESSFULLY!\n\n" +
                        $"Account Number: {newAccount.AccountNumber}\n" +
                        $"Account Type: {newAccount.AccountType}\n" +
                        $"Initial Balance: {newAccount.Balance:C2}\n" +
                        $"Status: {newAccount.Status}\n" +
                        $"Date Opened: {newAccount.DateOpened:MMMM dd, yyyy}\n\n" +
                        $"Customer can now use this account for transactions.",
                        "Account Created",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Clear initial deposit field
                    txtInitialDeposit.Clear();

                    // Reload customer accounts to show the new one
                    LoadCustomerAccounts(customerId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating account: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            try
            {
                string searchTerm = txbCustomerSearch.Text.Trim();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    MessageBox.Show("Please enter a customer code or name to search.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Search for verified customers only
                var allCustomers = _customerRepo.GetAllCustomers();
                var foundCustomers = allCustomers
                    .Where(c => c.IsKYCVerified &&
                                (c.CustomerCode.ToLower().Contains(searchTerm.ToLower()) ||
                                 c.FullName.ToLower().Contains(searchTerm.ToLower())))
                    .ToList();

                if (foundCustomers.Count == 0)
                {
                    MessageBox.Show($"No verified customer found matching '{searchTerm}'.", "Not Found",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearCustomerInfo();
                    return;
                }

                if (foundCustomers.Count == 1)
                {
                    // Only one match - display customer info
                    DisplayCustomerInfo(foundCustomers[0]);
                }
                else
                {
                    // Multiple matches - show selection dialog
                    ShowCustomerSelectionDialog(foundCustomers);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching customer: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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
            SetupGridStyle(dgv);





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

            // Store customer ID for account creation
            txtCustomerCode.Tag = customer.CustomerID;

            // Load existing accounts for this customer
            LoadCustomerAccounts(customer.CustomerID);

            // Enable create account button
            btnCreateAccount.Enabled = true;
        }
        private void ClearCustomerInfo()
        {
            txtCustomerCode.Text = "";
            txtCustomerName.Text = "";
            txtCustomerEmail.Text = "";
            txtCustomerPhoneNumber.Text = "";
            txtCustomerCode.Tag = null;
            btnCreateAccount.Enabled = false;

            if (dgvCustomerAccounts != null)
            {
                dgvCustomerAccounts.DataSource = null;
            }
        }

        private void LoadCustomerAccounts(int customerId)
        {
            try
            {
                var accounts = _accountRepo.GetAccountsByCustomerId(customerId);

                if (dgvCustomerAccounts != null)
                {
                    dgvCustomerAccounts.DataSource = accounts;

                    // Configure columns
                    if (dgvCustomerAccounts.Columns.Count > 0)
                    {
                        dgvCustomerAccounts.Columns["AccountID"].Visible = false;
                        dgvCustomerAccounts.Columns["CustomerID"].Visible = false;
                        dgvCustomerAccounts.Columns["InterestRateID"].Visible = false;
                        dgvCustomerAccounts.Columns["ClosedDate"].Visible = false;

                        dgvCustomerAccounts.Columns["AccountNumber"].HeaderText = "Account Number";
                        dgvCustomerAccounts.Columns["AccountType"].HeaderText = "Type";
                        dgvCustomerAccounts.Columns["Balance"].HeaderText = "Balance";
                        dgvCustomerAccounts.Columns["Balance"].DefaultCellStyle.Format = "C2";
                        dgvCustomerAccounts.Columns["Status"].HeaderText = "Status";
                        dgvCustomerAccounts.Columns["DateOpened"].HeaderText = "Date Opened";
                        dgvCustomerAccounts.Columns["DateOpened"].DefaultCellStyle.Format = "MM/dd/yyyy";
                    }
                }

                // Update account count label
                if (lblAccountCount != null)
                {
                    lblAccountCount.Text = $"Total Accounts: {accounts.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txbCustomerSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        private void SetupGridStyle()
        {
            var dgv = dgvCustomerAccounts;

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
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            dgv.RowHeadersVisible = false;

            dgv.RowTemplate.Height = 28;
            dgv.RowTemplate.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);

            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        }
        private void txtCustomerCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCustomerName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCustomerEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnCloseAccount_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if an account is selected
                if (dgvCustomerAccounts.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select an account to close.",
                        "No Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Get selected account
                var selectedAccount = dgvCustomerAccounts.SelectedRows[0].DataBoundItem as Account;

                if (selectedAccount == null)
                {
                    MessageBox.Show("Error getting selected account.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Check if account is already closed
                if (selectedAccount.Status.ToLower() == "closed")
                {
                    MessageBox.Show("This account is already closed.",
                        "Already Closed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Check if account has balance
                if (selectedAccount.Balance > 0)
                {
                    DialogResult balanceResult = MessageBox.Show(
                        $"⚠️ WARNING: This account has a remaining balance of {selectedAccount.Balance:C2}\n\n" +
                        $"Closing the account with remaining balance requires:\n" +
                        $"1. Customer withdrawal or transfer of funds\n" +
                        $"2. Manager approval\n\n" +
                        $"Are you sure you want to close this account?",
                        "Account Has Balance",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (balanceResult == DialogResult.No)
                    {
                        return;
                    }
                }

                // Get customer info
                var customer = _customerRepo.GetCustomerById(selectedAccount.CustomerID);

                // Confirmation dialog
                DialogResult result = MessageBox.Show(
                    $"CLOSE ACCOUNT\n\n" +
                    $"Customer: {customer?.FullName}\n" +
                    $"Account Number: {selectedAccount.AccountNumber}\n" +
                    $"Account Type: {selectedAccount.AccountType}\n" +
                    $"Current Balance: {selectedAccount.Balance:C2}\n" +
                    $"Date Opened: {selectedAccount.DateOpened:MMMM dd, yyyy}\n\n" +
                    $"⚠️ This action will:\n" +
                    $"• Set account status to 'Closed'\n" +
                    $"• Prevent future transactions\n" +
                    $"• Require reactivation to use again\n\n" +
                    $"Do you want to proceed?",
                    "Confirm Account Closure",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Close the account
                    bool success = _accountRepo.CloseAccount(selectedAccount.AccountID);

                    if (success)
                    {
                        MessageBox.Show(
                            $"✅ ACCOUNT CLOSED SUCCESSFULLY\n\n" +
                            $"Account Number: {selectedAccount.AccountNumber}\n" +
                            $"Closed Date: {DateTime.Now:MMMM dd, yyyy HH:mm}\n" +
                            $"Final Balance: {selectedAccount.Balance:C2}\n\n" +
                            $"The account has been closed and can no longer be used for transactions.\n" +
                            $"The account can be reactivated if needed.",
                            "Account Closed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Reload accounts to show updated status
                        LoadCustomerAccounts(selectedAccount.CustomerID);
                    }
                    else
                    {
                        MessageBox.Show("Failed to close account. Please try again.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error closing account: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnReactiveAccount_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if an account is selected
                if (dgvCustomerAccounts.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select an account to reactivate.",
                        "No Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Get selected account
                var selectedAccount = dgvCustomerAccounts.SelectedRows[0].DataBoundItem as Account;

                if (selectedAccount == null)
                {
                    MessageBox.Show("Error getting selected account.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Check if account is already active
                if (selectedAccount.Status.ToLower() == "active")
                {
                    MessageBox.Show("This account is already active.",
                        "Already Active",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Get customer info
                var customer = _customerRepo.GetCustomerById(selectedAccount.CustomerID);

                // Confirmation dialog
                DialogResult result = MessageBox.Show(
                    $"REACTIVATE ACCOUNT\n\n" +
                    $"Customer: {customer?.FullName}\n" +
                    $"Account Number: {selectedAccount.AccountNumber}\n" +
                    $"Account Type: {selectedAccount.AccountType}\n" +
                    $"Current Balance: {selectedAccount.Balance:C2}\n" +
                    $"Current Status: {selectedAccount.Status}\n\n" +
                    $"✅ This action will:\n" +
                    $"• Set account status to 'Active'\n" +
                    $"• Allow transactions to resume\n" +
                    $"• Restore full account functionality\n\n" +
                    $"Do you want to proceed?",
                    "Confirm Account Reactivation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Reactivate the account
                    bool success = _accountRepo.ReactivateAccount(selectedAccount.AccountID);

                    if (success)
                    {
                        MessageBox.Show(
                            $"✅ ACCOUNT REACTIVATED SUCCESSFULLY\n\n" +
                            $"Account Number: {selectedAccount.AccountNumber}\n" +
                            $"Reactivated Date: {DateTime.Now:MMMM dd, yyyy HH:mm}\n" +
                            $"Current Balance: {selectedAccount.Balance:C2}\n\n" +
                            $"The account is now active and can be used for transactions.",
                            "Account Reactivated",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Reload accounts to show updated status
                        LoadCustomerAccounts(selectedAccount.CustomerID);
                    }
                    else
                    {
                        MessageBox.Show("Failed to reactivate account. Please try again.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reactivating account: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvCustomerAccounts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvCustomerAccounts_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = dgvCustomerAccounts.HitTest(e.X, e.Y);

                if (hitTest.RowIndex >= 0)
                {
                    dgvCustomerAccounts.ClearSelection();
                    dgvCustomerAccounts.Rows[hitTest.RowIndex].Selected = true;

                    var selectedAccount = dgvCustomerAccounts.Rows[hitTest.RowIndex].DataBoundItem as Account;

                    if (selectedAccount != null)
                    {
                        ContextMenuStrip menu = new ContextMenuStrip();

                        if (selectedAccount.Status.ToLower() == "active")
                        {
                            menu.Items.Add("Close Account", null, (s, ev) => btnCloseAccount_Click(s, ev));
                        }
                        else if (selectedAccount.Status.ToLower() == "closed")
                        {
                            menu.Items.Add("Reactivate Account", null, (s, ev) => btnReactiveAccount_Click(s, ev));
                        }

                        menu.Items.Add("View Transactions", null, (s, ev) => ViewAccountTransactions(selectedAccount));

                        menu.Show(dgvCustomerAccounts, e.Location);
                    }
                }
            }
        }

        private void ViewAccountTransactions(Account account)
        {
            try
            {
                var transactions = _transactionRepo.GetTransactionsByAccountId(account.AccountID);

                // Create a simple form to display transactions
                Form transactionForm = new Form
                {
                    Text = $"Transactions - {account.AccountNumber}",
                    Size = new Size(800, 600),
                    StartPosition = FormStartPosition.CenterParent
                };

                DataGridView dgv = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    DataSource = transactions,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect
                };

                transactionForm.Controls.Add(dgv);
                transactionForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transactions: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void txbCustomerSearch_Click(object sender, EventArgs e)
        {
            txbCustomerSearch.Text = "";
        }

        private void txbCustomerSearch_Leave(object sender, EventArgs e)
        {
            txbCustomerSearch.Text = "Search/Select Customer";
        }
    }
}
