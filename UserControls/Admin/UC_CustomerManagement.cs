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

namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_CustomerManagement : UserControl
    {
        private CustomerRepository _customerRepo;
        private int _selectedCustomerId = 0;
        private List<Customer> _allCustomers;
        public UC_CustomerManagement()
        {
            InitializeComponent();

            _customerRepo = new CustomerRepository();
            _allCustomers = new List<Customer>();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            SetupGridStyle();

            this.dvgListOfCustomers.DataBindingComplete += dvgListOfCustomers_DataBindingComplete;
            this.dvgListOfCustomers.SelectionChanged += DvgListOfCustomers_SelectionChanged;
            this.dvgListOfCustomers.CellClick += DvgListOfCustomers_CellClick;
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

            // Make sure selection is full row and only one row can be selected
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;

            // Basic colors — pick contrasts that work with your theme
            dgv.BackgroundColor = Color.White;                       // grid background
            dgv.GridColor = Color.FromArgb(230, 230, 230);           // grid lines
            dgv.DefaultCellStyle.ForeColor = Color.Black;            // text color
            dgv.DefaultCellStyle.BackColor = Color.White;            // normal row background
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 249, 249); // alternate row

            // Header styling
            dgv.EnableHeadersVisualStyles = false; // let us control header colors
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 62, 84);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
            /*dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgv.ColumnHeadersDefaultCellStyle.BackColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;
*/
            // Selection styling — this is the highlight color when a row is selected
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 144, 255); // DodgerBlue-ish
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            // Make row headers less distracting (optional)
            dgv.RowHeadersVisible = false;

            // Improve cell padding / readability
            dgv.RowTemplate.Height = 28;
            dgv.RowTemplate.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);

            // Optional: remove cell border noise
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        }

        private void DvgListOfCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure click on actual row (not header)
            if (e.RowIndex >= 0)
            {
                dvgListOfCustomers.ClearSelection();
                dvgListOfCustomers.Rows[e.RowIndex].Selected = true;
                dvgListOfCustomers.CurrentCell = dvgListOfCustomers.Rows[e.RowIndex].Cells[Math.Max(0, e.ColumnIndex)];
            }
        }

        private void DvgListOfCustomers_SelectionChanged(object sender, EventArgs e)
        {
            // Keep only single-row selection and ensure current row is selected
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

            using (Forms.Admin.frmRegisterCust registerForm = new Forms.Admin.frmRegisterCust())
            {
                // Disable automatic form reposition
                registerForm.StartPosition = FormStartPosition.Manual;
                // Center it relative to parent
                registerForm.Location = new System.Drawing.Point(
                    parentForm.Location.X + (parentForm.Width - registerForm.Width) / 2,
                    parentForm.Location.Y + (parentForm.Height - registerForm.Height) / 2
                );

                registerForm.ShowDialog(parentForm);
            }
            LoadData();

            //using (Forms.Admin.frmAddCustomer addForm = new Forms.Admin.frmAddCustomer())
            //{
            //    addForm.ShowDialog();
            //}
            //LoadData();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dvgListOfCustomers.SelectedRows.Count > 0)
            {
                // Get the complete Customer object from the selected row
                Customer selectedCustomer = dvgListOfCustomers.SelectedRows[0].DataBoundItem as Customer;

                if (selectedCustomer != null)
                {
                    _selectedCustomerId = selectedCustomer.CustomerID;


                    /*btnViewDetails.Enabled = true;
                    btnDeleteCustomer.Enabled = true;*/
                }
            }
            else
            {
                _selectedCustomerId = 0;


                /*btnViewDetails.Enabled = false;
                btnDeleteCustomer.Enabled = false;*/
            }
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

            // Hidden columns (not shown in grid)
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

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

        }

        private void dvgListOfCustomers_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

        }

        private void UC_CustomerManagement_Load_1(object sender, EventArgs e)
        {
            BuildColumns();
            LoadData();


            // Disable buttons until a row is clicked
            /*btnViewDetails.Enabled = false;
            btnDeleteCustomer.Enabled = false;*/
        }

        private void btnVerification_Click(object sender, EventArgs e)
        {
             
            
        }
    }
}
