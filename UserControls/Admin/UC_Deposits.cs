using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VaultLinkBankSystem.Helpers;

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
using System.Windows.Media;

namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_Deposits : UserControl
    {
        private TransactionRepository _transactionRepo;
        private AccountRepository _accountRepo;
        private CustomerRepository _customerRepo;

        private VaultLinkBankSystem.Customer _selectedCustomer;
        private List<Account> _customerAccounts;


        private decimal accountBalance;
        public UC_Deposits()
        {
            InitializeComponent();
            _transactionRepo = new TransactionRepository();
            _accountRepo = new AccountRepository();
            _customerRepo = new CustomerRepository();


            cbxSelectAccount.DrawMode = DrawMode.OwnerDrawFixed;
            cbxSelectAccount.DrawItem += cbxSelectAccount_DrawItem;

        }

        private void UC_Deposits_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
            UpdateAccountDropdownState();

        }
        private void UpdateAccountDropdownState()
        {
            cbxSelectAccount.Enabled = (_selectedCustomer != null &&
                                        _customerAccounts != null &&
                                        _customerAccounts.Count > 0);
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


        private void ShowCustomerSelectionDialog(List<VaultLinkBankSystem.Customer> customers)
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

                if (dgv.Columns.Contains("CustomerID"))
                    dgv.Columns["CustomerID"].HeaderText = "ID";
                if (dgv.Columns.Contains("CustomerCode"))
                    dgv.Columns["CustomerCode"].HeaderText = "Customer Code";
                if (dgv.Columns.Contains("FullName"))
                    dgv.Columns["FullName"].HeaderText = "Full Name";
                if (dgv.Columns.Contains("Email"))
                    dgv.Columns["Email"].HeaderText = "Email";
                if (dgv.Columns.Contains("Phone"))
                    dgv.Columns["Phone"].HeaderText = "Phone";
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
                VaultLinkBankSystem.Customer selectedCustomer = dgv.SelectedRows[0].DataBoundItem as VaultLinkBankSystem.Customer;
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



        private void DisplayCustomerInfo(VaultLinkBankSystem.Customer customer)
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
            lblTotalBalance.Text = $"₱{totalBalance:N2}";

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
            UpdateAccountDropdownState();

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
                    AccountStatus = account.Status,   // ADD THIS
                    DisplayText = $"{account.AccountNumber} - {account.AccountType} (₱{account.Balance:N2}))"
                });
            }

            if (cbxSelectAccount.Items.Count > 0)
                cbxSelectAccount.SelectedIndex = 0;
        }


        private void btnDeposit_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
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

                // Parse amount
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



                // Confirm Deposit
                DialogResult result = MessageBox.Show(
                     $"Are you sure you want to deposit ₱{amount:N2} from this account?",
                     "Confirm Deposit",
                     MessageBoxButtons.YesNo,
                     MessageBoxIcon.Question);



                if (result == DialogResult.Yes)
                {

                    Transaction transaction = _transactionRepo.Deposit(accountId, amount, "Deposit");

                    MessageBox.Show(
                        $"Deposit successful!\n\n" +
                        $"Transaction ID: {transaction.TransactionID}\n" +
                        $"Amount: ₱{transaction.Amount:N2}\n" +
                        $"Previous Balance: ₱{transaction.PreviousBalance:N2}\n" +
                        $"New Balance: ₱{transaction.NewBalance:N2}" +
                        $"Date: {transaction.TransactionDate:g}",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Generate receipt
                    GenerateReceipt(
                        _selectedCustomer.FullName,
                        accountNumber,
                        amount,
                        transaction.NewBalance,
                        transaction.TransactionID,
                        "WITHDRAWAL");

                    DisplayCustomerInfo(_selectedCustomer);
                    txtDepositAmount.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing withdrawal: {ex.Message}",
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
            cbxSelectAccount.Enabled = false;

        }

        private void ClearForm()
        {
            tbxSearchCustomer.Clear();
            txtDepositAmount.Clear();
            ClearCustomerInfo();
        }


        public void GenerateReceipt(string customerName, string accountNumber, decimal amount, decimal newBalance, int transactionId, string transactionType = "WITHDRAWAL")
        {
            try
            {
                string cleanCustomerName = System.Text.RegularExpressions.Regex.Replace(customerName, @"[^a-zA-Z0-9_]", "_");
                string cleanAccountNumber = System.Text.RegularExpressions.Regex.Replace(accountNumber, @"[^a-zA-Z0-9_]", "_");
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"Receipt_{cleanCustomerName}_{cleanAccountNumber}_{timestamp}.pdf";

                string folder = @"D:\Programming\VaultLinkBankSystem\Transaction_Receipts\Withdraws\";

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string filePath = System.IO.Path.Combine(folder, fileName);

                using (PdfWriter writer = new PdfWriter(filePath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document doc = new Document(pdf, PageSize.A4))
                {
                    doc.SetMargins(50, 50, 50, 50);

                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    AddHeader(doc, boldFont, regularFont);
                    AddTransactionInfo(doc, boldFont, regularFont, transactionId, transactionType);
                    AddCustomerInfo(doc, boldFont, regularFont, customerName, accountNumber);
                    AddTransactionSummary(doc, boldFont, regularFont, amount, newBalance, transactionType);
                    AddFooter(doc, regularFont);
                }

                DialogResult result = MessageBox.Show(
                    $"Receipt generated successfully!\n\nSaved at:\n{filePath}\n\nDo you want to open it now?",
                    "Success",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not open the PDF viewer automatically: " + ex.Message);
                    }
                }
            }
            catch (IOException ioEx)
            {
                MessageBox.Show($"File access error: {ioEx.Message}", "I/O Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddHeader(Document doc, PdfFont boldFont, PdfFont regularFont)
        {
            doc.Add(new Paragraph("TRANSACTION RECEIPT")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20)
                .SetFont(boldFont)
                .SetMarginBottom(5));

            doc.Add(new Paragraph("VaultLink Bank")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(14)
                .SetFont(regularFont)
                .SetMarginBottom(20));

            doc.Add(new LineSeparator(new SolidLine()).SetMarginBottom(15));
        }

        private void AddTransactionInfo(Document doc, PdfFont boldFont, PdfFont regularFont, int transactionId, string transactionType)
        {
            doc.Add(new Paragraph("TRANSACTION INFORMATION")
                .SetFont(boldFont).SetFontSize(12).SetMarginBottom(10));

            doc.Add(new Paragraph($"Transaction ID: {transactionId}").SetFont(regularFont).SetFontSize(11));
            doc.Add(new Paragraph($"Transaction Type: {transactionType}").SetFont(regularFont).SetFontSize(11));
            doc.Add(new Paragraph($"Date & Time: {DateTime.Now:dddd, MMMM dd, yyyy hh:mm:ss tt}")
                .SetFont(regularFont).SetFontSize(11).SetMarginBottom(15));
        }

        private void AddCustomerInfo(Document doc, PdfFont boldFont, PdfFont regularFont, string customerName, string accountNumber)
        {
            doc.Add(new Paragraph("CUSTOMER INFORMATION")
                .SetFont(boldFont).SetFontSize(12).SetMarginBottom(10));

            doc.Add(new Paragraph($"Customer Name: {customerName}").SetFont(regularFont).SetFontSize(11));
            doc.Add(new Paragraph($"Account Number: {accountNumber}")
                .SetFont(regularFont).SetFontSize(11).SetMarginBottom(15));

            doc.Add(new LineSeparator(new SolidLine()).SetMarginBottom(15));
        }

        private void AddTransactionSummary(Document doc, PdfFont boldFont, PdfFont regularFont, decimal amount, decimal newBalance, string transactionType)
        {
            doc.Add(new Paragraph("TRANSACTION SUMMARY")
                .SetFont(boldFont).SetFontSize(12).SetMarginBottom(10));

            decimal previousBalance = transactionType.ToUpper() == "WITHDRAWAL"
                ? newBalance + amount
                : newBalance - amount;

            doc.Add(new Paragraph($"Transaction Amount: ₱{amount:N2}").SetFont(boldFont).SetFontSize(13));
            doc.Add(new Paragraph($"Previous Balance: ₱{previousBalance:N2}").SetFont(regularFont).SetFontSize(11));
            doc.Add(new Paragraph($"New Balance: ₱{newBalance:N2}")
                .SetFont(boldFont).SetFontSize(13).SetMarginBottom(20));

            doc.Add(new LineSeparator(new SolidLine()).SetMarginBottom(15));
        }

        private void AddFooter(Document doc, PdfFont regularFont)
        {
            doc.Add(new Paragraph("Thank you for banking with VaultLink!")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFont(regularFont).SetFontSize(10).SetMarginBottom(5));

            doc.Add(new Paragraph("This is an electronic receipt and does not require a signature.")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFont(regularFont).SetFontSize(8)
                .SetFontColor(PdfColor.GRAY));
        }











        private void cbxSelectAccount_SelectedIndexChanged(object sender, EventArgs e)
        {

            


            if (cbxSelectAccount.SelectedItem == null)
            {
                lblCurrentBalance.Text = "N/A";
                txtDepositAmount.Enabled = false;
                btnDeposit.Enabled = false;
                return;
            }

            try
            {
                dynamic selectedItem = cbxSelectAccount.SelectedItem;
                int accountId = selectedItem.AccountID;

                Account selectedAccount = _customerAccounts
                    .FirstOrDefault(a => a.AccountID == accountId);

                if (selectedAccount == null)
                {
                    lblCurrentBalance.Text = "N/A";
                    txtDepositAmount.Enabled = false;
                    btnDeposit.Enabled = false;
                    return;
                }

                accountBalance = selectedAccount.Balance;
                lblCurrentBalance.Text = $"₱{selectedAccount.Balance:N2}";

                // Compute maximum allowed deposit safely
                decimal maxAllowedDeposit = 100000m - selectedAccount.Balance;
                if (maxAllowedDeposit < 0) maxAllowedDeposit = 0;

                // Check if user already typed something numeric
                decimal typedAmount = 0;
                if (decimal.TryParse(txtDepositAmount.Text.Replace(",", ""), out decimal parsed))
                    typedAmount = parsed;

                bool isClosed = selectedAccount.Status == "Closed";
                bool reachedMaxBalance = selectedAccount.Balance >= 100000m;
                bool exceedingIfTyped = typedAmount > maxAllowedDeposit;

                if (isClosed || reachedMaxBalance || exceedingIfTyped)
                {
                    // Make field read-only
                    txtDepositAmount.Enabled = false;
                    btnDeposit.Enabled = false;

                    cbxSelectAccount.ForeColor = WinFormsColor.Red;

                    if (isClosed)
                    {
                        txtDepositAmount.Text = "Account is closed";
                        txtDepositAmount.ForeColor = WinFormsColor.DarkRed;
                    }
                    else
                    {
                        txtDepositAmount.Text = "Maximum balance reached!";
                        txtDepositAmount.ForeColor = WinFormsColor.DarkRed;
                    }
                }
                else
                {
                    // Account is open and deposit still allowed
                    txtDepositAmount.Enabled = true;
                    btnDeposit.Enabled = true;
                    cbxSelectAccount.ForeColor = WinFormsColor.Green;

                    // Only reset placeholder if field is empty or invalid text
                    if (string.IsNullOrWhiteSpace(txtDepositAmount.Text) ||
                        txtDepositAmount.Text == "Amount" ||
                        txtDepositAmount.Text == "Account is closed" ||
                        txtDepositAmount.Text == "Maximum balance reached!")
                    {
                        txtDepositAmount.Text = "Amount";
                        txtDepositAmount.ForeColor = WinFormsColor.Gray;
                    }
                }
            }
            catch
            {
                lblCurrentBalance.Text = "N/A";
                txtDepositAmount.Enabled = false;
                btnDeposit.Enabled = false;
            }
        }

        private void tbxAccountNumber_TextChanged(object sender, EventArgs e)
        {
            
        }





        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cbxSelectAccount_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            dynamic item = cbxSelectAccount.Items[e.Index];
            string displayText = item.DisplayText;
            string status = item.AccountStatus;

            e.DrawBackground();

            // Choose color
            var color = status == "Closed" ? WinFormsColor.Red : WinFormsColor.Green;

            using (var brush = new System.Drawing.SolidBrush(color))
            {
                e.Graphics.DrawString(displayText, e.Font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void tbxSearchCustomer_Click(object sender, EventArgs e)
        {
            tbxSearchCustomer.Clear();
        }

        private void tbxSearchCustomer_Leave(object sender, EventArgs e)
        {
            if (tbxSearchCustomer.Text.Trim() == "")
            {
                tbxSearchCustomer.Text = "Search Customer";
            }
        }

        private void txtDepositAmount_Leave(object sender, EventArgs e)
        {
            if (txtDepositAmount.Text.Trim() == "")
            {
                txtDepositAmount.Text = "Amount";
            }
        }

        private void txtDepositAmount_Click(object sender, EventArgs e)
        {
            txtDepositAmount.Clear();
        }

        private void txtDepositAmount_TextChanged(object sender, EventArgs e)
        {
            // Prevent recursion
            txtDepositAmount.TextChanged -= txtDepositAmount_TextChanged;

            try
            {
                string text = txtDepositAmount.Text ?? "";
                int cursor = txtDepositAmount.SelectionStart;

                // Ignore placeholder
                if (text == "Amount")
                {
                    txtDepositAmount.TextChanged += txtDepositAmount_TextChanged;
                    return;
                }

                // Remove commas
                string raw = text.Replace(",", "").Trim();

                // If field becomes empty, wipe it safely
                if (raw == "")
                {
                    txtDepositAmount.Text = "";
                    txtDepositAmount.SelectionStart = 0;
                    txtDepositAmount.TextChanged += txtDepositAmount_TextChanged;
                    return;
                }

                // Build filtered numeric text (digits + one dot)
                bool hasDot = false;
                List<char> valid = new List<char>();

                foreach (char c in raw)
                {
                    if (char.IsDigit(c))
                    {
                        valid.Add(c);
                    }
                    else if (c == '.' && !hasDot)
                    {
                        hasDot = true;
                        valid.Add(c);
                    }
                }

                string filtered = new string(valid.ToArray());

                // Disallow decimal as the first char
                if (filtered.StartsWith("."))
                    filtered = filtered.TrimStart('.');

                if (filtered == "")
                {
                    txtDepositAmount.Text = "";
                    txtDepositAmount.SelectionStart = 0;
                    txtDepositAmount.TextChanged += txtDepositAmount_TextChanged;
                    return;
                }

                // Limit decimal places to 2
                if (filtered.Contains("."))
                {
                    int dotIndex = filtered.IndexOf('.');
                    string whole = filtered.Substring(0, dotIndex);
                    string dec = filtered.Substring(dotIndex + 1);

                    if (dec.Length > 2)
                        dec = dec.Substring(0, 2);

                    filtered = whole + "." + dec;
                }

                // Parse safely
                if (!decimal.TryParse(filtered, out decimal amount))
                {
                    txtDepositAmount.TextChanged += txtDepositAmount_TextChanged;
                    return;
                }

                // --- MAX ACCOUNT BALANCE CHECK ---
                decimal newBalance = accountBalance + amount;
                if (newBalance > 100000m)
                {
                    decimal maxAllowedDeposit = 100000m - accountBalance;
                    if (maxAllowedDeposit < 0) maxAllowedDeposit = 0;

                    MessageBox.Show(
                        $"Deposit exceeds the allowed limit.\n" +
                        $"You can only deposit up to ₱{maxAllowedDeposit:N2}.",
                        "Limit Reached",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    // Force textbox to allowable max
                    txtDepositAmount.Text = maxAllowedDeposit.ToString("N2").TrimEnd('0').TrimEnd('.');
                    txtDepositAmount.SelectionStart = txtDepositAmount.Text.Length;
                    txtDepositAmount.TextChanged += txtDepositAmount_TextChanged;
                    return;
                }

                // Format with commas (keep decimals)
                string formatted;
                if (filtered.Contains("."))
                {
                    int dotIndex = filtered.IndexOf('.');
                    string whole = filtered.Substring(0, dotIndex);
                    string dec = filtered.Substring(dotIndex + 1);

                    if (whole == "")
                        whole = "0";

                    long wholeNumber = long.Parse(whole);
                    formatted = string.Format("{0:N0}", wholeNumber) + "." + dec;
                }
                else
                {
                    formatted = string.Format("{0:N0}", long.Parse(filtered));
                }

                // Recalculate cursor
                int newCursor = formatted.Length - (text.Length - cursor);
                if (newCursor < 0) newCursor = 0;
                if (newCursor > formatted.Length) newCursor = formatted.Length;

                txtDepositAmount.Text = formatted;
                txtDepositAmount.SelectionStart = newCursor;
            }
            finally
            {
                txtDepositAmount.TextChanged += txtDepositAmount_TextChanged;
            }
        }
    }
}

