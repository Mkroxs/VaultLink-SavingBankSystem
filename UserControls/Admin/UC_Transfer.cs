using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VaultLinkBankSystem.Helpers;
using IOPath = System.IO.Path;
using PdfColor = iText.Kernel.Colors.ColorConstants;
using WinFormsColor = System.Drawing.Color;
using WinFormsFont = System.Drawing.Font;
using WinFormsFontStyle = System.Drawing.FontStyle;
using WinFormsImage = System.Drawing.Image;
using WinFormsPadding = System.Windows.Forms.Padding;
using WinFormsPoint = System.Drawing.Point;
using WinFormsSize = System.Drawing.Size;

namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_Transfer : UserControl
    {
        private TransactionRepository _transactionRepo;
        private AccountRepository _accountRepo;
        private CustomerRepository _customerRepo;
        private Customers _selectedCustomer;
        private List<Account> _customerAccounts;

        public UC_Transfer()
        {
            InitializeComponent();

            _transactionRepo = new TransactionRepository();
            _accountRepo = new AccountRepository();
            _customerRepo = new CustomerRepository();


            cbxSelectAccount.DrawMode = DrawMode.OwnerDrawFixed;
            cbxSelectAccount.DrawItem += cbxSelectAccount_DrawItem;
        }

        private void UC_Transfer_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
            ClearForm();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearchAccountNumber.Text.Trim();

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

        private void ShowCustomerSelectionDialog(List<Customers> customers)
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
                Customers selectedCustomer = dgv.SelectedRows[0].DataBoundItem as Customers;
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

        private void DisplayCustomerInfo(Customers customer)
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

        public void PopulateAccountDropdown()
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
                    AccountStatus = acc.Status,      // REQUIRED ✔
                    DisplayText = $"{acc.AccountNumber} - {acc.AccountType} ({acc.Balance:C2})"
                });
            }

            if (cbxSelectAccount.Items.Count > 0)
                cbxSelectAccount.SelectedIndex = 0;
        }


        private void btnConfirm_Click(object sender, EventArgs e)
        {
            
        }

        public void GenerateTransferReceipt(
            string senderName,
            string senderAccountNumber,
            string recipientName,
            string recipientAccountNumber,
            decimal amount,
            decimal senderNewBalance,
            decimal recipientNewBalance,
            int transactionId)
        {
            try
            {
                string cleanSenderName = System.Text.RegularExpressions.Regex.Replace(senderName, @"[^a-zA-Z0-9_]", "_");
                string cleanRecipientName = System.Text.RegularExpressions.Regex.Replace(recipientName, @"[^a-zA-Z0-9_]", "_");
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"Transfer_{cleanSenderName}_to_{cleanRecipientName}_{timestamp}.pdf";

                string folder = @"D:\Programming\VaultLinkBankSystem\Transaction_Receipts\Withdraws\s";

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string filePath = IOPath.Combine(folder, fileName);

                using (PdfWriter writer = new PdfWriter(filePath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document doc = new Document(pdf, PageSize.A4))
                {
                    doc.SetMargins(50, 50, 50, 50);

                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    // Header
                    doc.Add(new Paragraph("TRANSFER RECEIPT")
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
                    doc.Add(new Paragraph($"Transaction Type: TRANSFER")
                        .SetFont(regularFont).SetFontSize(11));
                    doc.Add(new Paragraph($"Date & Time: {DateTime.Now:dddd, MMMM dd, yyyy hh:mm:ss tt}")
                        .SetFont(regularFont).SetFontSize(11).SetMarginBottom(15));

                    doc.Add(new LineSeparator(new SolidLine()).SetMarginBottom(15));

                    // Sender Information
                    doc.Add(new Paragraph("SENDER INFORMATION")
                        .SetFont(boldFont).SetFontSize(12).SetMarginBottom(10));

                    doc.Add(new Paragraph($"Name: {senderName}")
                        .SetFont(regularFont).SetFontSize(11));
                    doc.Add(new Paragraph($"Account Number: {senderAccountNumber}")
                        .SetFont(regularFont).SetFontSize(11));

                    decimal senderPreviousBalance = senderNewBalance + amount;
                    doc.Add(new Paragraph($"Previous Balance: {senderPreviousBalance:C2}")
                        .SetFont(regularFont).SetFontSize(11));
                    doc.Add(new Paragraph($"New Balance: {senderNewBalance:C2}")
                        .SetFont(boldFont).SetFontSize(11).SetMarginBottom(15));

                    doc.Add(new LineSeparator(new SolidLine()).SetMarginBottom(15));

                    // Recipient Information
                    doc.Add(new Paragraph("RECIPIENT INFORMATION")
                        .SetFont(boldFont).SetFontSize(12).SetMarginBottom(10));

                    doc.Add(new Paragraph($"Name: {recipientName}")
                        .SetFont(regularFont).SetFontSize(11));
                    doc.Add(new Paragraph($"Account Number: {recipientAccountNumber}")
                        .SetFont(regularFont).SetFontSize(11));

                    decimal recipientPreviousBalance = recipientNewBalance - amount;
                    doc.Add(new Paragraph($"Previous Balance: {recipientPreviousBalance:C2}")
                        .SetFont(regularFont).SetFontSize(11));
                    doc.Add(new Paragraph($"New Balance: {recipientNewBalance:C2}")
                        .SetFont(boldFont).SetFontSize(11).SetMarginBottom(15));

                    doc.Add(new LineSeparator(new SolidLine()).SetMarginBottom(15));

                    // Transfer Summary
                    doc.Add(new Paragraph("TRANSFER SUMMARY")
                        .SetFont(boldFont).SetFontSize(14).SetMarginBottom(10)
                        .SetTextAlignment(TextAlignment.CENTER));

                    doc.Add(new Paragraph($"Transfer Amount: {amount:C2}")
                        .SetFont(boldFont).SetFontSize(16)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(20));

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

                DialogResult result = MessageBox.Show(
                    $"Transfer receipt generated successfully!\n\nSaved at:\n{filePath}\n\nDo you want to open it now?",
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
            txtSearchAccountNumber.Clear();
            txtRecipientNumber.Clear();
            txtTransferAmount.Clear();
            ClearCustomerInfo();
        }

        private void guna2Panel8_Paint(object sender, EventArgs e)
        {
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomer == null)
                {
                    MessageBox.Show("Please search for a sender customer first.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (cbxSelectAccount.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a sender account.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtRecipientNumber.Text))
                {
                    MessageBox.Show("Please enter recipient account number.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtTransferAmount.Text))
                {
                    MessageBox.Show("Please enter transfer amount.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtTransferAmount.Text, out decimal amount))
                {
                    MessageBox.Show("Please enter a valid amount.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (amount <= 0)
                {
                    MessageBox.Show("Amount must be greater than zero.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                dynamic selectedItem = cbxSelectAccount.SelectedItem;
                int senderAccountId = selectedItem.AccountID;
                string senderAccountNumber = selectedItem.AccountNumber;

                string recipientAccountNumber = txtRecipientNumber.Text.Trim();

                var recipientAccount = _accountRepo.GetAccountByAccountNumber(recipientAccountNumber);

                var senderAccount = _accountRepo.GetAccountByAccountNumber(senderAccountNumber);

                if (senderAccount.Status == "Closed")
                {
                    MessageBox.Show("Sender account is CLOSED. Transfers are not allowed.",
                        "Account Closed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }


               




                if (recipientAccount == null)
                {
                    MessageBox.Show("Recipient account not found.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (recipientAccount.Status == "Closed")
                {
                    MessageBox.Show("Recipient account is CLOSED. You cannot transfer to this account.",
                        "Account Closed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }




                if (senderAccountId == recipientAccount.AccountID)
                {
                    MessageBox.Show("Cannot transfer to the same account.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var recipientCustomer = _customerRepo.GetCustomerById(recipientAccount.CustomerID);

                DialogResult result = MessageBox.Show(
                    $"Transfer Details:\n\n" +
                    $"FROM: {_selectedCustomer.FullName}\n" +
                    $"Account: {senderAccountNumber}\n\n" +
                    $"TO: {recipientCustomer.FullName}\n" +
                    $"Account: {recipientAccount.AccountNumber}\n\n" +
                    $"Amount: {amount:C2}\n\n" +
                    $"Are you sure you want to proceed with this transfer?",
                    "Confirm Transfer",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var (senderTransaction, recipientTransaction) = _transactionRepo.Transfer(
                        senderAccountId,
                        recipientAccount.AccountID,
                        amount,
                        $"Transfer to {recipientAccount.AccountNumber}");

                    MessageBox.Show(
                        $"Transfer successful!\n\n" +
                        $"Sender Transaction ID: {senderTransaction.TransactionID}\n" +
                        $"Amount: {amount:C2}\n" +
                        $"Sender New Balance: {senderTransaction.NewBalance:C2}\n\n" +
                        $"Recipient Transaction ID: {recipientTransaction.TransactionID}\n" +
                        $"Recipient New Balance: {recipientTransaction.NewBalance:C2}\n" +
                        $"Date: {senderTransaction.TransactionDate:g}",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Generate transfer receipt
                    GenerateTransferReceipt(
                        _selectedCustomer.FullName,
                        senderAccountNumber,
                        recipientCustomer.FullName,
                        recipientAccount.AccountNumber,
                        amount,
                        senderTransaction.NewBalance,
                        recipientTransaction.NewBalance,
                        senderTransaction.TransactionID);

                    DisplayCustomerInfo(_selectedCustomer);

                    txtRecipientNumber.Clear();
                    txtTransferAmount.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing transfer: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void cbxSelectAccount_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            dynamic item = cbxSelectAccount.Items[e.Index];

            string text = item.DisplayText;
            string status = item.AccountStatus;

            e.DrawBackground();

            // Choose color based on status
            Color color = status == "Closed" ? Color.Red : Color.Green;

            using (Brush brush = new SolidBrush(color))
            {
                e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void cbxSelectAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSelectAccount.SelectedItem == null) return;

            dynamic selectedItem = cbxSelectAccount.SelectedItem;
            int accountId = selectedItem.AccountID;

            var account = _customerAccounts.FirstOrDefault(a => a.AccountID == accountId);

            if (account == null) return;

            lblCurrentBalance.Text = account.Balance.ToString("C2");

            if (account.Status == "Closed")
            {
                // RED UI for closed accounts
                cbxSelectAccount.ForeColor = Color.Red;
                txtTransferAmount.Enabled = false;
                btnConfirmTransfer.Enabled = false;
                txtRecipientNumber.Enabled = false;
            }
            else
            {
                // GREEN UI for active accounts
                cbxSelectAccount.ForeColor = Color.Green;
                txtTransferAmount.Enabled = true;
                btnConfirmTransfer.Enabled = true;
            }
        }
    }
}