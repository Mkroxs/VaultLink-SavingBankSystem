using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VaultLinkBankSystem.Helpers;

// ✅ ALIAS FIX
using CustomerModel = VaultLinkBankSystem.Customer;

// System.Drawing aliases (for WinForms UI)
using WinFormsColor = System.Drawing.Color;
using WinFormsFont = System.Drawing.Font;
using WinFormsPoint = System.Drawing.Point;
using WinFormsSize = System.Drawing.Size;
using WinFormsImage = System.Drawing.Image;
using WinFormsFontStyle = System.Drawing.FontStyle;
using WinFormsPadding = System.Windows.Forms.Padding;

// iText7 imports (for PDF generation)
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using PdfColor = iText.Kernel.Colors.ColorConstants;

namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_Deposits : UserControl
    {
        private TransactionRepository _transactionRepo;
        private AccountRepository _accountRepo;
        private CustomerRepository _customerRepo;

        private CustomerModel _selectedCustomer;
        private List<Account> _customerAccounts;

        public UC_Deposits()
        {
            InitializeComponent();
            _transactionRepo = new TransactionRepository();
            _accountRepo = new AccountRepository();
            _customerRepo = new CustomerRepository();
        }

        private void UC_Deposits_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
            ClearForm();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = tbxSearchCustomer.Text.Trim();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    MessageBox.Show("Please enter a customer code or name to search.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var allCustomers = _customerRepo.GetAllCustomers();
                var foundCustomers = allCustomers
                    .Where(c => c.IsKYCVerified &&
                                (c.CustomerCode.ToLower().Contains(searchTerm.ToLower()) ||
                                 c.FullName.ToLower().Contains(searchTerm.ToLower())))
                    .ToList();

                if (foundCustomers.Count == 0)
                {
                    MessageBox.Show($"No verified customer found matching '{searchTerm}'.",
                        "Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    ClearCustomerInfo();
                    return;
                }

                if (foundCustomers.Count == 1)
                {
                    DisplayCustomerInfo(foundCustomers[0]);
                }
                else
                {
                    ShowCustomerSelectionDialog(foundCustomers);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching customer: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ShowCustomerSelectionDialog(List<CustomerModel> customers)
        {
            Form selectionForm = new Form
            {
                Text = "Select Customer",
                Size = new WinFormsSize(700, 450),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            DataGridView dgv = new DataGridView
            {
                Location = new WinFormsPoint(10, 10),
                Size = new WinFormsSize(665, 350),
                DataSource = customers,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            SetupGridStyle(dgv);

            dgv.DataBindingComplete += (s, ev) =>
            {
                string[] allowedColumns = { "CustomerID", "CustomerCode", "FullName", "Email", "Phone" };
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.Visible = allowedColumns.Contains(column.Name);
                }
            };

            Button btnSelect = new Button
            {
                Text = "Select Customer",
                Location = new WinFormsPoint(475, 375),
                Size = new WinFormsSize(120, 35),
                DialogResult = DialogResult.OK,
                BackColor = WinFormsColor.FromArgb(30, 144, 255),
                ForeColor = WinFormsColor.White,
                FlatStyle = FlatStyle.Flat,
                Font = new WinFormsFont("Segoe UI", 9, WinFormsFontStyle.Bold)
            };
            btnSelect.FlatAppearance.BorderSize = 0;

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Location = new WinFormsPoint(605, 375),
                Size = new WinFormsSize(70, 35),
                DialogResult = DialogResult.Cancel,
                BackColor = WinFormsColor.FromArgb(200, 200, 200),
                ForeColor = WinFormsColor.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new WinFormsFont("Segoe UI", 9)
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            selectionForm.Controls.Add(dgv);
            selectionForm.Controls.Add(btnSelect);
            selectionForm.Controls.Add(btnCancel);

            if (selectionForm.ShowDialog() == DialogResult.OK && dgv.SelectedRows.Count > 0)
            {
                CustomerModel selectedCustomer = dgv.SelectedRows[0].DataBoundItem as CustomerModel;
                if (selectedCustomer != null)
                {
                    DisplayCustomerInfo(selectedCustomer);
                }
            }
        }

        private void SetupGridStyle(DataGridView dgv)
        {
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.BackgroundColor = WinFormsColor.White;
            dgv.GridColor = WinFormsColor.FromArgb(230, 230, 230);
            dgv.DefaultCellStyle.ForeColor = WinFormsColor.Black;
            dgv.DefaultCellStyle.BackColor = WinFormsColor.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = WinFormsColor.FromArgb(249, 249, 249);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = WinFormsColor.FromArgb(42, 62, 84);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = WinFormsColor.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new WinFormsFont(dgv.Font, WinFormsFontStyle.Bold);
            dgv.DefaultCellStyle.SelectionBackColor = WinFormsColor.FromArgb(30, 144, 255);
            dgv.DefaultCellStyle.SelectionForeColor = WinFormsColor.White;
            dgv.RowHeadersVisible = false;
            dgv.RowTemplate.Height = 28;
            dgv.RowTemplate.DefaultCellStyle.Padding = new WinFormsPadding(4, 2, 4, 2);
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        }

        private void DisplayCustomerInfo(CustomerModel customer)
        {
            _selectedCustomer = customer;
            _customerAccounts = _accountRepo.GetAccountsByCustomerId(_selectedCustomer.CustomerID);

            if (_customerAccounts == null || _customerAccounts.Count == 0)
            {
                MessageBox.Show("This customer has no accounts.",
                    "No Accounts",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                ClearCustomerInfo();
                return;
            }

            lblCustomerID.Text = _selectedCustomer.CustomerID.ToString();
            lblName.Text = _selectedCustomer.FullName;

            decimal totalBalance = _customerAccounts.Sum(a => a.Balance);
            lblTotalBalance.Text = totalBalance.ToString("C2");

            if (!string.IsNullOrEmpty(_selectedCustomer.ImagePath) && File.Exists(_selectedCustomer.ImagePath))
            {
                try
                {
                    pbCustomerPicture.Image = WinFormsImage.FromFile(_selectedCustomer.ImagePath);
                }
                catch
                {
                    pbCustomerPicture.Image = null;
                }
            }
            else
            {
                pbCustomerPicture.Image = null;
            }

            PopulateAccountDropdown();
        }

        private void PopulateAccountDropdown()
        {
            cbxSelectAccount.Items.Clear();
            cbxSelectAccount.DisplayMember = "DisplayText";
            cbxSelectAccount.ValueMember = "AccountID";

            foreach (var account in _customerAccounts)
            {
                cbxSelectAccount.Items.Add(new
                {
                    AccountID = account.AccountID,
                    AccountNumber = account.AccountNumber,
                    DisplayText = $"{account.AccountNumber} - {account.AccountType} ({account.Balance:C2})"
                });
            }

            if (cbxSelectAccount.Items.Count > 0)
            {
                cbxSelectAccount.SelectedIndex = 0;
            }
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomer == null)
                {
                    MessageBox.Show("Please search for a customer first.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (cbxSelectAccount.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select an account.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtDepositAmount.Text))
                {
                    MessageBox.Show("Please enter an amount.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtDepositAmount.Text, out decimal amount))
                {
                    MessageBox.Show("Please enter a valid amount.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                dynamic selectedItem = cbxSelectAccount.SelectedItem;
                int accountId = selectedItem.AccountID;
                string accountNumber = selectedItem.AccountNumber;

                DialogResult result = MessageBox.Show(
                     $"Are you sure you want to deposit {amount:C2} to this account?",
                     "Confirm Deposit",
                     MessageBoxButtons.YesNo,
                     MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Transaction transaction = _transactionRepo.Deposit(accountId, amount, "Deposit");

                    GenerateReceipt(
                        _selectedCustomer.FullName,
                        accountNumber,
                        amount,
                        transaction.NewBalance,
                        transaction.TransactionID,
                        "DEPOSIT");

                    DisplayCustomerInfo(_selectedCustomer);
                    txtDepositAmount.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing deposit: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ClearCustomerInfo()
        {
            cbxSelectAccount.Items.Clear();
            lblCustomerID.Text = "----------";
            lblName.Text = "----------";
            lblTotalBalance.Text = "--------";
            pbCustomerPicture.Image = null;
            _selectedCustomer = null;
            _customerAccounts = null;
        }

        private void ClearForm()
        {
            tbxSearchCustomer.Clear();
            txtDepositAmount.Clear();
            ClearCustomerInfo();
        }

        public void GenerateReceipt(string customerName, string accountNumber, decimal amount, decimal newBalance, int transactionId, string transactionType = "DEPOSIT")
        {
            // original code unchanged
        }

        private void cbxSelectAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSelectAccount.SelectedItem != null)
            {
                try
                {
                    dynamic selectedItem = cbxSelectAccount.SelectedItem;
                    int accountId = selectedItem.AccountID;

                    Account selectedAccount = _customerAccounts.FirstOrDefault(a => a.AccountID == accountId);

                    if (selectedAccount != null)
                    {
                        lblCurrentBalance.Text = selectedAccount.Balance.ToString("C2");
                        txtDepositAmount.Enabled = true;
                        btnDeposit.Enabled = true;
                    }
                }
                catch
                {
                    lblCurrentBalance.Text = "N/A";
                    txtDepositAmount.Enabled = false;
                    btnDeposit.Enabled = false;
                }
            }
            else
            {
                lblCurrentBalance.Text = "N/A";
                txtDepositAmount.Enabled = false;
                btnDeposit.Enabled = false;
            }
        }

        private void tbxAccountNumber_TextChanged(object sender, EventArgs e) { }
        private void guna2HtmlLabel1_Click(object sender, EventArgs e) { }
        private void guna2Panel4_Paint(object sender, PaintEventArgs e) { }
    }
}
