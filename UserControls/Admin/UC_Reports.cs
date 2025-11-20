using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VaultLinkBankSystem.Helpers;

namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_Reports : UserControl
    {
        public UC_Reports()
        {
            InitializeComponent();
            LoadReportGrid();


        }
        private void UC_Reports_Load(object sender, EventArgs e)
        {
            LoadReportGrid();
        }

        public void LoadReportGrid()
        {
           
            guna2DataGridView1.Columns.Clear();
            guna2DataGridView1.Rows.Clear();

          
            guna2DataGridView1.AutoGenerateColumns = false;
            guna2DataGridView1.RowHeadersVisible = false;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.AllowUserToResizeRows = false;
            guna2DataGridView1.ReadOnly = true;
            
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 40, 70);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            guna2DataGridView1.EnableHeadersVisualStyles = false;

            guna2DataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            guna2DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            guna2DataGridView1.DefaultCellStyle.BackColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            guna2DataGridView1.GridColor = Color.LightGray;
            guna2DataGridView1.BorderStyle = BorderStyle.None;

            guna2DataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            guna2DataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            guna2DataGridView1.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.White;
            guna2DataGridView1.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.Black;
            guna2DataGridView1.RowHeadersDefaultCellStyle.SelectionBackColor = Color.White;
            guna2DataGridView1.RowHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

            guna2DataGridView1.EnableHeadersVisualStyles = false;


            guna2DataGridView1.Columns.Add("TransactionID", "Transaction ID");
            guna2DataGridView1.Columns.Add("AccountNumber", "Account Number");
            guna2DataGridView1.Columns.Add("CustomerCode", "Customer");
            guna2DataGridView1.Columns.Add("TransactionType", "Transaction Type");
            guna2DataGridView1.Columns.Add("Amount", "Amount");
            guna2DataGridView1.Columns.Add("Date", "Date");
            guna2DataGridView1.Columns.Add("Status", "Status");
            guna2DataGridView1.Columns.Add("Remarks", "Remarks");

            foreach (DataGridViewColumn col in guna2DataGridView1.Columns)
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            guna2DataGridView1.Rows.Add("TX1001", "ACC2025-0001", "0001", "Withdraw", "₱2,000", "01/01/2025", "Completed", "Teller");
            guna2DataGridView1.Rows.Add("TX1002", "ACC2025-0001", "0001", "Deposit", "₱5,000", "01/02/2025", "Completed", "Teller");
            guna2DataGridView1.Rows.Add("TX1003", "ACC2025-0002", "0002", "Transfer", "₱1,500", "01/04/2025", "Pending", "Transfer to 0003");
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void UC_Reports_Load_1(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
        }
    }

}
