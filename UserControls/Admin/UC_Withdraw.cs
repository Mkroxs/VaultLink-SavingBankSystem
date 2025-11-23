using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Xml.Linq;
using VaultLinkBankSystem.Helpers;
using Path = System.IO.Path;

using Paragraph = iText.Layout.Element.Paragraph;
using LineSeparator = iText.Layout.Element.LineSeparator;
using SolidLine = iText.Kernel.Pdf.Canvas.Draw.SolidLine;


using iText.Kernel.Colors;


namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_Withdraw : UserControl
    {
        private TransactionRepository _transactionRepo;
        private AccountRepository _accountRepo;
        private CustomerRepository _customerRepo;

        private Customer _selectedCustomer;
        private List<Account> _customerAccounts;

        public UC_Withdraw()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();


            _transactionRepo = new TransactionRepository();
            _accountRepo = new AccountRepository();
            _customerRepo = new CustomerRepository();
        }



        private void UC_Withdraw_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
            ClearForm();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = tbxSearchAccountNumber.Text.Trim(); // tbxSearchAccountNumber is now used for Customer Search

                if (string.IsNullOrEmpty(searchText))
                {
                    MessageBox.Show("Please enter a customer name, code, or phone number to search.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // *** MODIFIED LOGIC START ***
                // Search for customers by FullName, Email, Phone, or CustomerCode
                List<Customer> foundCustomers = _customerRepo.SearchCustomers(searchText);

                if (foundCustomers == null || foundCustomers.Count == 0)
                {
                    MessageBox.Show("Customer not found.",
                        "Search Result",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    ClearForm();
                    return;
                }

                if (foundCustomers.Count > 1)
                {
                    // Handle multiple results (e.g., prompt user to select one)
                    // For now, we will select the first customer found.
                    MessageBox.Show($"Found {foundCustomers.Count} customers. Selecting the first result. You should implement a customer selection screen.",
                        "Multiple Results",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    _selectedCustomer = foundCustomers.First();
                }
                else
                {
                    // Exactly one customer found
                    _selectedCustomer = foundCustomers.First();
                }
                // *** MODIFIED LOGIC END ***

                // Get all accounts for this customer
                _customerAccounts = _accountRepo.GetAccountsByCustomerId(_selectedCustomer.CustomerID);

                // Display customer information
                DisplayCustomerInfo();

                // Populate account dropdown
                PopulateAccountDropdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching for customer: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

        }


        private void DisplayCustomerInfo()
        {
            if (_selectedCustomer != null)
            {
                lblCustomerID.Text = _selectedCustomer.CustomerCode.ToString();
                lblName.Text = $"{_selectedCustomer.FullName}";

                // Calculate total balance across all accounts
                decimal totalBalance = _customerAccounts.Sum(a => a.Balance);
                lblTotalBalance.Text = totalBalance.ToString("C2");

                // Load customer picture if available
                if (!string.IsNullOrEmpty(_selectedCustomer.ImagePath))
                {
                    // Check if the file exists at the given path to prevent exceptions
                    if (System.IO.File.Exists(_selectedCustomer.ImagePath))
                    {
                        try
                        {
                            // Load the image directly from the file path
                            pbCustomerPicture.Image = System.Drawing.Image.FromFile(_selectedCustomer.ImagePath);
                        }
                        catch (System.Exception ex)
                        {

                            System.Diagnostics.Debug.WriteLine($"Error loading image from file: {ex.Message}");
                            pbCustomerPicture.Image = null; // Set to null on error
                        }
                    }
                    else
                    {
                        pbCustomerPicture.Image = null;
                    }
                }
                else
                {
                    pbCustomerPicture.Image = null;
                }
            }
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
                    DisplayText = $"{account.AccountNumber} - {account.AccountType} ({account.Balance:C2})"
                });
            }

            if (cbxSelectAccount.Items.Count > 0)
            {
                cbxSelectAccount.SelectedIndex = 0;
            }
        }



        private void btnWthdraw_Click(object sender, EventArgs e)
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

                if (string.IsNullOrEmpty(tbxAmount.Text))
                {
                    MessageBox.Show("Please enter an amount.",
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Parse amount
                if (!decimal.TryParse(tbxAmount.Text, out decimal amount))
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
                int accountId = selectedItem.AccountID;
                string accountDisplay = selectedItem.DisplayText;
                // Confirm withdrawal
                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to withdraw {amount:C2} from this account?",
                    "Confirm Withdrawal",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Process withdrawal
                    Transaction transaction = _transactionRepo.Withdraw(accountId, amount, "Withdraw");

                    MessageBox.Show(
                        $"Withdrawal successful!\n\n" +
                        $"Transaction ID: {transaction.TransactionID}\n" +
                        $"Amount: {transaction.Amount:C2}\n" +
                        $"Previous Balance: {transaction.PreviousBalance:C2}\n" +
                        $"New Balance: {transaction.NewBalance:C2}\n" +
                        $"Date: {transaction.TransactionDate:g}",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);




                    GenerateReceipt(_selectedCustomer.FullName, accountDisplay, amount, transaction.NewBalance, transaction.TransactionID);

                    // Refresh the display
                    btnSearch_Click(sender, e); // Re-search to update balances

                    // Clear amount field
                    tbxAmount.Clear();
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




        private void ClearForm()
        {
            tbxSearchAccountNumber.Clear();
            tbxAmount.Clear();
            cbxSelectAccount.Items.Clear();
            lblCustomerID.Text = "----------";
            lblName.Text = "----------";
            lblTotalBalance.Text = "--------";
            lblCurrentBalance.Text = "--------";
            pbCustomerPicture.Image = null;
            _selectedCustomer = null;
            _customerAccounts = null;
        }




        public void GenerateReceipt(string customerName, string accountNumber, decimal amount, decimal newBalance, int transactionId, string transactionType = "WITHDRAWAL")
        {
            try
            {
                // 1. SETUP FILE PATHS (Dynamic, not hardcoded)
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

                // 2. GENERATE PDF
                // The 'using' block automatically closes the file, writer, and document when done.
                using (PdfWriter writer = new PdfWriter(filePath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document doc = new Document(pdf, PageSize.A4))
                {
                    // Set margins
                    doc.SetMargins(50, 50, 50, 50);

                    // Create fonts
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    // Add Content using your helper methods
                    AddHeader(doc, boldFont, regularFont);
                    AddTransactionInfo(doc, boldFont, regularFont, transactionId, transactionType);
                    AddCustomerInfo(doc, boldFont, regularFont, customerName, accountNumber);
                    AddTransactionSummary(doc, boldFont, regularFont, amount, newBalance, transactionType);
                    AddFooter(doc, regularFont);
                }

                // 3. SUCCESS & OPEN FILE
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
                MessageBox.Show($"File access error (is the file open?): {ioEx.Message}", "I/O Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- YOUR HELPER METHODS (Unchanged, just ensuring they are here) ---

        private void AddHeader(Document doc, PdfFont boldFont, PdfFont regularFont)
        {
            doc.Add(new Paragraph("TRANSACTION RECEIPT")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(20)
                .SetFont(boldFont)
                .SetMarginBottom(5));

            doc.Add(new Paragraph("VaultLink Bank")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(14)
                .SetFont(regularFont)
                .SetMarginBottom(20));

            doc.Add(new LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.SolidLine()).SetMarginBottom(15));
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

            doc.Add(new LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.SolidLine()).SetMarginBottom(15));
        }

        private void AddTransactionSummary(Document doc, PdfFont boldFont, PdfFont regularFont, decimal amount, decimal newBalance, string transactionType)
        {
            doc.Add(new Paragraph("TRANSACTION SUMMARY")
                .SetFont(boldFont).SetFontSize(12).SetMarginBottom(10));

            // Logic Check: If withdrawal, we had MORE money before. If Deposit, we had LESS.
            decimal previousBalance = transactionType.ToUpper() == "WITHDRAWAL"
                ? newBalance + amount
                : newBalance - amount;

            doc.Add(new Paragraph($"Transaction Amount: {amount:C2}").SetFont(boldFont).SetFontSize(13));
            doc.Add(new Paragraph($"Previous Balance: {previousBalance:C2}").SetFont(regularFont).SetFontSize(11));
            doc.Add(new Paragraph($"New Balance: {newBalance:C2}")
                .SetFont(boldFont).SetFontSize(13).SetMarginBottom(20));

            doc.Add(new LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.SolidLine()).SetMarginBottom(15));
        }

        private void AddFooter(Document doc, PdfFont regularFont)
        {
            doc.Add(new Paragraph("Thank you for banking with VaultLink!")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFont(regularFont).SetFontSize(10).SetMarginBottom(5));

            doc.Add(new Paragraph("This is an electronic receipt and does not require a signature.")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFont(regularFont).SetFontSize(8)
                .SetFontColor(ColorConstants.GRAY));
        }







        private void cbxSelectAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSelectAccount.SelectedItem != null)
            {
                try
                {
                    // We need to find the Account object corresponding to the selected item.
                    // A safer way is to extract the AccountID and find the original object.
                    dynamic selectedItem = cbxSelectAccount.SelectedItem;
                    int accountId = selectedItem.AccountID;

                    // Find the full Account object from the cached list
                    Account selectedAccount = _customerAccounts.FirstOrDefault(a => a.AccountID == accountId);

                    if (selectedAccount != null)
                    {
                        lblCurrentBalance.Text = selectedAccount.Balance.ToString("C2");
                        tbxAmount.Enabled = true;
                        btnWthdraw.Enabled = true;
                    }
                }
                catch
                {
                    // If casting fails or list is empty
                    lblCurrentBalance.Text = "N/A";
                    tbxAmount.Enabled = false;
                    btnWthdraw.Enabled = false;
                }
            }
            else
            {
                lblCurrentBalance.Text = "N/A";
                tbxAmount.Enabled = false;
                btnWthdraw.Enabled = false;
            }
        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void tbxSearchAccountNumber_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
