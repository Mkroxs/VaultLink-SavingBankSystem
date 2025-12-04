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
    public partial class UC_InterestComputation : UserControl
    {
        private AccountRepository _accountRepo;
        private TransactionRepository _transactionRepo;
        private CustomerRepository _customerRepo;
        private readonly InterestRateRepository _interestRepo = new InterestRateRepository();

        public UC_InterestComputation()
        {
            InitializeComponent();
            _accountRepo = new AccountRepository();
            _transactionRepo = new TransactionRepository();
            _customerRepo = new CustomerRepository();

            InitializeControls();
        }


        private void InitializeControls()
        {
            // Set default date to current month
            dtpSelectMonth.Value = DateTime.Now;
            dtpSelectMonth.Format = DateTimePickerFormat.Custom;
            dtpSelectMonth.CustomFormat = "MMMM, yyyy";
            dtpSelectMonth.ShowUpDown = true; // Shows month/year selector

            // Setup DataGridView
            SetupDataGridView();

            // Load preview data
            LoadAccountsPreview();
        }

        private void UC_InterestComputation_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
        }


        private void SetupDataGridView()
        {
            dvgListOfCustomers.Columns.Clear();
            dvgListOfCustomers.Rows.Clear();

            dvgListOfCustomers.AutoGenerateColumns = false;
            dvgListOfCustomers.RowHeadersVisible = false;
            dvgListOfCustomers.AllowUserToAddRows = false;
            dvgListOfCustomers.AllowUserToResizeRows = false;
            dvgListOfCustomers.ReadOnly = true;
            dvgListOfCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dvgListOfCustomers.MultiSelect = false;

            // Header styling
            dvgListOfCustomers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 62, 84);
            dvgListOfCustomers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dvgListOfCustomers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dvgListOfCustomers.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dvgListOfCustomers.ColumnHeadersHeight = 40;
            dvgListOfCustomers.EnableHeadersVisualStyles = false;

            // Cell styling
            dvgListOfCustomers.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dvgListOfCustomers.DefaultCellStyle.ForeColor = Color.Black;
            dvgListOfCustomers.DefaultCellStyle.BackColor = Color.White;
            dvgListOfCustomers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 144, 255);
            dvgListOfCustomers.DefaultCellStyle.SelectionForeColor = Color.White;
            dvgListOfCustomers.DefaultCellStyle.Padding = new Padding(5, 3, 5, 3);

            dvgListOfCustomers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249);
            dvgListOfCustomers.RowTemplate.Height = 35;

            dvgListOfCustomers.GridColor = Color.FromArgb(230, 230, 230);
            dvgListOfCustomers.BorderStyle = BorderStyle.None;
            dvgListOfCustomers.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // Add columns
            dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AccountNumber",
                HeaderText = "Account Number",
                Width = 150
            });

            dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerName",
                HeaderText = "Customer Name",
                Width = 200
            });

            dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AccountType",
                HeaderText = "Account Type",
                Width = 120
            });

            dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CurrentBalance",
                HeaderText = "Current Balance",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                }
            });

            dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "InterestAmount",
                HeaderText = "Interest Amount",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "C2",
                    ForeColor = Color.Green,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                }
            });

            dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NewBalance",
                HeaderText = "New Balance",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "C2",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                }
            });
            dvgListOfCustomers.CellFormatting += DvgListOfCustomers_CellFormatting;
        }

        private void DvgListOfCustomers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string[] moneyColumns = { "CurrentBalance", "InterestAmount", "NewBalance" };

            if (moneyColumns.Contains(dvgListOfCustomers.Columns[e.ColumnIndex].Name))
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal amount))
                {
                    e.Value = "₱" + amount.ToString("#,##0.00");
                    e.FormattingApplied = true;
                }
            }
        }

        private void LoadAccountsPreview()
        {
            try
            {
                dvgListOfCustomers.Rows.Clear();

                // Get all active savings accounts
                var allAccounts = _accountRepo.GetAllAccounts();
                var savingsAccounts = allAccounts.Where(a => 
                    a.AccountType.ToLower().Contains("savings") && 
                    a.Status.ToLower() == "active").ToList();

                if (chkApplyToAll.Checked)
                {
                    // Show all active savings accounts
                    foreach (var account in savingsAccounts)
                    {
                        AddAccountRow(account);
                    }
                }

                // Update summary
                UpdateSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void AddAccountRow(Account account)
        {
            try
            {
                var customer = _customerRepo.GetCustomerById(account.CustomerID);

                // FIXED — pass the Account object, not the Balance
                decimal interestAmount = CalculateMonthlyInterest(account);

                decimal newBalance = account.Balance + interestAmount;

                dvgListOfCustomers.Rows.Add(
                    account.AccountNumber,
                    customer?.FullName ?? "Unknown",
                    account.AccountType,
                    account.Balance,
                    interestAmount,
                    newBalance
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding account row: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private decimal CalculateMonthlyInterest(Account account)
        {
            decimal annualRate = _accountRepo.GetAnnualRateForAccount(account.AccountID);
            decimal monthlyRate = annualRate / 12m;
            decimal interest = account.Balance * monthlyRate;

            return Math.Round(interest, 2);
        }



        private void UpdateSummary()
        {
            int accountCount = dvgListOfCustomers.Rows.Count;
            decimal totalInterest = 0;

            foreach (DataGridViewRow row in dvgListOfCustomers.Rows)
            {
                if (row.Cells["InterestAmount"].Value != null)
                {
                    totalInterest += Convert.ToDecimal(row.Cells["InterestAmount"].Value);
                }
            }
            decimal annualRate = _interestRepo.GetCurrentAnnualRate();




            lblAccountCount.Text = $"Accounts: {accountCount}";
            lblTotalInterest.Text = $"Total Interest: {totalInterest:C2}";
            lblInterestRate.Text = $"Rate: {annualRate:P2} per year ({(annualRate / 12m):P4} per month)";

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chkApplyToAll_CheckedChanged(object sender, EventArgs e)
        {
            LoadAccountsPreview();
        }

        private void dtpSelectMonth_ValueChanged(object sender, EventArgs e)
        {
            lblSelectedMonth.Text = $"Computing interest for: {dtpSelectMonth.Value:MMMM yyyy}";
        }

        private void btnComputeInterest_Click(object sender, EventArgs e)
        {
            try
            {
                if (dvgListOfCustomers.Rows.Count == 0)
                {
                    MessageBox.Show("No accounts to process. Please check 'Apply interest to all active savings accounts'.",
                        "No Accounts",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Confirmation dialog
                int accountCount = dvgListOfCustomers.Rows.Count;
                decimal totalInterest = 0;

                foreach (DataGridViewRow row in dvgListOfCustomers.Rows)
                {
                    totalInterest += Convert.ToDecimal(row.Cells["InterestAmount"].Value);
                }


                decimal annualRate = _interestRepo.GetCurrentAnnualRate();



                DialogResult result = MessageBox.Show(
                    $"Apply interest to {accountCount} savings accounts?\n\n" +
                    $"Total Interest to be added: {totalInterest:C2}\n" +
                    $"Interest Rate: {annualRate:P2} per year\n" +
                    $"Month: {dtpSelectMonth.Value:MMMM yyyy}\n\n" +
                    $"This action cannot be undone.",
                    "Confirm Interest Application",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ApplyInterest();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error computing interest: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ApplyInterest()
        {
            try
            {
                int successCount = 0;
                int failCount = 0;
                List<string> errors = new List<string>();

                foreach (DataGridViewRow row in dvgListOfCustomers.Rows)
                {
                    try
                    {
                        string accountNumber = row.Cells["AccountNumber"].Value.ToString();
                        decimal interestAmount = Convert.ToDecimal(row.Cells["InterestAmount"].Value);

                        // Get account
                        var account = _accountRepo.GetAccountByAccountNumber(accountNumber);

                        if (account != null && interestAmount > 0)
                        {
                            // Create deposit transaction for interest
                            _transactionRepo.Deposit(
                                account.AccountID,
                                interestAmount,
                                $"Monthly Interest - {dtpSelectMonth.Value:MMMM yyyy}");

                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        errors.Add($"Account {row.Cells["AccountNumber"].Value}: {ex.Message}");
                    }
                }

                // Show results
                string message = $"Interest application completed!\n\n" +
                                $"Successful: {successCount}\n" +
                                $"Failed: {failCount}";

                if (errors.Count > 0 && errors.Count <= 5)
                {
                    message += "\n\nErrors:\n" + string.Join("\n", errors);
                }
                else if (errors.Count > 5)
                {
                    message += $"\n\n{errors.Count} errors occurred. Check logs for details.";
                }

                MessageBox.Show(message,
                    "Interest Application Complete",
                    MessageBoxButtons.OK,
                    failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                // Refresh preview
                LoadAccountsPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying interest: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


    }
}
