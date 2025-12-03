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
        private List<Account> _customerAccounts;

        public UC_CustomerMySavings(VaultLinkBankSystem.Customer currentCustomer)
        {
            InitializeComponent();
            _currentCustomer = currentCustomer;

            _accountRepo = new AccountRepository();
            


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
                    Account = account, // diri e store ang full object
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
                string mask = new string('*', original.Length);
                lblBalance.Text = mask;

            }
            else
            {

                lblBalance.Text = original;
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

            
            lblAccountNumber.Text = account.AccountNumber;
            lblBalance.Text = account.Balance.ToString("C2");
*/
            
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

            
        }










        private void showAccountNumber_Click(object sender, EventArgs e)
        {
            if (lblAccountNumber.Tag == null)
                lblAccountNumber.Tag = lblAccountNumber.Text;

            string original = lblAccountNumber.Tag.ToString();

            if (showAccountNumber.IconChar == FontAwesome.Sharp.IconChar.EyeSlash)
            {

                if (original.Length > 4)
                {
                    string lastFour = original.Substring(original.Length - 4);
                    string mask = new string('*', original.Length - 4);
                    lblAccountNumber.Text = mask + lastFour;
                }

                showAccountNumber.IconChar = FontAwesome.Sharp.IconChar.Eye;
                showAccountNumber.IconSize = 45;
            }
            else
            {
                lblAccountNumber.Text = original;


                showAccountNumber.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
                showAccountNumber.IconSize = 46;
            }
        }

        private void UC_CustomerMySavings_Load(object sender, EventArgs e)
        {
            DisplayCustomerInfo();

        }

        private void cbxSelectAccount_SelectedIndexChanged(object sender, EventArgs e)
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
    }
}
