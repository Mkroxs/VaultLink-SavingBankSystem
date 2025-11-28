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
    public partial class UC_Reports : UserControl
    {
        private TransactionRepository _transactionRepo;
        private AccountRepository _accountRepo;
        private CustomerRepository _customerRepo;
        private List<TransactionReportData> _allTransactions;
        public UC_Reports()
        {
            InitializeComponent();
            LoadReportGrid();

            _transactionRepo = new TransactionRepository();
            _accountRepo = new AccountRepository();
            _customerRepo = new CustomerRepository();

            InitializeControls();
            LoadAllTransactions();
        }
        private void UC_Reports_Load(object sender, EventArgs e)
        {
            LoadReportGrid();
        }



        private void InitializeControls()
        {
            // Setup Transaction Type ComboBox
            cmbTransactionType.Items.Clear();
            cmbTransactionType.Items.Add("All Transactions");
            cmbTransactionType.Items.Add("Deposit");
            cmbTransactionType.Items.Add("Withdrawal");
            cmbTransactionType.Items.Add("Transfer Out");
            cmbTransactionType.Items.Add("Transfer In");
            cmbTransactionType.SelectedIndex = 0;

            // Set default date range (last 30 days)
            dtpDateFrom.Value = DateTime.Now.AddDays(-30);
            dtpDateTo.Value = DateTime.Now;

            // Setup DataGridView
            SetupDataGridView();
        }


        private void SetupDataGridView()
        {
            guna2DataGridView1.Columns.Clear();
            guna2DataGridView1.Rows.Clear();

            guna2DataGridView1.AutoGenerateColumns = false;
            guna2DataGridView1.RowHeadersVisible = false;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.AllowUserToResizeRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.MultiSelect = false;

            // Header styling
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 62, 84);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            guna2DataGridView1.ColumnHeadersHeight = 40;
            guna2DataGridView1.EnableHeadersVisualStyles = false;

            // Cell styling
            guna2DataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            guna2DataGridView1.DefaultCellStyle.BackColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 144, 255);
            guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.Padding = new Padding(5, 3, 5, 3);

            guna2DataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249);
            guna2DataGridView1.RowTemplate.Height = 35;

            guna2DataGridView1.GridColor = Color.FromArgb(230, 230, 230);
            guna2DataGridView1.BorderStyle = BorderStyle.None;
            guna2DataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // Add columns
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TransactionID",
                HeaderText = "Transaction ID",
                Width = 120
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AccountNumber",
                HeaderText = "Account Number",
                Width = 150
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerCode",
                HeaderText = "Customer",
                Width = 120
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TransactionType",
                HeaderText = "Transaction Type",
                Width = 130
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "Amount",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "C2"
                }
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "Date",
                Width = 150
            });

            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Remarks",
                HeaderText = "Remarks",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
        }



        private void LoadAllTransactions()
        {
            try
            {
                _allTransactions = new List<TransactionReportData>();

                // Get all accounts
                var allAccounts = _accountRepo.GetAllAccounts();

                foreach (var account in allAccounts)
                {
                    // Get transactions for each account
                    var transactions = _transactionRepo.GetTransactionsByAccountId(account.AccountID);

                    // Get customer info
                    var customer = _customerRepo.GetCustomerById(account.CustomerID);

                    foreach (var transaction in transactions)
                    {
                        _allTransactions.Add(new TransactionReportData
                        {
                            TransactionID = transaction.TransactionID,
                            AccountNumber = account.AccountNumber,
                            CustomerCode = customer?.CustomerCode ?? "N/A",
                            CustomerName = customer?.FullName ?? "N/A",
                            TransactionType = transaction.TransactionType,
                            Amount = transaction.Amount,
                            TransactionDate = transaction.TransactionDate,
                            Remarks = transaction.Remarks
                        });
                    }
                }

                // Sort by date descending
                _allTransactions = _allTransactions.OrderByDescending(t => t.TransactionDate).ToList();

                // Apply initial filter
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading transactions: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }




        private void btnFilter_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            try
            {
                if (_allTransactions == null || _allTransactions.Count == 0)
                {
                    guna2DataGridView1.Rows.Clear();
                    UpdateSummary(new List<TransactionReportData>());
                    return;
                }

                // Start with all transactions
                var filtered = _allTransactions.AsEnumerable();

                // Filter by transaction type
                string selectedType = cmbTransactionType.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(selectedType) && selectedType != "All Transactions")
                {
                    filtered = filtered.Where(t => t.TransactionType == selectedType);
                }

                // Filter by date range
                DateTime dateFrom = dtpDateFrom.Value.Date;
                DateTime dateTo = dtpDateTo.Value.Date.AddDays(1).AddSeconds(-1); // End of day

                filtered = filtered.Where(t => t.TransactionDate >= dateFrom && t.TransactionDate <= dateTo);

                var filteredList = filtered.ToList();

                // Display results
                DisplayTransactions(filteredList);

                // Update summary
                UpdateSummary(filteredList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DisplayTransactions(List<TransactionReportData> transactions)
        {
            guna2DataGridView1.Rows.Clear();

            foreach (var transaction in transactions)
            {
                guna2DataGridView1.Rows.Add(
                    transaction.TransactionID.ToString(),
                    transaction.AccountNumber,
                    transaction.CustomerCode,
                    transaction.TransactionType,
                    transaction.Amount,
                    transaction.TransactionDate.ToString("MMM dd, yyyy hh:mm tt"),
                    transaction.Remarks
                );
            }

            // Show message if no results   
            if (transactions.Count == 0)
            {
                MessageBox.Show("No transactions found matching the selected filters.",
                    "No Results",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void UpdateSummary(List<TransactionReportData> transactions)
        {
            decimal totalDeposits = transactions
                .Where(t => t.TransactionType == "Deposit")
                .Sum(t => t.Amount);

            decimal totalWithdrawals = transactions
                .Where(t => t.TransactionType == "Withdrawal")
                .Sum(t => t.Amount);

            decimal totalTransfers = transactions
                .Where(t => t.TransactionType == "Transfer Out")
                .Sum(t => t.Amount);

            // Update labels (adjust control names to match your UI)
            lblTotalDeposits.Text = totalDeposits.ToString("C2");
            lblTotalWithdrawals.Text = totalWithdrawals.ToString("C2");
            lblTotalTransfers.Text = totalTransfers.ToString("C2");
        }





        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAllTransactions();
            MessageBox.Show("Data refreshed successfully!",
                "Refresh",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

        }



















        public void LoadReportGrid()
        {

            /*guna2DataGridView1.Columns.Clear();
            guna2DataGridView1.Rows.Clear();

          
            guna2DataGridView1.AutoGenerateColumns = false;
            guna2DataGridView1.RowHeadersVisible = false;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.AllowUserToResizeRows = false;
            guna2DataGridView1.ReadOnly = true;
            
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 40, 70);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            guna2DataGridView1.EnableHeadersVisualStyles = false;

            guna2DataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            guna2DataGridView1.DefaultCellStyle.BackColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            guna2DataGridView1.GridColor = Color.LightGray;
            guna2DataGridView1.BorderStyle = BorderStyle.None;

            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            guna2DataGridView1.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.White;
            guna2DataGridView1.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.Black;
            guna2DataGridView1.RowHeadersDefaultCellStyle.SelectionBackColor = Color.White;
            guna2DataGridView1.RowHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

            guna2DataGridView1.EnableHeadersVisualStyles = false;


            guna2DataGridView1.Columns.Add("TransactionID", "Transaction ID");
            guna2DataGridView1.Columns.Add("AccountNumber", "Account Number");
            guna2DataGridView1.Columns.Add("CustomerCode", "Customer");
            guna2DataGridView1.Columns.Add("TransactionType", "Transaction Type");
            guna2DataGridView1.Columns.Add("Amount", "Amount");
            guna2DataGridView1.Columns.Add("Date", "Date");
            guna2DataGridView1.Columns.Add("Status", "Status");
            guna2DataGridView1.Columns.Add("Remarks", "Remarks");

            foreach (DataGridViewColumn col in guna2DataGridView1.Columns)
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            guna2DataGridView1.Rows.Add("TX1001", "ACC2025-0001", "0001", "Withdraw", "₱2,000", "01/01/2025", "Completed", "Teller");
            guna2DataGridView1.Rows.Add("TX1002", "ACC2025-0001", "0001", "Deposit", "₱5,000", "01/02/2025", "Completed", "Teller");
            guna2DataGridView1.Rows.Add("TX1003", "ACC2025-0002", "0002", "Transfer", "₱1,500", "01/04/2025", "Pending", "Transfer to 0003");
        }*/
        }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void UC_Reports_Load_1(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        
    }

}
