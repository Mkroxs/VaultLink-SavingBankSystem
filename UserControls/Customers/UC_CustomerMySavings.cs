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
                    DisplayText = $"{account.AccountNumber} - {account.AccountType} ({account.Balance:C2})"
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
            lblBalance.Tag = account.Balance.ToString("C2");

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
            string balanceText = account.Balance.ToString("C2");
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
                lblInterestEarned.Text = "₱0.00"; 
                lblDateValue.Text = account.ClosedDate?.ToString("MM/dd/yyyy") ?? "N/A";
            }

            // Calculate and display financial statistics
            CalculateAndDisplayStats(account.AccountID);
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

                // ---------------------------------------------------------
                // 1. DIRECTLY CALCULATE INTEREST
                // Look for transactions where the description contains "Interest"
                // ---------------------------------------------------------
                decimal totalInterestEarned = transactions
                    .Where(t => t.TransactionType == "Deposit" &&
                               (t.Remarks != null && t.Remarks.Contains("Interest")))
                    .Sum(t => t.Amount);

                // ---------------------------------------------------------
                // 2. CALCULATE DEPOSITS (Excluding Interest)
                // We exclude interest here so "Deposited" reflects only money the customer put in
                // ---------------------------------------------------------
                decimal totalDeposits = transactions
                    .Where(t => t.TransactionType == "Deposit" &&
                               (t.Remarks == null || !t.Remarks.Contains("Interest")))
                    .Sum(t => t.Amount);

                // 3. Calculate Withdrawals
                decimal totalWithdrawals = transactions
                    .Where(t => t.TransactionType == "Withdrawal")
                    .Sum(t => t.Amount);

                // 4. Calculate Transfers
                decimal transfersIn = transactions
                    .Where(t => t.TransactionType == "Transfer In")
                    .Sum(t => t.Amount);

                decimal transfersOut = transactions
                    .Where(t => t.TransactionType == "Transfer Out")
                    .Sum(t => t.Amount);

                // 5. Calculate Net Savings (Money In - Money Out)
                // Note: This matches the Current Balance logic
                decimal netSavings = (totalDeposits + totalInterestEarned + transfersIn) - (totalWithdrawals + transfersOut);

                // ---------------------------------------------------------
                // DISPLAY VALUES
                // ---------------------------------------------------------
                lblNetSavings1.Text = netSavings.ToString("C2");
                lblDeposited.Text = totalDeposits.ToString("C2");
                lblWithdrawn.Text = totalWithdrawals.ToString("C2");

                // Now displaying the ACTUAL summed interest from the database
                lblInterestEarned.Text = totalInterestEarned.ToString("C2");

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