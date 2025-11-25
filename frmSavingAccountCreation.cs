using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaultLinkBankSystem
{
    public partial class frmSavingAccountCreation : Form
    {
        private int _currentCustomerId;
/*        private AccountCreationService _accountService;
*/        private CustomerRepository _customerRepo;
        public frmSavingAccountCreation(int customerId)
        {
            InitializeComponent();
            _currentCustomerId = customerId;
/*            _accountService = new AccountCreationService();
*/            _customerRepo = new CustomerRepository();
        }

        private void frmSavingAccountCreation_Load(object sender, EventArgs e)
        {
            Customers cust = _customerRepo.GetCustomerById(_currentCustomerId);
            if (cust != null)
            {
                lblCustomerNameInformation.Text = cust.FullName;
                lblCustomerCodeInformation.Text = cust.CustomerCode;
            }
            else
            {
                MessageBox.Show("Customer not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*try
            {
                if (!ValidateInputs())
                    return;

                // Check if customer can create more accounts
                if (!_accountService.CanCreateAccount(_currentCustomerId))
                {
                    MessageBox.Show("Maximum number of accounts reached for this customer.",
                        "Account Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                decimal initialDeposit = decimal.Parse(txtInitialDep.Text);

                CustomerKYC kycInfo = new CustomerKYC
                {
                    EmploymentStatus = cmbEmploymentStat.SelectedItem.ToString(),
                    EmployerName = txtEmployerNme.Text,
                    SourceOfFunds = cmbSourceOfFnds.SelectedItem.ToString(),
                    MonthlyIncomeRange = cmbMonthlyIncomeRnge.SelectedItem.ToString(),
                    AccountPurpose = txtAccountPurpose.Text,
                    IDType = cmbIDTyp.SelectedItem.ToString(),
                    IDNumber = "2006202501" // Hardcoded for now
                };

                int latestInterestRateId = _accountService.GetLatestInterestRateId();

                int accountId = _accountService.CreateAccountWithKYC(
            _currentCustomerId,
            initialDeposit,
            kycInfo,
            latestInterestRateId
        );


                MessageBox.Show($"Savings account created successfully!\nAccount ID: {accountId}",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating account: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }





private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtInitialDep.Text))
            {
                MessageBox.Show("Please enter initial deposit amount.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtInitialDep.Focus();
                return false;
            }

            if (!decimal.TryParse(txtInitialDep.Text, out decimal deposit) || deposit < 0)
            {
                MessageBox.Show("Please enter a valid deposit amount.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtInitialDep.Focus();
                return false;
            }

            if (cmbEmploymentStat.SelectedIndex == -1)
            {
                MessageBox.Show("Please select employment status.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbEmploymentStat.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmployerNme.Text))
            {
                MessageBox.Show("Please enter employer name.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmployerNme.Focus();
                return false;
            }

            if (cmbSourceOfFnds.SelectedIndex == -1)
            {
                MessageBox.Show("Please select source of funds.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSourceOfFnds.Focus();
                return false;
            }

            if (cmbMonthlyIncomeRnge.SelectedIndex == -1)
            {
                MessageBox.Show("Please select monthly income range.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbMonthlyIncomeRnge.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAccountPurpose.Text))
            {
                MessageBox.Show("Please enter account purpose.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAccountPurpose.Focus();
                return false;
            }

            if (cmbIDTyp.SelectedIndex == -1)
            {
                MessageBox.Show("Please select ID type.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbIDTyp.Focus();
                return false;
            }

            return true;
        }
    }
}
