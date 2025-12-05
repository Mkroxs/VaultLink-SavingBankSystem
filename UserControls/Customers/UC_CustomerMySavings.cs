using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VaultLinkBankSystem.Forms.Customer;

namespace VaultLinkBankSystem.UserControls.Customer
{
    public partial class UC_CustomerMySavings : UserControl
    {
        private VaultLinkBankSystem.Customer _currentCustomer;
        private AccountRepository _accountRepo;
        private TransactionRepository _transactionRepo;
        private InterestRateRepository _interestRateRepo;
        private List<Account> _customerAccounts;

        public UC_CustomerMySavings(VaultLinkBankSystem.Customer currentCustomer)
        {
            InitializeComponent();
            _currentCustomer = currentCustomer;

            _accountRepo = new AccountRepository();
            _transactionRepo = new TransactionRepository();
            _interestRateRepo = new InterestRateRepository();

            // Setup the recent transactions DataGridView
            SetupRecentTransactionsDataGridView();
        }

        private void guna2HtmlLabel8_Click(object sender, EventArgs e)
        {
        }

        private void guna2Panel9_Paint(object sender, PaintEventArgs e)
        {
        }

        private void iconPictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            frmCustomerDashboard form = this.FindForm() as frmCustomerDashboard;

            if (form != null)
            {
                form.ShowDashboard();
            }
        }

        private void PopulateAccountDropdown()
        {
            cbxSelectAccount.Items.Clear();
            cbxSelectAccount.DisplayMember = "DisplayText";
            cbxSelectAccount.ValueMember = "AccountID";

            cbxSelectAccount.FocusedState.ForeColor = Color.White;
            cbxSelectAccount.ForeColor = Color.White;
            cbxSelectAccount.FillColor = Color.FromArgb(20, 55, 90);

            foreach (var account in _customerAccounts)
            {
                cbxSelectAccount.Items.Add(new
                {
                    AccountID = account.AccountID,
                    Account = account,
                    DisplayText = $"{account.AccountNumber} - {account.AccountType} (₱{account.Balance:N2})"
                });
            }

            if (cbxSelectAccount.Items.Count > 0)
                cbxSelectAccount.SelectedIndex = 0;
        }

        private void showBalance_Click(object sender, EventArgs e)
        {
            if (lblBalance.Tag == null)
                lblBalance.Tag = lblBalance.Text;

            string original = lblBalance.Tag.ToString();

            if (showBalance.IconChar == FontAwesome.Sharp.IconChar.EyeSlash)
            {
                showBalance.IconChar = FontAwesome.Sharp.IconChar.Eye;
                showBalance.IconSize = 45;
                lblBalance.Text = original;
            }
            else
            {
                string mask = new string('*', original.Length);
                lblBalance.Text = mask;
                showBalance.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
                showBalance.IconSize = 46;
            }
        }

        private void DisplayCustomerInfo()
        {
            _customerAccounts = _accountRepo.GetAccountsByCustomerId(_currentCustomer.CustomerID);

            if (_customerAccounts == null || _customerAccounts.Count == 0)
            {
                MessageBox.Show("This customer has no accounts.",
                    "No Accounts", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PopulateAccountDropdown();
            DisplaySelectedAccount();
        }

        private void DisplaySelectedAccount()
        {
            if (cbxSelectAccount.SelectedItem == null)
                return;

            dynamic selected = cbxSelectAccount.SelectedItem;
            Account account = selected.Account;

            // Store original values in Tag before masking
            lblAccountNumber.Tag = account.AccountNumber;
            lblBalance.Tag = $"₱{account.Balance:N2}";

            // Check if we should show masked or real values
            bool accountNumberMasked = (showAccountNumber.IconChar == FontAwesome.Sharp.IconChar.EyeSlash);
            bool balanceMasked = (showBalance.IconChar == FontAwesome.Sharp.IconChar.EyeSlash);

            // Display account number (masked or not)
            if (accountNumberMasked && account.AccountNumber.Length > 4)
            {
                string lastFour = account.AccountNumber.Substring(account.AccountNumber.Length - 4);
                string mask = new string('*', account.AccountNumber.Length - 4);
                lblAccountNumber.Text = mask + lastFour;
            }
            else
            {
                lblAccountNumber.Text = account.AccountNumber;
            }

            // Display balance (masked or not)
            string balanceText = $"₱{account.Balance:N2}";
            if (balanceMasked)
            {
                lblBalance.Text = new string('*', balanceText.Length);
            }
            else
            {
                lblBalance.Text = balanceText;
            }

            // Display account status
            if (account.Status == "Active")
            {
                lblAccountStatus.ForeColor = Color.Green;
                lblAccountStatus.Text = "Active";
                lblDateLabel.Text = "Date Created";
                lblDateValue.Text = account.DateOpened.ToString("MM/dd/yyyy");
            }
            else
            {
                lblAccountStatus.ForeColor = Color.Red;
                lblAccountStatus.Text = "Closed";
                lblDateLabel.Text = "Date Closed";
                lblDateValue.Text = account.ClosedDate?.ToString("MM/dd/yyyy") ?? "N/A";
            }

            // Calculate and display financial statistics
            CalculateAndDisplayStats(account.AccountID);

            // Display recent transactions
            DisplayRecentTransactions(account.AccountID);
        }

        private void CalculateAndDisplayStats(int accountId)
        {
            try
            {
                // Get all transactions for this account
                var transactions = _transactionRepo.GetTransactionsByAccountId(accountId);

                if (transactions == null || transactions.Count == 0)
                {
                    // No transactions yet - show zeros
                    lblNetSavings1.Text = "₱0.00";
                    lblDeposited.Text = "₱0.00";
                    lblWithdrawn.Text = "₱0.00";
                    lblInterestEarned.Text = "₱0.00";
                    return;
                }

                // Calculate total interest earned from "Interest Added" transactions
                decimal totalInterestEarned = transactions
                    .Where(t => t.TransactionType == "Interest Added")
                    .Sum(t => t.Amount);

                // Calculate total deposits (excluding interest)
                decimal totalDeposits = transactions
                    .Where(t => t.TransactionType == "Deposit")
                    .Sum(t => t.Amount);

                // Calculate total withdrawals
                decimal totalWithdrawals = transactions
                    .Where(t => t.TransactionType == "Withdrawal")
                    .Sum(t => t.Amount);

                // Calculate transfers in (money received)
                decimal transfersIn = transactions
                    .Where(t => t.TransactionType == "Transfer In")
                    .Sum(t => t.Amount);

                // Calculate transfers out (money sent)
                decimal transfersOut = transactions
                    .Where(t => t.TransactionType == "Transfer Out")
                    .Sum(t => t.Amount);

                // Net Savings = Deposits + Transfers In - Withdrawals - Transfers Out
                decimal netSavings = totalDeposits + transfersIn - totalWithdrawals - transfersOut;

                // Display the results
                lblNetSavings1.Text = $"₱{netSavings:N2}";
                lblDeposited.Text = $"₱{totalDeposits:N2}";
                lblWithdrawn.Text = $"₱{totalWithdrawals:N2}";
                lblInterestEarned.Text = $"₱{totalInterestEarned:N2}";

                // Optional: Color code net savings
                if (netSavings >= 0)
                    lblNetSavings1.ForeColor = Color.Green;
                else
                    lblNetSavings1.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating statistics: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void showAccountNumber_Click(object sender, EventArgs e)
        {
            if (lblAccountNumber.Tag == null)
                return;

            string original = lblAccountNumber.Tag.ToString();

            if (showAccountNumber.IconChar == FontAwesome.Sharp.IconChar.EyeSlash)
            {
                lblAccountNumber.Text = original;
                showAccountNumber.IconChar = FontAwesome.Sharp.IconChar.Eye;
                showAccountNumber.IconSize = 45;
            }
            else
            {
                if (original.Length > 4)
                {
                    string lastFour = original.Substring(original.Length - 4);
                    string mask = new string('*', original.Length - 4);
                    lblAccountNumber.Text = mask + lastFour;
                }

                showAccountNumber.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
                showAccountNumber.IconSize = 46;
            }
        }

        // ============================================
        // SETUP RECENT TRANSACTIONS DATAGRIDVIEW
        // ============================================
        private void SetupRecentTransactionsDataGridView()
        {
            if (dgvRecentTransactions == null)
                return;

            dgvRecentTransactions.Columns.Clear();
            dgvRecentTransactions.Rows.Clear();

            dgvRecentTransactions.AutoGenerateColumns = false;
            dgvRecentTransactions.RowHeadersVisible = false;
            dgvRecentTransactions.AllowUserToAddRows = false;
            dgvRecentTransactions.AllowUserToResizeRows = false;
            dgvRecentTransactions.ReadOnly = true;
            dgvRecentTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecentTransactions.MultiSelect = false;

            // Header styling
            dgvRecentTransactions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 62, 84);
            dgvRecentTransactions.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRecentTransactions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvRecentTransactions.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvRecentTransactions.ColumnHeadersHeight = 40;
            dgvRecentTransactions.EnableHeadersVisualStyles = false;

            // Cell styling
            dgvRecentTransactions.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvRecentTransactions.DefaultCellStyle.ForeColor = Color.Black;
            dgvRecentTransactions.DefaultCellStyle.BackColor = Color.White;
            dgvRecentTransactions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 144, 255);
            dgvRecentTransactions.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvRecentTransactions.DefaultCellStyle.Padding = new Padding(5, 3, 5, 3);

            dgvRecentTransactions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249);
            dgvRecentTransactions.RowTemplate.Height = 35;

            dgvRecentTransactions.GridColor = Color.FromArgb(230, 230, 230);
            dgvRecentTransactions.BorderStyle = BorderStyle.None;
            dgvRecentTransactions.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // Add columns - Date, Type, Amount
            dgvRecentTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "Date",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            });

            dgvRecentTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Type",
                HeaderText = "Type",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                }
            });

            dgvRecentTransactions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Amount",
                HeaderText = "Amount",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                }
            });
        }

       
        private void DisplayRecentTransactions(int accountId)
        {
            try
            {
                if (dgvRecentTransactions == null)
                    return;

                // Clear existing rows
                dgvRecentTransactions.Rows.Clear();

                // Get last 5 transactions
                var recentTransactions = _transactionRepo.GetRecentTransactions(accountId, 5);

                if (recentTransactions == null || recentTransactions.Count == 0)
                {
                    // Add a single row indicating no transactions
                    dgvRecentTransactions.Rows.Add("No transactions", "", "");
                    return;
                }

                foreach (var transaction in recentTransactions)
                {
                    // Format date, type, and amount
                    string dateText = transaction.TransactionDate.ToString("MM/dd/yyyy hh:mm tt");
                    string typeText = transaction.TransactionType;
                    string amountText = $"₱{transaction.Amount:N2}";

                    // Add row to DataGridView
                    int rowIndex = dgvRecentTransactions.Rows.Add(dateText, typeText, amountText);

                    // Optional: Color code based on transaction type
                    var row = dgvRecentTransactions.Rows[rowIndex];

                    if (transaction.TransactionType == "Deposit" ||
                        transaction.TransactionType == "Transfer In" ||
                        transaction.TransactionType == "Interest Added")
                    {
                        row.Cells["Amount"].Style.ForeColor = Color.Green;
                    }
                    else if (transaction.TransactionType == "Withdrawal" ||
                             transaction.TransactionType == "Transfer Out")
                    {
                        row.Cells["Amount"].Style.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent transactions: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UC_CustomerMySavings_Load(object sender, EventArgs e)
        {
            // Initialize - show masked by default
            showBalance.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            showBalance.IconSize = 46;

            showAccountNumber.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            showAccountNumber.IconSize = 46;

            // Load customer info
            DisplayCustomerInfo();
        }

        private void cbxSelectAccount_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            DisplaySelectedAccount();
        }

        private void lblAccountNumber_Click(object sender, EventArgs e)
        {
        }

        private void guna2Panel14_Paint(object sender, PaintEventArgs e)
        {
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {
        }

        private void btnBackToDashboard_Click(object sender, EventArgs e)
        {
        }

        private void showBalance_Click_1(object sender, EventArgs e)
        {
        }

        private void showAccountNumber_Click_1(object sender, EventArgs e)
        {
        }

        private void lblBalance_Click(object sender, EventArgs e)
        {
        }

        private void guna2HtmlLabel1_Click_1(object sender, EventArgs e)
        {
        }

        private void cbxSelectAccount_SelectedIndexChanged_2(object sender, EventArgs e)
        {
            DisplaySelectedAccount();
        }
    }
}