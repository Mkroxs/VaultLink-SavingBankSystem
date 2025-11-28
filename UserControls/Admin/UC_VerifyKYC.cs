using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_VerifyKYC : UserControl
    {
        private CustomerRepository _customerRepo;
        private int _selectedCustomerId = 0;

        public UC_VerifyKYC()
        {
            InitializeComponent();
            _customerRepo = new CustomerRepository();

           
            dgvPendingKYC.SelectionChanged += DgvPendingKYC_SelectionChanged;
            dgvPendingKYC.CellFormatting += DgvPendingKYC_CellFormatting;

            
            btnVerify.Click += btnVerify_Click;
            btnReject.Click += btnReject_Click;
            btnRefresh.Click += btnRefresh_Click;
            dgvPendingKYC.DataBindingComplete += DgvPendingKYC_DataBindingComplete;

            LoadPendingKYC();


        }
        private void DgvPendingKYC_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ConfigureColumns();
        }

       
        private void UC_VerifyKYC_Load(object sender, EventArgs e)
        {

        }

        private void LoadPendingKYC()
        {
            try
            {
                // Get all customers
                var allCustomers = _customerRepo.GetAllCustomers();

                // Filter only unverified customers
                var pendingCustomers = allCustomers.Where(c => !c.IsKYCVerified).ToList();

                // Bind to DataGridView
                dgvPendingKYC.DataSource = null;
                dgvPendingKYC.DataSource = pendingCustomers;

                ConfigureColumns();

                // Update pending count label
                lblPendingCount.Text = $"Total Pending: {pendingCustomers.Count}";
                lblPendingCount.ForeColor = pendingCustomers.Count > 0 ?
                    Color.FromArgb(231, 76, 60) : Color.FromArgb(46, 204, 113);

                // Handle display based on count
                if (pendingCustomers.Count == 0)
                {
                    ShowNoDataMessage();
                }
                else
                {
                    // Auto-select first row after data loads
                    if (dgvPendingKYC.Rows.Count > 0)
                    {
                        dgvPendingKYC.ClearSelection();
                        dgvPendingKYC.Rows[0].Selected = true;

                        // Manually trigger display of first customer
                        Customers firstCustomer = dgvPendingKYC.Rows[0].DataBoundItem as Customers;
                        if (firstCustomer != null)
                        {
                            _selectedCustomerId = firstCustomer.CustomerID;
                            DisplayCustomerDetails(firstCustomer);
                            btnVerify.Enabled = true;
                            btnReject.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading pending KYC: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void ConfigureColumns()
        {
            try
            {
                foreach (DataGridViewColumn col in dgvPendingKYC.Columns)
                {
                    if (col == null) continue;
                    if (string.IsNullOrWhiteSpace(col.DataPropertyName)) continue;
                    if (string.IsNullOrWhiteSpace(col.Name))
                        col.Name = col.DataPropertyName; 

                    if (!ColumnExistsOnCustomer(col.DataPropertyName))
                        continue;
                    if (new[] {
                "CustomerID", "PIN","EmploymentStatus", "ImagePath", "IsKYCVerified", "KYCVerifiedDate",
                "Address", "Gender", "BirthDate", "CivilStatus",
                "EmployerName", "SourceOfFunds", "MonthlyIncomeRange", "IDType", "IDNumber"
            }.Contains(col.Name))
                    {
                        col.Visible = false;
                        continue;
                    }

                    switch (col.Name)
                    {
                        case "CustomerCode":
                            col.HeaderText = "Code";
                            col.Width = 100;
                            break;
                        case "FullName":
                            col.HeaderText = "Full Name";
                            break;
                        case "Email":
                            col.HeaderText = "Email";
                            break;
                        case "Phone":
                            col.HeaderText = "Phone";
                            break;
                        case "CreatedAt":
                            col.HeaderText = "Registered";
                            col.DefaultCellStyle.Format = "MM/dd/yyyy";
                            col.Width = 100;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Column config error: " + ex.Message);
            }
        }

        private bool ColumnExistsOnCustomer(string propertyName)
        {
            return typeof(Customers).GetProperty(propertyName) != null;
        }



        private void DgvPendingKYC_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPendingKYC.SelectedRows.Count > 0)
            {
                Customers selectedCustomer = dgvPendingKYC.SelectedRows[0].DataBoundItem as Customers;
                if (selectedCustomer != null)
                {
                    _selectedCustomerId = selectedCustomer.CustomerID;
                    DisplayCustomerDetails(selectedCustomer);
                    btnVerify.Enabled = true;
                    btnReject.Enabled = true;
                }
            }
        }

        private void DgvPendingKYC_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dgvPendingKYC.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            }
        }

        private void DisplayCustomerDetails(Customers customer)
        {
            txtCustomerCode.Text = customer.CustomerCode;
            txtFullName.Text = customer.FullName;
            txtEmail.Text = customer.Email ?? "N/A";
            txtPhone.Text = customer.Phone ?? "N/A";
            txtAddress.Text = customer.Address ?? "N/A";
            txtGender.Text = customer.Gender ?? "N/A";
            txtBirthDate.Text = customer.BirthDate?.ToString("MMMM dd, yyyy") ?? "N/A";
            txtCivilStatus.Text = customer.CivilStatus ?? "N/A";
            txtEmployment.Text = customer.EmploymentStatus ?? "N/A";
            txtEmployer.Text = customer.EmployerName ?? "N/A";
            txtSourceOfFunds.Text = customer.SourceOfFunds ?? "N/A";
            txtIncome.Text = customer.MonthlyIncomeRange ?? "N/A";
            txtIDType.Text = customer.IDType ?? "N/A";
            txtIDNumber.Text = customer.IDNumber ?? "N/A";
            txtRegistered.Text = customer.CreatedAt.ToString("MMMM dd, yyyy hh:mm tt");
        }

        private void ClearDetailsPanel()
        {
            txtCustomerCode.Text = "";
            txtFullName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            txtGender.Text = "";
            txtBirthDate.Text = "";
            txtCivilStatus.Text = "";
            txtEmployment.Text = "";
            txtEmployer.Text = "";
            txtSourceOfFunds.Text = "";
            txtIncome.Text = "";
            txtIDType.Text = "";
            txtIDNumber.Text = "";
            txtRegistered.Text = "";
            btnVerify.Enabled = false;
            btnReject.Enabled = false;
        }

        private void ShowNoDataMessage()
        {
            ClearDetailsPanel();
         
            txtFullName.Text = "No pending KYC verifications";
            txtCustomerCode.Text = "All customers verified! ✅";
        
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if (_selectedCustomerId == 0)
            {
                MessageBox.Show("Please select a customer first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Customers customer = _customerRepo.GetCustomerById(_selectedCustomerId);

                if (customer == null)
                {
                    MessageBox.Show("Customer not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to VERIFY the KYC for:\n\n" +
                    $"Customer: {customer.FullName}\n" +
                    $"Email: {customer.Email}\n" +
                    $"ID: {customer.IDType} - {customer.IDNumber}\n\n" +
                    $"This will allow the customer to:\n" +
                    $"✓ Create savings accounts\n" +
                    $"✓ Login to kiosk\n" +
                    $"✓ Use all banking services",
                    "Confirm KYC Verification",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    bool success = _customerRepo.VerifyKYC(_selectedCustomerId);

                    if (success)
                    {
                        MessageBox.Show(
                            $"✅ KYC VERIFIED SUCCESSFULLY!\n\n" +
                            $"Customer: {customer.FullName}\n" +
                            $"Customer Code: {customer.CustomerCode}\n" +
                            $"Verified: {DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt")}\n\n" +
                            $"The customer can now create accounts and use banking services.",
                            "Verification Complete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        _selectedCustomerId = 0;
                        LoadPendingKYC();
                    }
                    else
                    {
                        MessageBox.Show("Failed to verify KYC. Please try again.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error verifying KYC: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (_selectedCustomerId == 0)
            {
                MessageBox.Show("Please select a customer first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Customers customer = _customerRepo.GetCustomerById(_selectedCustomerId);

                if (customer == null)
                {
                    MessageBox.Show("Customer not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to REJECT the KYC for:\n\n" +
                    $"Customer: {customer.FullName}\n" +
                    $"Email: {customer.Email}\n\n" +
                    $"This action should be followed by contacting the customer.\n" +
                    $"You may need to request additional documents.",
                    "Confirm Rejection",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    MessageBox.Show(
                        "KYC rejection noted.\n\n" +
                        "Please contact the customer to request additional documentation.\n\n" +
                        "Customer record remains pending until verified.",
                        "KYC Rejected",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPendingKYC();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        private void dgvPendingKYC_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}