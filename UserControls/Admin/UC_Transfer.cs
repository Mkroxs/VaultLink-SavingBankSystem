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
        private decimal _senderBalance = 0;

        private TransactionRepository _transactionRepo;
        private AccountRepository _accountRepo;
        private CustomerRepository _customerRepo;

        private VaultLinkBankSystem.Customer _selectedCustomer;
        private List<Account> _customerAccounts;

        public UC_Transfer()
        {
            InitializeComponent();
            txtTransferAmount.TextChanged += txtTransferAmount_TextChanged;

            _transactionRepo = new TransactionRepository();
            _accountRepo = new AccountRepository();
            _customerRepo = new CustomerRepository();

            // ComboBox draw mode
            cbxSelectAccount.DrawMode = DrawMode.OwnerDrawFixed;
            cbxSelectAccount.DrawItem += cbxSelectAccount_DrawItem;

            // manually register handlers (same as designer)
            btnSearch.Click += btnSearch_Click;
            btnConfirmTransfer.Click += btnConfirmTransfer_Click;
            cbxSelectAccount.SelectedIndexChanged += cbxSelectAccount_SelectedIndexChanged;

            txtSearchCustomerName.Click += txtSearchCustomerName_Click;
            txtSearchCustomerName.Leave += txtSearchCustomerName_Leave;

            txtRecipientNumber.Click += txtRecipientNumber_Click;
            txtRecipientNumber.Leave += txtRecipientNumber_Leave;

            txtTransferAmount.Click += txtTransferAmount_Click;
        }

        private void UC_Transfer_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
        }

        // ---------------------------------------------------
        // SEARCH CUSTOMER
        // ---------------------------------------------------
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearchCustomerName.Text.Trim();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    MessageBox.Show("Please enter a customer name or code.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var all = _customerRepo.GetAllCustomers();
                var found = all.Where(c =>
                    c.IsKYCVerified &&
                    (c.CustomerCode.ToLower().Contains(searchTerm.ToLower())
                    || c.FullName.ToLower().Contains(searchTerm.ToLower()))
                ).ToList();

                if (found.Count == 0)
                {
                    MessageBox.Show("No verified customer found.",
                        "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Error searching customer: " + ex.Message);
            }
        }

        private void ShowCustomerSelectionDialog(List<VaultLinkBankSystem.Customer> customers)
        {
            Form frm = new Form
            {
                Text = "Select Customer",
                Size = new Size(700, 450),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            DataGridView dgv = new DataGridView
            {
                DataSource = customers,
                Location = new WinFormsPoint(10, 10),
                Size = new Size(665, 350),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            SetupGridStyle(dgv);

            dgv.DataBindingComplete += (s, e) =>
            {
                string[] allowed = { "CustomerID", "CustomerCode", "FullName", "Email", "Phone" };
                foreach (DataGridViewColumn col in dgv.Columns)
                    col.Visible = allowed.Contains(col.Name);
            };

            Button btnSelect = new Button
            {
                Text = "Select Customer",
                DialogResult = DialogResult.OK,
                Location = new WinFormsPoint(480, 365),
                Size = new Size(120, 35)
            };

            frm.Controls.Add(dgv);
            frm.Controls.Add(btnSelect);

            if (frm.ShowDialog() == DialogResult.OK && dgv.SelectedRows.Count > 0)
            {
                DisplayCustomerInfo(dgv.SelectedRows[0].DataBoundItem as VaultLinkBankSystem.Customer);
            }
        }

        private void SetupGridStyle(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 62, 84);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.RowHeadersVisible = false;
        }

        // ---------------------------------------------------
        // DISPLAY CUSTOMER DETAILS
        // ---------------------------------------------------
        private void DisplayCustomerInfo(VaultLinkBankSystem.Customer customer)
        {
            _selectedCustomer = customer;
            _customerAccounts = _accountRepo.GetAccountsByCustomerId(customer.CustomerID);

            if (_customerAccounts.Count == 0)
            {
                MessageBox.Show("This customer has no accounts.");
                ClearCustomerInfo();
                return;
            }

            lblCustomerID.Text = customer.CustomerID.ToString();
            lblName.Text = customer.FullName;
            lblTotalBalance.Text = "₱" + _customerAccounts.Sum(x => x.Balance).ToString("N2");

            if (!string.IsNullOrEmpty(customer.ImagePath) && File.Exists(customer.ImagePath))
                pbCustomerPicture.Image = WinFormsImage.FromFile(customer.ImagePath);
            else
                pbCustomerPicture.Image = null;

            PopulateAccountDropdown();
        }

        private void PopulateAccountDropdown()
        {
            cbxSelectAccount.Items.Clear();

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

            if (cbxSelectAccount.Items.Count > 0)
                cbxSelectAccount.SelectedIndex = 0;
        }

        // ---------------------------------------------------
        // ACCOUNT DROPDOWN COLOR + BALANCE
        // ---------------------------------------------------
        private void cbxSelectAccount_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            dynamic item = cbxSelectAccount.Items[e.Index];

            string text = item.DisplayText;
            string status = item.AccountStatus;

            e.DrawBackground();

            Color color = status == "Closed" ? Color.Red : Color.Green;

            using (Brush b = new SolidBrush(color))
                e.Graphics.DrawString(text, e.Font, b, e.Bounds);

            e.DrawFocusRectangle();
        }

        private void cbxSelectAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSelectAccount.SelectedItem == null) return;

            dynamic selected = cbxSelectAccount.SelectedItem;
            int id = selected.AccountID;

            var acc = _customerAccounts.FirstOrDefault(a => a.AccountID == id);

            lblCurrentBalance.Text = "₱" + acc.Balance.ToString("N2");

            _senderBalance = acc.Balance; // <-- IMPORTANT

            if (acc.Status == "Closed")
            {
                cbxSelectAccount.ForeColor = Color.Red;
                txtTransferAmount.Enabled = false;
                txtRecipientNumber.Enabled = false;
                btnConfirmTransfer.Enabled = false;
            }
            else
            {
                cbxSelectAccount.ForeColor = Color.Green;
                txtTransferAmount.Enabled = true;
                txtRecipientNumber.Enabled = true;
                btnConfirmTransfer.Enabled = true;
            }
        }

        // ============================================
        // FORMATTER WITH TRANSFER LIMIT CHECK
        // ============================================
        private void FormatTransferAmount(Guna.UI2.WinForms.Guna2TextBox textbox, EventHandler handler)
        {
            textbox.TextChanged -= handler;

            try
            {
                string text = textbox.Text ?? "";
                int cursor = textbox.SelectionStart;

                if (text == "Amount")
                {
                    textbox.TextChanged += handler;
                    return;
                }

                string raw = text.Replace(",", "").Trim();

                if (raw == "")
                {
                    textbox.Text = "";
                    textbox.SelectionStart = 0;
                    textbox.TextChanged += handler;
                    return;
                }

                bool hasDot = false;
                List<char> valid = new List<char>();

                foreach (char c in raw)
                {
                    if (char.IsDigit(c))
                        valid.Add(c);
                    else if (c == '.' && !hasDot)
                    {
                        hasDot = true;
                        valid.Add(c);
                    }
                }

                string filtered = new string(valid.ToArray());

                if (filtered.StartsWith("."))
                    filtered = filtered.TrimStart('.');

                if (filtered == "")
                {
                    textbox.Text = "";
                    textbox.SelectionStart = 0;
                    textbox.TextChanged += handler;
                    return;
                }

                // Limit decimal places
                if (filtered.Contains("."))
                {
                    int dot = filtered.IndexOf('.');
                    string whole = filtered.Substring(0, dot);
                    string dec = filtered.Substring(dot + 1);

                    if (dec.Length > 2)
                        dec = dec.Substring(0, 2);

                    filtered = whole + "." + dec;
                }

                // Safe parse
                if (!decimal.TryParse(filtered, out decimal amount))
                {
                    textbox.TextChanged += handler;
                    return;
                }

                // -----------------------------------------
                // LIMIT CHECK (NO OVERDRAFT)
                // -----------------------------------------
                if (amount > _senderBalance)
                {
                    MessageBox.Show(
                        $"Transfer amount exceeds available balance.\n" +
                        $"Maximum you can transfer: ₱{_senderBalance:N2}",
                        "Insufficient Balance",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    // Force textbox to the max allowed
                    filtered = _senderBalance.ToString("0.##");
                }

                // Formatting:
                string formatted = "";

                if (filtered.Contains("."))
                {
                    int dot = filtered.IndexOf('.');
                    string whole = filtered.Substring(0, dot);
                    string dec = filtered.Substring(dot + 1);

                    if (whole == "")
                        whole = "0";

                    long wholeNumber = long.Parse(whole);
                    formatted = string.Format("{0:N0}", wholeNumber) + "." + dec;
                }
                else
                {
                    formatted = string.Format("{0:N0}", long.Parse(filtered));
                }

                int newCursor = formatted.Length - (text.Length - cursor);
                if (newCursor < 0) newCursor = 0;
                if (newCursor > formatted.Length) newCursor = formatted.Length;

                textbox.Text = formatted;
                textbox.SelectionStart = newCursor;
            }
            finally
            {
                textbox.TextChanged += handler;
            }
        }

        private void txtTransferAmount_TextChanged(object sender, EventArgs e)
        {
            FormatTransferAmount(txtTransferAmount, txtTransferAmount_TextChanged);
        }



        // ---------------------------------------------------
        // CONFIRM TRANSFER
        // ---------------------------------------------------
        private void btnConfirmTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedCustomer == null)
                {
                    MessageBox.Show("Search a customer first.");
                    return;
                }

                if (cbxSelectAccount.SelectedIndex < 0)
                {
                    MessageBox.Show("Select a sender account.");
                    return;
                }

                if (!decimal.TryParse(txtTransferAmount.Text, out decimal amount))
                {
                    MessageBox.Show("Enter a valid amount.");
                    return;
                }

                dynamic selectedItem = cbxSelectAccount.SelectedItem;
                int senderAccId = selectedItem.AccountID;
                string senderAccNum = selectedItem.AccountNumber;

                string recipientAccountNumber = txtRecipientNumber.Text.Trim();
                var recipientAcc = _accountRepo.GetAccountByAccountNumber(recipientAccountNumber);
                var senderAcc = _accountRepo.GetAccountByAccountNumber(senderAccNum);

                if (recipientAcc == null)
                {
                    MessageBox.Show("Recipient account not found.");
                    return;
                }

                if (senderAccId == recipientAcc.AccountID)
                {
                    MessageBox.Show("Cannot transfer to same account.");
                    return;
                }

                var recCustomer = _customerRepo.GetCustomerById(recipientAcc.CustomerID);

                DialogResult confirm = MessageBox.Show(
                    $"FROM: {_selectedCustomer.FullName}\n" +
                    $"Account: {senderAccNum}\n\n" +
                    $"TO: {recCustomer.FullName}\n" +
                    $"Account: {recipientAcc.AccountNumber}\n\n" +
                    $"Amount: ₱{amount:N2}\n\n" +
                    $"Proceed?",
                    "Confirm Transfer",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes) return;

                var (senderTx, receiverTx) = _transactionRepo.Transfer(
                    senderAccId,
                    recipientAcc.AccountID,
                    amount,
                    $"Transfer to {recipientAcc.AccountNumber}");

                GenerateTransferReceipt(
                    _selectedCustomer.FullName,
                    senderAccNum,
                    recCustomer.FullName,
                    recipientAcc.AccountNumber,
                    amount,
                    senderTx.NewBalance,
                    receiverTx.NewBalance,
                    senderTx.TransactionID);

                DisplayCustomerInfo(_selectedCustomer);

                txtTransferAmount.Clear();
                txtRecipientNumber.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Transfer error:\n" + ex.Message);
            }
        }

        // ---------------------------------------------------
        // RECEIPT GENERATION
        // ---------------------------------------------------
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
                string folder = @"D:\Programming\VaultLinkBankSystem\Transaction_Receipts\Transfers";

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = $"Transfer_{senderName}_{recipientName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = IOPath.Combine(folder, fileName);

                using (PdfWriter writer = new PdfWriter(filePath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document doc = new Document(pdf, PageSize.A4))
                {
                    doc.SetMargins(40, 40, 40, 40);

                    PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont reg = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    doc.Add(new Paragraph("TRANSFER RECEIPT")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20).SetFont(bold));

                    doc.Add(new Paragraph("VaultLink Bank")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(14));

                    doc.Add(new LineSeparator(new SolidLine()).SetMarginBottom(10));

                    doc.Add(new Paragraph($"Transaction ID: {transactionId}"));
                    doc.Add(new Paragraph($"Date: {DateTime.Now:g}"));

                    doc.Add(new LineSeparator(new SolidLine()));

                    doc.Add(new Paragraph("SENDER:").SetFont(bold));
                    doc.Add(new Paragraph(senderName));
                    doc.Add(new Paragraph(senderAccountNumber));
                    doc.Add(new Paragraph($"New Balance: ₱{senderNewBalance:N2}"));

                    doc.Add(new LineSeparator(new SolidLine()));

                    doc.Add(new Paragraph("RECIPIENT:").SetFont(bold));
                    doc.Add(new Paragraph(recipientName));
                    doc.Add(new Paragraph(recipientAccountNumber));
                    doc.Add(new Paragraph($"New Balance: ₱{recipientNewBalance:N2}"));

                    doc.Add(new LineSeparator(new SolidLine()));

                    doc.Add(new Paragraph($"AMOUNT TRANSFERRED: ₱{amount:N2}")
                        .SetFontSize(16)
                        .SetTextAlignment(TextAlignment.CENTER));
                }

                MessageBox.Show($"Receipt saved:\n{filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Receipt error:\n" + ex.Message);
            }
        }

        // ---------------------------------------------------
        // TEXTBOX EVENTS
        // ---------------------------------------------------
        private void txtSearchCustomerName_Click(object sender, EventArgs e)
        {
            if (txtSearchCustomerName.Text == "Search Customer")
                txtSearchCustomerName.Clear();
        }


        private void txtSearchCustomerName_Leave(object sender, EventArgs e)
        {
            if (txtSearchCustomerName.Text.Trim() == "")
                txtSearchCustomerName.Text = "Search Customer";
        }

        private void txtRecipientNumber_Click(object sender, EventArgs e)
        {
            txtRecipientNumber.Clear();
        }

        private void txtRecipientNumber_Leave(object sender, EventArgs e)
        {
            if (txtRecipientNumber.Text.Trim() == "")
                txtRecipientNumber.Text = "Recipient Account Number";
        }

        private void txtTransferAmount_Click(object sender, EventArgs e)
        {
            txtTransferAmount.Clear();
        }

        private void txtTransferAmount_Leave(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtTransferAmount.Text, out decimal value))
            {
                txtTransferAmount.Text = value.ToString("N2");  // format as 1,234.00
                txtTransferAmount.FillColor = Color.White;
            }
            else
            {
                txtTransferAmount.FillColor = Color.FromArgb(255, 200, 200); // highlight red
            }
        }


        private void txtTransferAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '.' && txtTransferAmount.Text.Contains("."))
            {
                e.Handled = true;
            }
        }


        // ---------------------------------------------------
        private void ClearCustomerInfo()
        {
            lblCustomerID.Text = "---------";
            lblName.Text = "---------";
            lblTotalBalance.Text = "---------";
            pbCustomerPicture.Image = null;

            cbxSelectAccount.Items.Clear();

            _selectedCustomer = null;
            _customerAccounts = null;

        }

        private void txtSearchCustomerName_TextChanged(object sender, EventArgs e)
        {
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            btnConfirmTransfer_Click(sender, e);
        }


    }
}
