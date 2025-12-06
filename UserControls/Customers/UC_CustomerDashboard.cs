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
