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
using static Syncfusion.Windows.Forms.TabBar;

namespace VaultLinkBankSystem.UserControls.Customers
{
    public partial class UC_CustomerDashboard : UserControl
    {
        private VaultLinkBankSystem.Customer _currentCustomer;
        private AccountRepository _accountRepo;
        private CustomerRepository _customerRepo;
        private readonly TransactionRepository _transactionRepo;

        private readonly InterestRateRepository _interestRepo = new InterestRateRepository();
        public UC_CustomerDashboard(VaultLinkBankSystem.Customer customer)
        {
            InitializeComponent();
            _currentCustomer = customer;

            _accountRepo = new AccountRepository();
            _customerRepo = new CustomerRepository();
            _transactionRepo = new TransactionRepository();

            LoadRecentTransactions();
        }

        private void guna2HtmlLabel8_Click(object sender, EventArgs e)
        {
        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnMySavings_Click(object sender, EventArgs e)
        {
            frmCustomerDashboard form = this.FindForm() as frmCustomerDashboard;

            if (form != null)
            {
                form.ShowMySavings();
            }
        }

        private void btnTransactionHistory_Click(object sender, EventArgs e)
        {
            frmCustomerDashboard form = this.FindForm() as frmCustomerDashboard;

            if (form != null)
            {
                form.ShowTransactionHistory();
            }
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            frmCustomerDashboard form = this.FindForm() as frmCustomerDashboard;

            if (form != null)
            {
                form.ShowProfile();
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            frmLogin login = new frmLogin();
            login.Show();

            Form parent = this.FindForm();
            if (parent != null)
                parent.Close();
        }

        private void UC_CustomerDashboard_Load(object sender, EventArgs e)
        {
            decimal annualRate = _interestRepo.GetCurrentAnnualRate();
            var accounts = _accountRepo.GetAccountsByCustomerId(_currentCustomer.CustomerID);

            lblInterestRate.Text = $"{annualRate:P2}";
            lblActiveAccounts.Text = $"{accounts.Count} Accounts";

        }




        private void LoadRecentTransactions()
        {
            try
            {
                if (_transactionRepo == null)
                    throw new Exception("TransactionRepo was not initialized.");

                dgvRecentTransactions.Rows.Clear();
                dgvRecentTransactions.Columns.Clear();

                dgvRecentTransactions.Columns.Add("Account", "Account");
                dgvRecentTransactions.Columns.Add("Type", "Type");
                dgvRecentTransactions.Columns.Add("Amount", "Amount");
                dgvRecentTransactions.Columns.Add("Date", "Date");

                dgvRecentTransactions.Columns["Account"].Width = 120;
                dgvRecentTransactions.Columns["Type"].Width = 80;
                dgvRecentTransactions.Columns["Amount"].Width = 100;
                dgvRecentTransactions.Columns["Date"].Width = 140;
                dgvRecentTransactions.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dgvRecentTransactions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(25, 45, 75);
                dgvRecentTransactions.RowTemplate.Height = 32;

                dgvRecentTransactions.Columns["Account"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvRecentTransactions.DefaultCellStyle.Padding = new Padding(5, 3, 5, 3);
                dgvRecentTransactions.ColumnHeadersHeight = 35;
                dgvRecentTransactions.DefaultCellStyle.Font = new Font("Segoe UI", 9);
                dgvRecentTransactions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

                dgvRecentTransactions.BackgroundColor = Color.FromArgb(20, 40, 70);
                dgvRecentTransactions.ForeColor = Color.White;
                dgvRecentTransactions.DefaultCellStyle.BackColor = Color.FromArgb(20, 40, 70);
                dgvRecentTransactions.DefaultCellStyle.ForeColor = Color.White;
                dgvRecentTransactions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 60, 90);
                dgvRecentTransactions.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvRecentTransactions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 30, 60);
                dgvRecentTransactions.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvRecentTransactions.EnableHeadersVisualStyles = false;
                dgvRecentTransactions.GridColor = Color.FromArgb(40, 60, 90);
                dgvRecentTransactions.BorderStyle = BorderStyle.None;
                dgvRecentTransactions.RowHeadersVisible = false;
                dgvRecentTransactions.AllowUserToAddRows = false;
                dgvRecentTransactions.ReadOnly = true;
                dgvRecentTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                var allAccounts = _accountRepo.GetAccountsByCustomerId(_currentCustomer.CustomerID);
                var allTransactions = new List<Transaction>();

                foreach (var account in allAccounts)
                {
                    var transactions = _transactionRepo.GetTransactionsByAccountId(account.AccountID);
                    allTransactions.AddRange(transactions);

                }



                var recent = allTransactions
            .OrderByDescending(t => t.TransactionDate)
            .Take(10)
            .ToList();

                foreach (var t in recent)
                {
                    var account = allAccounts.FirstOrDefault(a => a.AccountID == t.AccountID);

                    dgvRecentTransactions.Rows.Add(
                        account?.AccountNumber ?? "Unknown Account",
                        t.TransactionType,
                        "₱" + t.Amount.ToString("#,##0.00"),
                        t.TransactionDate.ToString("MM/dd/yyyy hh:mm tt")
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading recent transactions:\n" + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void lblTotalBalance_Click(object sender, EventArgs e)
        {

        }

        private void dgvRecentTransactions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2ShadowPanel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
