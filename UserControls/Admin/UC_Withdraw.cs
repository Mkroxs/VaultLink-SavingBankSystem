using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VaultLinkBankSystem.Helpers;

// WinForms aliases (avoid conflicts with iText types)
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
using IOPath = System.IO.Path;

namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_Withdraw : UserControl
    {
        private TransactionRepository _transactionRepo;
        private AccountRepository _accountRepo;
        private CustomerRepository _customerRepo;

        private VaultLinkBankSystem.Customer _selectedCustomer;
        private List<Account> _customerAccounts = new List<Account>();
        private decimal accountBalance = 0m;

        public UC_Withdraw()
        {
            InitializeComponent();

            // repositories
            _transactionRepo = new TransactionRepository();
            _accountRepo = new AccountRepository();
            _customerRepo = new CustomerRepository();

            // ComboBox owner draw for status color
            cbxSelectAccount.DrawMode = DrawMode.OwnerDrawFixed;
            cbxSelectAccount.DrawItem += cbxSelectAccount_DrawItem;

            // Ensure event handlers are attached (safe to reattach)
            btnWithdraw.Click -= btnWthdraw_Click;
            btnWithdraw.Click += btnWthdraw_Click;

            btnSearch.Click -= btnSearch_Click;
            btnSearch.Click += btnSearch_Click;

            cbxSelectAccount.SelectedIndexChanged -= cbxSelectAccount_SelectedIndexChanged;
            cbxSelectAccount.SelectedIndexChanged += cbxSelectAccount_SelectedIndexChanged;

            tbxSearchCustomer.Click -= tbxSearchCustomer_Click;
            tbxSearchCustomer.Click += tbxSearchCustomer_Click;

            tbxSearchCustomer.Leave -= tbxSearchCustomer_Leave;
            tbxSearchCustomer.Leave += tbxSearchCustomer_Leave;

            txtWithdrawAmount.Click -= txtWithdrawAmount_Click;
            txtWithdrawAmount.Click += txtWithdrawAmount_Click;

            txtWithdrawAmount.Leave -= txtWithdrawAmount_Leave;
            txtWithdrawAmount.Leave += txtWithdrawAmount_Leave;

            txtWithdrawAmount.TextChanged -= txtWithdrawAmount_TextChanged;
            txtWithdrawAmount.TextChanged += txtWithdrawAmount_TextChanged;

            txtWithdrawAmount.KeyPress -= txtWithdrawAmount_KeyPress;
            txtWithdrawAmount.KeyPress += txtWithdrawAmount_KeyPress;
        }

        private void UC_Withdraw_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
            ResetPlaceholders();
        }

        // -----------------------
        // Search + selection UI
        // -----------------------
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = tbxSearchCustomer.Text?.Trim() ?? "";

                if (string.IsNullOrEmpty(searchTerm) || searchTerm == "Search Customer")
                {
                    MessageBox.Show("Please enter a customer code or name to search.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var all = _customerRepo.GetAllCustomers();
                var found = all.Where(c =>
                    c.IsKYCVerified &&
                    ((c.CustomerCode ?? "").ToLower().Contains(searchTerm.ToLower()) ||
                     (c.FullName ?? "").ToLower().Contains(searchTerm.ToLower()))
                ).ToList();

                if (found.Count == 0)
                {
                    MessageBox.Show($"No verified customer found matching '{searchTerm}'.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearCustomerInfo();
                    return;
                }

                if (found.Count == 1)
                {
                    DisplayCustomerInfo(found[0]);
                }
                else
                {
                    ShowCustomerSelectionDialog(found);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowCustomerSelectionDialog(List<VaultLinkBankSystem.Customer> customers)
        {
            using (Form selectionForm = new Form())
            {
                selectionForm.Text = "Select Customer";
                selectionForm.Size = new WinFormsSize(700, 450);
                selectionForm.StartPosition = FormStartPosition.CenterParent;
                selectionForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                selectionForm.MaximizeBox = false;
                selectionForm.MinimizeBox = false;

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
                        column.Visible = allowedColumns.Contains(column.Name);

                    if (dgv.Columns.Contains("CustomerID")) dgv.Columns["CustomerID"].HeaderText = "ID";
                    if (dgv.Columns.Contains("CustomerCode")) dgv.Columns["CustomerCode"].HeaderText = "Customer Code";
                    if (dgv.Columns.Contains("FullName")) dgv.Columns["FullName"].HeaderText = "Full Name";
                };

                Button btnSelect = new Button
                {
                    Text = "Select Customer",
                    Location = new WinFormsPoint(480, 365),
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
                    Location = new WinFormsPoint(605, 365),
                    Size = new WinFormsSize(70, 35),
                    DialogResult = DialogResult.Cancel
                };

                selectionForm.Controls.Add(dgv);
                selectionForm.Controls.Add(btnSelect);
                selectionForm.Controls.Add(btnCancel);

                if (selectionForm.ShowDialog() == DialogResult.OK && dgv.SelectedRows.Count > 0)
                {
                    var selectedCustomer = dgv.SelectedRows[0].DataBoundItem as VaultLinkBankSystem.Customer;
                    if (selectedCustomer != null) DisplayCustomerInfo(selectedCustomer);
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

        // -----------------------
        // Display customer + accounts
        // -----------------------
        private void DisplayCustomerInfo(VaultLinkBankSystem.Customer customer)
        {
            _selectedCustomer = customer;
            _customerAccounts = _accountRepo.GetAccountsByCustomerId(_selectedCustomer.CustomerID) ?? new List<Account>();

            if (_customerAccounts.Count == 0)
            {
                MessageBox.Show("This customer has no accounts.", "No Accounts", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClearCustomerInfo();
                return;
            }

            lblCustomerID.Text = _selectedCustomer.CustomerID.ToString();
            lblName.Text = _selectedCustomer.FullName;
            lblTotalBalance.Text = $"₱{_customerAccounts.Sum(a => a.Balance):N2}";

            if (!string.IsNullOrEmpty(_selectedCustomer.ImagePath) && File.Exists(_selectedCustomer.ImagePath))
            {
                try { pbCustomerPicture.Image = WinFormsImage.FromFile(_selectedCustomer.ImagePath); }
                catch { pbCustomerPicture.Image = null; }
            }
            else pbCustomerPicture.Image = null;

            PopulateAccountDropdown();
        }

        private void PopulateAccountDropdown()
        {
            cbxSelectAccount.Items.Clear();
            cbxSelectAccount.DisplayMember = "DisplayText";
            cbxSelectAccount.ValueMember = "AccountID";

            foreach (var acc in _customerAccounts)
            {
                cbxSelectAccount.Items.Add(new
                {
                    AccountID = acc.AccountID,
                    AccountNumber = acc.AccountNumber,
                    AccountStatus = acc.Status,
                    DisplayText = $"{acc.AccountNumber} - {acc.AccountType} (₱{acc.Balance:N2})"
                });
            }

            if (cbxSelectAccount.Items.Count > 0) cbxSelectAccount.SelectedIndex = 0;
        }

        // -----------------------
        // ComboBox draw + selection
        // -----------------------
        private void cbxSelectAccount_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            dynamic item = cbxSelectAccount.Items[e.Index];
            string text = item.DisplayText;
            string status = item.AccountStatus;

            e.DrawBackground();

            var color = status == "Closed" ? WinFormsColor.Red : WinFormsColor.Green;
            using (var brush = new System.Drawing.SolidBrush(color))
                e.Graphics.DrawString(text, e.Font, brush, e.Bounds);

            e.DrawFocusRectangle();
        }

        private void cbxSelectAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSelectAccount.SelectedItem == null)
            {
                lblCurrentBalance.Text = "N/A";
                txtWithdrawAmount.Enabled = false;
                btnWithdraw.Enabled = false;
                return;
            }

            try
            {
                dynamic selectedItem = cbxSelectAccount.SelectedItem;
                int accountId = selectedItem.AccountID;

                Account selectedAccount = _customerAccounts.FirstOrDefault(a => a.AccountID == accountId);
                if (selectedAccount == null)
                {
                    lblCurrentBalance.Text = "N/A";
                    txtWithdrawAmount.Enabled = false;
                    btnWithdraw.Enabled = false;
                    return;
                }

                accountBalance = selectedAccount.Balance;
                lblCurrentBalance.Text = $"₱{selectedAccount.Balance:N2}";

                bool isClosed = selectedAccount.Status == "Closed";
                if (isClosed)
                {
                    cbxSelectAccount.ForeColor = WinFormsColor.Red;
                    txtWithdrawAmount.Enabled = false;
                    btnWithdraw.Enabled = false;
                    txtWithdrawAmount.Text = "Account is closed";
                    txtWithdrawAmount.ForeColor = WinFormsColor.DarkRed;
                }
                else
                {
                    cbxSelectAccount.ForeColor = WinFormsColor.Green;
                    txtWithdrawAmount.Enabled = true;
                    btnWithdraw.Enabled = true;

                    if (string.IsNullOrWhiteSpace(txtWithdrawAmount.Text) || txtWithdrawAmount.Text == "Amount" || txtWithdrawAmount.Text == "Account is closed")
                    {
                        txtWithdrawAmount.Text = "Amount";
                        txtWithdrawAmount.ForeColor = WinFormsColor.Gray;
                    }
                    else
                    {
                        txtWithdrawAmount.ForeColor = WinFormsColor.Black;
                    }
                }
            }
            catch
            {
                lblCurrentBalance.Text = "N/A";
                txtWithdrawAmount.Enabled = false;
                btnWithdraw.Enabled = false;
            }
        }

        // -----------------------
        // Withdraw action
        // -----------------------
        private void btnWthdraw_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomer == null)
                {
                    MessageBox.Show("Please search for a customer first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cbxSelectAccount.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select an account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // sanitize numeric text (remove commas)
                string raw = (txtWithdrawAmount.Text ?? "").Replace(",", "").Trim();
                if (string.IsNullOrEmpty(raw) || raw == "Amount" || raw == "Account is closed")
                {
                    MessageBox.Show("Please enter an amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(raw, out decimal amount))
                {
                    MessageBox.Show("Please enter a valid amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (amount <= 0)
                {
                    MessageBox.Show("Amount must be greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                dynamic selectedItem = cbxSelectAccount.SelectedItem;
                int accountId = selectedItem.AccountID;
                string accountNumber = selectedItem.AccountNumber;

                Account account = _customerAccounts.FirstOrDefault(a => a.AccountID == accountId);
                if (account == null)
                {
                    MessageBox.Show("Selected account not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (account.Status == "Closed")
                {
                    MessageBox.Show("Account is closed. Withdrawal not allowed.", "Account Closed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (account.Balance < amount)
                {
                    MessageBox.Show($"Insufficient funds. Current balance ₱{account.Balance:N2}.", "Insufficient Funds", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    $"Are you sure you want to withdraw ₱{amount:N2} from account {accountNumber}?",
                    "Confirm Withdrawal",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes) return;

                Transaction transaction = _transactionRepo.Withdraw(accountId, amount, "Withdrawal");

                MessageBox.Show(
                    $"Withdrawal successful!\n\n" +
                    $"Transaction ID: {transaction.TransactionID}\n" +
                    $"Amount: ₱{transaction.Amount:N2}\n" +
                    $"Previous Balance: ₱{transaction.PreviousBalance:N2}\n" +
                    $"New Balance: ₱{transaction.NewBalance:N2}\n" +
                    $"Date: {transaction.TransactionDate:g}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                GenerateReceipt(
                    _selectedCustomer.FullName,
                    accountNumber,
                    amount,
                    transaction.NewBalance,
                    transaction.TransactionID,
                    "WITHDRAWAL");

                // refresh UI
                DisplayCustomerInfo(_selectedCustomer);
                txtWithdrawAmount.Text = "Amount";
                txtWithdrawAmount.ForeColor = WinFormsColor.Gray;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing withdrawal: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // -----------------------
        // Receipt generation
        // -----------------------
        public void GenerateReceipt(string customerName, string accountNumber, decimal amount, decimal newBalance, int transactionId, string transactionType = "WITHDRAWAL")
        {
            try
            {
                string cleanCustomerName = System.Text.RegularExpressions.Regex.Replace(customerName ?? "Customer", @"[^a-zA-Z0-9_]", "_");
                string cleanAccountNumber = System.Text.RegularExpressions.Regex.Replace(accountNumber ?? "Account", @"[^a-zA-Z0-9_]", "_");
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"Receipt_{cleanCustomerName}_{cleanAccountNumber}_{timestamp}.pdf";

                string folder = @"D:\Programming\VaultLinkBankSystem\Transaction_Receipts\Withdraws\";

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string filePath = IOPath.Combine(folder, fileName);

                using (PdfWriter writer = new PdfWriter(filePath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document doc = new Document(pdf, PageSize.A4))
                {
                    doc.SetMargins(50, 50, 50, 50);

                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    // Header
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

                    // Transaction Information
                    doc.Add(new Paragraph("TRANSACTION INFORMATION")
                        .SetFont(boldFont).SetFontSize(12).SetMarginBottom(10));

                    doc.Add(new Paragraph($"Transaction ID: {transactionId}")
                        .SetFont(regularFont).SetFontSize(11));
                    doc.Add(new Paragraph($"Transaction Type: {transactionType}")
                        .SetFont(regularFont).SetFontSize(11));
                    doc.Add(new Paragraph($"Date & Time: {DateTime.Now:dddd, MMMM dd, yyyy hh:mm:ss tt}")
                        .SetFont(regularFont).SetFontSize(11).SetMarginBottom(15));

                    doc.Add(new LineSeparator(new SolidLine()).SetMarginBottom(15));

                    // Customer Information
                    doc.Add(new Paragraph("CUSTOMER INFORMATION")
                        .SetFont(boldFont).SetFontSize(12).SetMarginBottom(10));

                    doc.Add(new Paragraph($"Customer Name: {customerName}")
                        .SetFont(regularFont).SetFontSize(11));
                    doc.Add(new Paragraph($"Account Number: {accountNumber}")
                        .SetFont(regularFont).SetFontSize(11));

                    doc.Add(new LineSeparator(new SolidLine()).SetMarginBottom(15));

                    // Transaction Summary
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

                    // Footer
                    doc.Add(new Paragraph("Thank you for banking with VaultLink!")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFont(regularFont).SetFontSize(10).SetMarginBottom(5));

                    doc.Add(new Paragraph("This is an electronic receipt and does not require a signature.")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFont(regularFont).SetFontSize(8)
                        .SetFontColor(PdfColor.GRAY));
                }

                DialogResult result = MessageBox.Show($"Receipt generated successfully!\n\nSaved at:\n{filePath}\n\nDo you want to open it now?", "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true }); }
                    catch (Exception ex) { MessageBox.Show("Could not open the PDF viewer automatically: " + ex.Message); }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating receipt: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // -----------------------
        // Textbox placeholder & numeric formatting handlers
        // -----------------------
        private void tbxSearchCustomer_Click(object sender, EventArgs e)
        {
            if (tbxSearchCustomer.Text == "Search Customer") tbxSearchCustomer.Clear();
        }

        private void tbxSearchCustomer_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxSearchCustomer.Text)) tbxSearchCustomer.Text = "Search Customer";
        }

        private void txtWithdrawAmount_Click(object sender, EventArgs e)
        {
            if (txtWithdrawAmount.Text == "Amount" || txtWithdrawAmount.Text == "Account is closed")
            {
                txtWithdrawAmount.Clear();
                txtWithdrawAmount.ForeColor = WinFormsColor.Black;
            }
        }

        private void txtWithdrawAmount_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtWithdrawAmount.Text))
            {
                txtWithdrawAmount.Text = "Amount";
                txtWithdrawAmount.ForeColor = WinFormsColor.Gray;
                return;
            }

            // format if valid numeric
            string raw = txtWithdrawAmount.Text.Replace(",", "").Trim();
            if (decimal.TryParse(raw, out decimal val))
            {
                txtWithdrawAmount.Text = val.ToString("N2");
                txtWithdrawAmount.ForeColor = WinFormsColor.Black;
            }
            else
            {
                txtWithdrawAmount.ForeColor = WinFormsColor.FromArgb(160, 0, 0);
            }
        }

        private void txtWithdrawAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // allow digits, decimal point and control
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '.' && txtWithdrawAmount.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        

        // -----------------------
        // Utilities
        // -----------------------
        private void ClearCustomerInfo()
        {
            cbxSelectAccount.Items.Clear();
            lblCustomerID.Text = "----------";
            lblName.Text = "----------";
            lblTotalBalance.Text = "--------";
            lblCurrentBalance.Text = "N/A";
            pbCustomerPicture.Image = null;
            _selectedCustomer = null;
            _customerAccounts = new List<Account>();
            txtWithdrawAmount.Text = "Amount";
            txtWithdrawAmount.ForeColor = WinFormsColor.Gray;
        }

        private void ClearForm()
        {
            tbxSearchCustomer.Text = "Search Customer";
            txtWithdrawAmount.Text = "Amount";
            txtWithdrawAmount.ForeColor = WinFormsColor.Gray;
            ClearCustomerInfo();
        }


        // Put this inside your UC_Withdraw class

        /// <summary>
        /// Universal formatter for Guna2 amount textboxes. Unsubscribes the provided EventHandler
        /// while mutating the text to avoid recursion, then re-subscribes it.
        /// limitCheck: optional function that receives the parsed amount and returns
        /// a tuple: (isValid, correctedValue, message). If isValid == false, correctedValue
        /// will be applied to the textbox and message will be shown to the user.
        /// </summary>
        private void FormatAmountTextbox(
            Guna.UI2.WinForms.Guna2TextBox txtBox,
            EventHandler textChangedHandler,
            Func<decimal, (bool isValid, decimal correctedValue, string message)> limitCheck = null
        )
        {
            // Unsubscribe caller handler to avoid recursion while we change Text
            txtBox.TextChanged -= textChangedHandler;

            try
            {
                string current = txtBox.Text ?? "";
                int cursor = txtBox.SelectionStart;

                // Placeholder guard
                if (current == "Amount")
                {
                    txtBox.TextChanged += textChangedHandler;
                    return;
                }

                // remove thousands separators and whitespace
                string raw = current.Replace(",", "").Trim();

                // empty -> keep empty
                if (string.IsNullOrEmpty(raw))
                {
                    txtBox.Text = "";
                    txtBox.SelectionStart = 0;
                    txtBox.TextChanged += textChangedHandler;
                    return;
                }

                // allow digits and just one dot
                bool hasDot = false;
                var filteredChars = new List<char>();
                foreach (char c in raw)
                {
                    if (char.IsDigit(c))
                        filteredChars.Add(c);
                    else if (c == '.' && !hasDot)
                    {
                        hasDot = true;
                        filteredChars.Add(c);
                    }
                }

                string filtered = new string(filteredChars.ToArray());

                // disallow leading dot
                if (filtered.StartsWith("."))
                    filtered = filtered.TrimStart('.');

                if (string.IsNullOrEmpty(filtered))
                {
                    txtBox.Text = "";
                    txtBox.SelectionStart = 0;
                    txtBox.TextChanged += textChangedHandler;
                    return;
                }

                // limit to 2 decimal places
                if (filtered.Contains("."))
                {
                    int dotIndex = filtered.IndexOf('.');
                    string whole = filtered.Substring(0, dotIndex);
                    string dec = filtered.Substring(dotIndex + 1);
                    if (dec.Length > 2)
                        dec = dec.Substring(0, 2);
                    filtered = whole + "." + dec;
                }

                if (!decimal.TryParse(filtered, out decimal amount))
                {
                    // if parse fails, restore subscriptions and exit
                    txtBox.TextChanged += textChangedHandler;
                    return;
                }

                // Optional limit check (withdraw / deposit / transfer logic)
                if (limitCheck != null)
                {
                    var check = limitCheck(amount);
                    if (!check.isValid)
                    {
                        // Show message and set corrected value (formatted)
                        if (!string.IsNullOrEmpty(check.message))
                            MessageBox.Show(check.message, "Amount Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // apply corrected value
                        txtBox.Text = check.correctedValue.ToString("N2").TrimEnd('0').TrimEnd('.');
                        txtBox.SelectionStart = txtBox.Text.Length;
                        txtBox.TextChanged += textChangedHandler;
                        return;
                    }
                }

                // Format with thousands separators while keeping decimals
                string formatted;
                if (filtered.Contains("."))
                {
                    int dotIndex = filtered.IndexOf('.');
                    string whole = filtered.Substring(0, dotIndex);
                    string dec = filtered.Substring(dotIndex + 1);

                    if (string.IsNullOrEmpty(whole))
                        whole = "0";

                    long wholeNum = long.Parse(whole);
                    formatted = string.Format("{0:N0}", wholeNum) + "." + dec;
                }
                else
                {
                    formatted = string.Format("{0:N0}", long.Parse(filtered));
                }

                // Reposition cursor reasonably
                int newCursor = formatted.Length - (current.Length - cursor);
                if (newCursor < 0) newCursor = 0;
                if (newCursor > formatted.Length) newCursor = formatted.Length;

                txtBox.Text = formatted;
                txtBox.SelectionStart = newCursor;
            }
            finally
            {
                // Always re-subscribe
                txtBox.TextChanged += textChangedHandler;
            }
        }

        /// <summary>
        /// Example withdraw textbox handler — replace your current handler with this.
        /// It uses FormatAmountTextbox and enforces withdraw rules:
        /// - selected account must exist
        /// - account must be open
        /// - amount must not exceed balance
        /// - optional minimum remaining balance (example: ₱100)
        /// </summary>
        private void txtWithdrawAmount_TextChanged(object sender, EventArgs e)
        {
            // textChangedHandler reference to pass to FormatAmountTextbox
            EventHandler handler = txtWithdrawAmount_TextChanged;

            FormatAmountTextbox(
                txtWithdrawAmount,
                handler,
                amount =>
                {
                    // If no selected account, allow typing but don't enforce limits
                    if (_customerAccounts == null || cbxSelectAccount.SelectedItem == null)
                        return (true, amount, "");

                    dynamic selectedItem = cbxSelectAccount.SelectedItem;
                    int accountId = selectedItem.AccountID;
                    Account acc = _customerAccounts.FirstOrDefault(a => a.AccountID == accountId);

                    if (acc == null)
                        return (true, amount, "");

                    // Account closed
                    if (acc.Status == "Closed")
                        return (false, 0m, "This account is CLOSED. Withdrawals are not allowed.");

                    // Exceeding available balance
                    if (amount > acc.Balance)
                    {
                        decimal corrected = acc.Balance;
                        return (false, corrected, $"Withdrawal cannot exceed current balance ₱{acc.Balance:N2}.");
                    }

                    // Example: require minimum remaining balance (change or remove as needed)
                    decimal minRemaining = 500m;
                    if (acc.Balance - amount < minRemaining)
                    {
                        decimal corrected = Math.Max(0, acc.Balance - minRemaining);
                        return (false, corrected, $"This withdrawal would reduce balance below the required minimum of ₱{minRemaining:N2}.");
                    }

                    return (true, amount, "");
                }
            );
        }



        private void ResetPlaceholders()
        {
            if (string.IsNullOrWhiteSpace(tbxSearchCustomer.Text)) tbxSearchCustomer.Text = "Search Customer";
            if (string.IsNullOrWhiteSpace(txtWithdrawAmount.Text)) { txtWithdrawAmount.Text = "Amount"; txtWithdrawAmount.ForeColor = WinFormsColor.Gray; }
        }
    }
}
