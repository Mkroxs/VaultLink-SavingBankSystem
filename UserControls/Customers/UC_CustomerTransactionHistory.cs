using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VaultLinkBankSystem.Forms.Customer;

namespace VaultLinkBankSystem.UserControls.Customers
{
    public partial class UC_CustomerTransactionHistory : UserControl
    {
        private VaultLinkBankSystem.Customer _currentCustomer;
        private AccountRepository _accountRepo;
        private CustomerRepository _customerRepo;
        private readonly TransactionRepository _transactionRepo;
        private List<TransactionWithAccount> _allTransactions = new List<TransactionWithAccount>();
        private List<Account> _customerAccounts = new List<Account>();

        public UC_CustomerTransactionHistory(VaultLinkBankSystem.Customer currentCustomer)
        {
            InitializeComponent();

            _currentCustomer = currentCustomer;
            _accountRepo = new AccountRepository();
            _customerRepo = new CustomerRepository();
            _transactionRepo = new TransactionRepository();

            LoadCustomerAccounts();
            LoadAccountsToComboBox();
            LoadTransactionTypes();
            LoadAllTransactions();
            SetupDataGridView();
            ApplyFilters();

            if (btnProfile != null)
            {
                btnProfile.Click += btnProfile_Click;
            }
        }

        // ============================================
        // LOAD CUSTOMER ACCOUNTS
        // ============================================
        private void LoadCustomerAccounts()
        {
            _customerAccounts = _accountRepo.GetAccountsByCustomerId(_currentCustomer.CustomerID);
        }

        // ============================================
        // LOAD ACCOUNTS TO COMBOBOX
        // ============================================
        private void LoadAccountsToComboBox()
        {
            cbxSelectAccount.Items.Clear();
            cbxSelectAccount.Items.Add("All Accounts");

            foreach (var account in _customerAccounts)
            {
                cbxSelectAccount.Items.Add($"{account.AccountNumber} - {account.AccountType}");
            }

            cbxSelectAccount.SelectedIndex = 0;
        }

        // ============================================
        // LOAD TRANSACTION TYPES
        // ============================================
        private void LoadTransactionTypes()
        {
            cbxTransactionType.Items.Clear();
            cbxTransactionType.Items.Add("All Types");
            cbxTransactionType.Items.Add("Deposit");
            cbxTransactionType.Items.Add("Withdrawal");
            cbxTransactionType.Items.Add("Transfer In");
            cbxTransactionType.Items.Add("Transfer Out");
            cbxTransactionType.Items.Add("Interest Added");

            cbxTransactionType.SelectedIndex = 0;
        }

        // ============================================
        // LOAD ALL TRANSACTIONS
        // ============================================
        private void LoadAllTransactions()
        {
            _allTransactions.Clear();

            foreach (var account in _customerAccounts)
            {
                var transactions = _transactionRepo.GetTransactionsByAccountId(account.AccountID);

                foreach (var transaction in transactions)
                {
                    _allTransactions.Add(new TransactionWithAccount
                    {
                        Transaction = transaction,
                        AccountNumber = account.AccountNumber,
                        AccountType = account.AccountType
                    });
                }
            }
        }

        // ============================================
        // SETUP DATAGRIDVIEW
        // ============================================
        private void SetupDataGridView()
        {
            dgvHistoryTransactions.Rows.Clear();

            // Keep only the styling code:
            dgvHistoryTransactions.BackgroundColor = Color.FromArgb(20, 40, 70);
            dgvHistoryTransactions.ForeColor = Color.White;
            dgvHistoryTransactions.DefaultCellStyle.BackColor = Color.FromArgb(20, 40, 70);
            dgvHistoryTransactions.DefaultCellStyle.ForeColor = Color.White;
            dgvHistoryTransactions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(25, 45, 75);
            dgvHistoryTransactions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 60, 90);
            dgvHistoryTransactions.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvHistoryTransactions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 30, 60);
            dgvHistoryTransactions.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHistoryTransactions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvHistoryTransactions.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Center headers
            dgvHistoryTransactions.EnableHeadersVisualStyles = false;

            dgvHistoryTransactions.RowTemplate.Height = 40; // Increased height
            dgvHistoryTransactions.DefaultCellStyle.Font = new Font("Segoe UI", 10); // Larger font
            dgvHistoryTransactions.DefaultCellStyle.Padding = new Padding(8, 5, 8, 5); // More padding

            dgvHistoryTransactions.GridColor = Color.FromArgb(40, 60, 90);
            dgvHistoryTransactions.BorderStyle = BorderStyle.None;
            dgvHistoryTransactions.RowHeadersVisible = false;
            dgvHistoryTransactions.AllowUserToAddRows = false;
            dgvHistoryTransactions.ReadOnly = true;
            dgvHistoryTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistoryTransactions.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // Set column widths for existing columns - WIDER
            dgvHistoryTransactions.Columns[0].Width = 180; 
            dgvHistoryTransactions.Columns[1].Width = 130; 
            dgvHistoryTransactions.Columns[2].Width = 130; 
            dgvHistoryTransactions.Columns[3].Width = 150; 
            dgvHistoryTransactions.Columns[4].Width = 150; 
            dgvHistoryTransactions.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; 

            dgvHistoryTransactions.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvHistoryTransactions.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 
            dgvHistoryTransactions.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;  
            dgvHistoryTransactions.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;  
            dgvHistoryTransactions.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;  
            dgvHistoryTransactions.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;   
        }


        private void ApplyFilters()
        {
            if (_allTransactions == null || _allTransactions.Count == 0)
            {
                dgvHistoryTransactions.Rows.Clear();
                return;
            }

            var filtered = _allTransactions.AsEnumerable();

            // Filter by Account
            if (cbxSelectAccount.SelectedIndex > 0)
            {
                string selectedText = cbxSelectAccount.SelectedItem.ToString();

                string[] parts = selectedText.Split(new[] { " - " }, StringSplitOptions.None);
                if (parts.Length > 0)
                {
                    string accountNumber = parts[0].Trim();
                    filtered = filtered.Where(t => t.AccountNumber == accountNumber);
                }
            }

            if (cbxTransactionType.SelectedIndex > 0)
            {
                string selectedType = cbxTransactionType.SelectedItem.ToString();
                filtered = filtered.Where(t => t.Transaction.TransactionType == selectedType);
            }

            //diri na ta i-order ang filtered list by date descending
            var filteredList = filtered
                .OrderByDescending(t => t.Transaction.TransactionDate)
                .ToList();

            DisplayFilteredTransactions(filteredList);
        }

        private void DisplayFilteredTransactions(List<TransactionWithAccount> list)
        {
            dgvHistoryTransactions.Rows.Clear();

            foreach (var item in list)
            {
                var t = item.Transaction;

                dgvHistoryTransactions.Rows.Add(
                    t.TransactionDate.ToString("MM/dd/yyyy hh:mm tt"), 
                    t.TransactionType,                                   
                    $"₱{t.Amount:N2}",                                  
                    $"₱{t.PreviousBalance:N2}",                        
                    $"₱{t.NewBalance:N2}",                             
                    $"{item.AccountNumber} - {t.Remarks ?? "N/A"}"      
                );
            }
        }

  
        private void cbxSelectAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void cbxTransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            frmCustomerDashboard form = this.FindForm() as frmCustomerDashboard;

            if (form != null)
            {
                form.ShowDashboard();
            }
        }

        private void UC_CustomerTransactionHistory_Load(object sender, EventArgs e)
        {
        }

        private void UC_CustomerTransactionHistory_Load_1(object sender, EventArgs e)
        {
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dgvHistoryTransactions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}