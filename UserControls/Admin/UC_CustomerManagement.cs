using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VaultLinkBankSystem.Forms.Admin;
using static Syncfusion.Windows.Forms.TabBar;

namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_CustomerManagement : UserControl
    {
        private CustomerRepository _customerRepo;
        private int _selectedCustomerId = 0;
        private List<VaultLinkBankSystem.Customer> _allCustomers;
        private List<VaultLinkBankSystem.Customer> customers = new List<VaultLinkBankSystem.Customer>();


        private Forms.Admin.frmViewDetails _preloadedViewDetails;

        public UC_CustomerManagement()
        {
            InitializeComponent();

            _customerRepo = new CustomerRepository();
            _allCustomers = new List<VaultLinkBankSystem.Customer>();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            SetupGridStyle();

            this.dvgListOfCustomers.DataBindingComplete += dvgListOfCustomers_DataBindingComplete;
            this.dvgListOfCustomers.SelectionChanged += DvgListOfCustomers_SelectionChanged;
            this.dvgListOfCustomers.CellClick += DvgListOfCustomers_CellClick;


            try
            {
                VaultLinkBankSystem.Customer selectedCustomer;

                selectedCustomer = dvgListOfCustomers.SelectedRows[0].DataBoundItem as VaultLinkBankSystem.Customer;

                _preloadedViewDetails = new Forms.Admin.frmViewDetails(selectedCustomer);

                var h = _preloadedViewDetails.Handle;
                _preloadedViewDetails.CreateControl();
                _preloadedViewDetails.SuspendLayout();
                _preloadedViewDetails.ResumeLayout(false);
                _preloadedViewDetails.PerformLayout();

                _preloadedViewDetails.Visible = false;

                this.Disposed += (s, e) =>
                {
                    // Clean up _preloadedViewDetails when the control is disposed.
                    try { _preloadedViewDetails?.Dispose(); } catch { }
                };

            }
            catch
            {
                _preloadedViewDetails = null;
            }

        }


        





        private void LoadData()
        {
            try
            {
                _allCustomers = _customerRepo.GetAllCustomers();

                dvgListOfCustomers.DataSource = _allCustomers;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customer data: {ex.Message}");
            }
        }




















        private void SetupGridStyle()
        {
            var dgv = dvgListOfCustomers;

            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;

            dgv.BackgroundColor = Color.White;                       
            dgv.GridColor = Color.FromArgb(230, 230, 230);           
            dgv.DefaultCellStyle.ForeColor = Color.Black;            
            dgv.DefaultCellStyle.BackColor = Color.White;            
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249); 

            dgv.EnableHeadersVisualStyles = false; 
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 62, 84);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 144, 255); 
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            dgv.RowHeadersVisible = false;

            dgv.RowTemplate.Height = 28;
            dgv.RowTemplate.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);

            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        }

        private void DvgListOfCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dvgListOfCustomers.ClearSelection();
                dvgListOfCustomers.Rows[e.RowIndex].Selected = true;
                dvgListOfCustomers.CurrentCell = dvgListOfCustomers.Rows[e.RowIndex].Cells[Math.Max(0, e.ColumnIndex)];
            }
        }

        private void DvgListOfCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dvgListOfCustomers.SelectedRows.Count > 0)
            {
                var row = dvgListOfCustomers.SelectedRows[0];
                if (!row.Selected)
                    row.Selected = true;
            }
        }


        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Form parentForm = this.FindForm();

            using (Forms.Admin.frmRegistration registerForm = new Forms.Admin.frmRegistration())
            {
                registerForm.StartPosition = FormStartPosition.Manual;
                registerForm.Location = new System.Drawing.Point(
                    parentForm.Location.X + (parentForm.Width - registerForm.Width) / 2,
                    parentForm.Location.Y + (parentForm.Height - registerForm.Height) / 2
                );

                registerForm.ShowDialog(parentForm);
            }
            LoadData();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dvgListOfCustomers.SelectedRows.Count > 0)
            {
                VaultLinkBankSystem.Customer selectedCustomer = dvgListOfCustomers.SelectedRows[0].DataBoundItem as VaultLinkBankSystem.Customer;

                if (selectedCustomer != null)
                {
                    _selectedCustomerId = selectedCustomer.CustomerID;
                }
            }
            else
            {
                _selectedCustomerId = 0;
            }
        }

        private void guna2DataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BuildColumns()
        {
            dvgListOfCustomers.Columns.Clear();

            dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CustomerCode",
                HeaderText = "Code",
                Width = 100
            });

            dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FullName",
                HeaderText = "Full Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Email",
                HeaderText = "Email",
                Width = 180
            });

            dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Phone",
                HeaderText = "Phone",
                Width = 130
            });

            dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "EmploymentStatus",
                HeaderText = "Employment",
                Width = 150
            });

            dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CreatedAt",
                HeaderText = "Registered",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "MM/dd/yyyy" }
            });

            string[] hiddenColumns =
            {
        "CustomerID", "PIN", "ImagePath", "IsKYCVerified", "KYCVerifiedDate",
        "Address", "Gender", "BirthDate", "CivilStatus",
        "EmployerName", "SourceOfFunds", "MonthlyIncomeRange",
        "IDType", "IDNumber"
    };

            foreach (string colName in hiddenColumns)
            {
                dvgListOfCustomers.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = colName,
                    Name = colName,
                    Visible = false
                });
            }
        }

        private void btnViewDetails(object sender, EventArgs e)
        {
            if (dvgListOfCustomers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer first.");
                return;
            }

            VaultLinkBankSystem.Customer selectedCustomer = dvgListOfCustomers.SelectedRows[0].DataBoundItem as VaultLinkBankSystem.Customer;

            if (selectedCustomer == null)
            {
                MessageBox.Show("Invalid customer selection.");
                return;
            }

            using (frmViewDetails viewForm = new frmViewDetails(selectedCustomer))
            {
                viewForm.ShowDialog();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txbCustomerSearch.Text.Trim();

                // If empty → reload full data
                if (string.IsNullOrEmpty(searchTerm))
                {
                    dvgListOfCustomers.DataSource = _allCustomers;
                    return;
                }

                // Search through already-loaded VaultLinkBankSystem.Customer
                var foundCustomers = _allCustomers
                    .Where(c =>
    (c.CustomerCode != null && c.CustomerCode.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
    (c.FullName != null && c.FullName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
    (c.Email != null && c.Email.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
    (c.Phone != null && c.Phone.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
)
                    .ToList();

                dvgListOfCustomers.DataSource = foundCustomers;

                if (foundCustomers.Count == 0)
                {
                    MessageBox.Show("No matching customers found.", "Search Result",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during search: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
    
}





        private void dvgListOfCustomers_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

        }

        private void UC_CustomerManagement_Load_1(object sender, EventArgs e)
        {
            BuildColumns();
            LoadData();

        }

        private void btnVerification_Click(object sender, EventArgs e)
        {


        }

        private void txbCustomerSearch_TextChanged(object sender, EventArgs e)
        {

            string keyword = txbCustomerSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(keyword))
            {
                dvgListOfCustomers.DataSource = _allCustomers; // reset
                return;
            }

            var filtered = _allCustomers
                .Where(c =>
                    (!string.IsNullOrEmpty(c.FullName) && c.FullName.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(c.Address) && c.Address.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(c.Email) && c.Email.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(c.Phone) && c.Phone.ToLower().Contains(keyword))
                )
                .ToList();

            dvgListOfCustomers.DataSource = filtered;
        }

        private void txbCustomerSearch_Click(object sender, EventArgs e)
        {
            txbCustomerSearch.Clear();
        }

        private void txbCustomerSearch_Leave(object sender, EventArgs e)
        {
            txbCustomerSearch.Text = "Search";
        }
    }
}