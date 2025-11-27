using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using VaultLinkBankSystem.Helpers;

namespace VaultLinkBankSystem.UserControls.Admin
{
    public partial class UC_AdminDashboard : UserControl
    {

        private AccountRepository _accountRepo;
        private CustomerRepository _customerRepo;
        private TransactionRepository _transactionRepo;
        private Timer _refreshTimer;
        public UC_AdminDashboard()
        {
            InitializeComponent();
            _accountRepo = new AccountRepository();
            _customerRepo = new CustomerRepository();
            _transactionRepo = new TransactionRepository();

            lstNotifications.BackColor = Color.FromArgb(20, 40, 70);



/*            StyleDashboardChart();
*/
              this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();


            _refreshTimer = new Timer();
            _refreshTimer.Interval = 30000; // 30 seconds
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        
        }

        private void UC_AdminDashboard_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
            LoadDashboardData();
        }


        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadDashboardData();
        }



        private void LoadDashboardData()
        {
            try
            {
                LoadStatistics();
                LoadTransactionChart();
                LoadRecentTransactions();
                LoadNotifications();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void LoadStatistics()
        {
            try
            {
                // Total Balance - sum of all account balances
                var allAccounts = _accountRepo.GetAllAccounts();
                decimal totalBalance = allAccounts.Sum(a => a.Balance);
                lblTotalBalance.Text = totalBalance.ToString("₱#,##0.00");

                // Total Users - count of all customers
                var allCustomers = _customerRepo.GetAllCustomers();
                lblTotalUsers.Text = allCustomers.Count.ToString();

                // New Accounts - accounts opened in last 30 days
                DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);
                int newAccounts = allAccounts.Count(a => a.DateOpened >= thirtyDaysAgo);
                lblNewAccounts.Text = newAccounts.ToString();
            }
            catch (Exception ex)
            {
                lblTotalBalance.Text = "Error";
                lblTotalUsers.Text = "Error";
                lblNewAccounts.Text = "Error";
            }
        }

        private void LoadTransactionChart()
        {
            try
            {
                Chart chart = chartWithdrawals;

                chart.Series.Clear();
                chart.ChartAreas.Clear();
                chart.Legends.Clear();
                chart.BackColor = Color.Transparent;
                chart.BorderlineWidth = 0;

                // Create chart area
                ChartArea area = new ChartArea("MainArea");
                area.BackColor = Color.Transparent;
                area.AxisX.MajorGrid.Enabled = false;
                area.AxisY.MajorGrid.LineColor = Color.FromArgb(50, 255, 255, 255);
                area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                area.AxisX.LabelStyle.ForeColor = Color.White;
                area.AxisY.LabelStyle.ForeColor = Color.White;
                area.AxisX.LineColor = Color.FromArgb(70, 130, 180);
                area.AxisY.LineColor = Color.FromArgb(70, 130, 180);
                area.AxisX.MajorTickMark.LineColor = Color.White;
                area.AxisY.MajorTickMark.LineColor = Color.White;
                area.AxisY.LabelStyle.Format = "{0}";
                area.AxisY.Interval = 5; // Show intervals of 5

                chart.ChartAreas.Add(area);

                // Create legend
                Legend legend = new Legend();
                legend.Docking = Docking.Top;
                legend.ForeColor = Color.White;
                legend.BackColor = Color.Transparent;
                legend.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                chart.Legends.Add(legend);

                // Get transaction data for last 7 days
                var transactionData = GetLast7DaysTransactionData();

                // Create Deposits series
                Series depositSeries = new Series("Deposits");
                depositSeries.ChartType = SeriesChartType.Column;
                depositSeries.Color = Color.FromArgb(0, 168, 255);
                depositSeries.BorderWidth = 0;
                depositSeries.ChartArea = "MainArea";
                depositSeries.IsValueShownAsLabel = true;
                depositSeries.LabelForeColor = Color.White;
                depositSeries.Font = new Font("Segoe UI", 8F, FontStyle.Bold);

                // Create Withdrawals series
                Series withdrawalSeries = new Series("Withdrawals");
                withdrawalSeries.ChartType = SeriesChartType.Column;
                withdrawalSeries.Color = Color.FromArgb(255, 85, 85);
                withdrawalSeries.BorderWidth = 0;
                withdrawalSeries.ChartArea = "MainArea";
                withdrawalSeries.IsValueShownAsLabel = true;
                withdrawalSeries.LabelForeColor = Color.White;
                withdrawalSeries.Font = new Font("Segoe UI", 8F, FontStyle.Bold);

                // Add data points
                foreach (var data in transactionData)
                {
                    depositSeries.Points.AddXY(data.DateLabel, data.DepositCount);
                    withdrawalSeries.Points.AddXY(data.DateLabel, data.WithdrawalCount);
                }

                chart.Series.Add(depositSeries);
                chart.Series.Add(withdrawalSeries);

                // Set column width
                depositSeries["PointWidth"] = "0.8";
                withdrawalSeries["PointWidth"] = "0.8";
            }
            catch (Exception ex)
            {
                // Keep existing styled chart if error
            }
        }


        private List<DailyTransactionData> GetLast7DaysTransactionData()
        {
            var result = new List<DailyTransactionData>();
            var allAccounts = _accountRepo.GetAllAccounts();

            for (int i = 6; i >= 0; i--)
            {
                DateTime date = DateTime.Now.AddDays(-i).Date;
                int depositCount = 0;
                int withdrawalCount = 0;

                foreach (var account in allAccounts)
                {
                    var transactions = _transactionRepo.GetTransactionsByAccountId(account.AccountID);

                    depositCount += transactions.Count(t =>
                        t.TransactionDate.Date == date &&
                        t.TransactionType == "Deposit");

                    withdrawalCount += transactions.Count(t =>
                        t.TransactionDate.Date == date &&
                        t.TransactionType == "Withdrawal");
                }

                result.Add(new DailyTransactionData
                {
                    Date = date,
                    DateLabel = date.ToString("MM/dd"),
                    DepositCount = depositCount,
                    WithdrawalCount = withdrawalCount
                });
            }

            return result;
        }


        private void LoadRecentTransactions()
        {
            try
            {
                dgvRecentTransactions.Rows.Clear();
                dgvRecentTransactions.Columns.Clear();

                // Setup columns
                dgvRecentTransactions.Columns.Add("ID", "ID");
                dgvRecentTransactions.Columns.Add("Name", "Name");
                dgvRecentTransactions.Columns.Add("Type", "Type");
                dgvRecentTransactions.Columns.Add("Date", "Date");

                dgvRecentTransactions.Columns["ID"].Width = 50;
                dgvRecentTransactions.Columns["Name"].Width = 120;
                dgvRecentTransactions.Columns["Type"].Width = 100;
                dgvRecentTransactions.Columns["Date"].Width = 140;
                dgvRecentTransactions.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                // Styling
                dgvRecentTransactions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(25, 45, 75);
                dgvRecentTransactions.RowTemplate.Height = 32;

                dgvRecentTransactions.Columns["ID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvRecentTransactions.DefaultCellStyle.Padding = new Padding(5, 3, 5, 3);
                dgvRecentTransactions.ColumnHeadersHeight = 35;
                dgvRecentTransactions.DefaultCellStyle.Font = new Font("Segoe UI", 9);
                dgvRecentTransactions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

                dgvRecentTransactions.BackgroundColor = Color.FromArgb(20, 40, 70);
                dgvRecentTransactions.ForeColor = Color.White;
                dgvRecentTransactions.DefaultCellStyle.BackColor = Color.FromArgb(20, 40, 70);
                dgvRecentTransactions.DefaultCellStyle.ForeColor = Color.White;
                dgvRecentTransactions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 60, 90);
                dgvRecentTransactions.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvRecentTransactions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 30, 60);
                dgvRecentTransactions.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvRecentTransactions.EnableHeadersVisualStyles = false;
                dgvRecentTransactions.GridColor = Color.FromArgb(40, 60, 90);
                dgvRecentTransactions.BorderStyle = BorderStyle.None;
                dgvRecentTransactions.RowHeadersVisible = false;
                dgvRecentTransactions.AllowUserToAddRows = false;
                dgvRecentTransactions.ReadOnly = true;
                dgvRecentTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                // Get all transactions
                var allAccounts = _accountRepo.GetAllAccounts();
                var allTransactions = new List<TransactionWithCustomer>();

                foreach (var account in allAccounts)
                {
                    var transactions = _transactionRepo.GetTransactionsByAccountId(account.AccountID);
                    var customer = _customerRepo.GetCustomerById(account.CustomerID);

                    foreach (var transaction in transactions)
                    {
                        allTransactions.Add(new TransactionWithCustomer
                        {
                            Transaction = transaction,
                            CustomerName = customer?.FullName ?? "Unknown"
                        });
                    }
                }

                // Sort by date descending and take top 10
                var recentTransactions = allTransactions
                    .OrderByDescending(t => t.Transaction.TransactionDate)
                    .Take(10)
                    .ToList();

                // Add to grid
                foreach (var item in recentTransactions)
                {
                    dgvRecentTransactions.Rows.Add(
                        item.Transaction.TransactionID,
                        item.CustomerName,
                        item.Transaction.TransactionType,
                        item.Transaction.TransactionDate.ToString("MM/dd/yyyy HH:mm")
                    );
                }
            }
            catch (Exception ex)
            {
                // Keep empty if error
            }
        }

        private void LoadNotifications()
        {
            try
            {
                lstNotifications.Items.Clear();

                // Get all transactions
                var allAccounts = _accountRepo.GetAllAccounts();
                var allTransactions = new List<TransactionWithCustomer>();

                foreach (var account in allAccounts)
                {
                    var transactions = _transactionRepo.GetTransactionsByAccountId(account.AccountID);
                    var customer = _customerRepo.GetCustomerById(account.CustomerID);

                    foreach (var transaction in transactions)
                    {
                        allTransactions.Add(new TransactionWithCustomer
                        {
                            Transaction = transaction,
                            CustomerName = customer?.FullName ?? "Unknown",
                            AccountNumber = account.AccountNumber
                        });
                    }
                }

                // Sort by date descending and take top 15
                var recentTransactions = allTransactions
                    .OrderByDescending(t => t.Transaction.TransactionDate)
                    .Take(15)
                    .ToList();

                // Add to listbox with formatting
                foreach (var item in recentTransactions)
                {
                    string notification = $"[{item.Transaction.TransactionDate:HH:mm}] " +
                                        $"{item.CustomerName} - " +
                                        $"{item.Transaction.TransactionType} " +
                                        $"{item.Transaction.Amount:C2} " +
                                        $"(Acc: {item.AccountNumber})";

                    lstNotifications.Items.Add(notification);
                }

                // Styling
                lstNotifications.BackColor = Color.FromArgb(20, 40, 70);
                lstNotifications.ForeColor = Color.White;
                lstNotifications.BorderStyle = BorderStyle.None;
            }
            catch (Exception ex)
            {
                lstNotifications.Items.Clear();
                lstNotifications.Items.Add("Error loading notifications");
            }
        }











        /*private void StyleDashboardChart()
        {
            Chart chart = chartWithdrawals;

            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Legends.Clear();
            chart.BackColor = Color.Transparent;
            chart.BorderlineWidth = 0;

            ChartArea area = new ChartArea("MainArea");
            area.BackColor = Color.Transparent;

            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.Enabled = false;
            area.AxisX.MinorGrid.Enabled = false;
            area.AxisY.MinorGrid.Enabled = false;

            area.AxisX.LabelStyle.ForeColor = Color.White;
            area.AxisY.LabelStyle.ForeColor = Color.White;

            area.AxisX.LineColor = Color.FromArgb(70, 130, 180);  
            area.AxisY.LineColor = Color.FromArgb(70, 130, 180);

            area.AxisX.MajorTickMark.LineColor = Color.White;
            area.AxisY.MajorTickMark.LineColor = Color.White;

            chart.ChartAreas.Add(area);

            Legend legend = new Legend();
            legend.Docking = Docking.Top;
            legend.ForeColor = Color.White;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            chart.Legends.Add(legend);

            Series deposit = new Series("Deposit");
            deposit.ChartType = SeriesChartType.Line;
            deposit.Color = Color.FromArgb(0, 168, 255); 
            deposit.BorderWidth = 3;
            deposit.ChartArea = "MainArea";
            deposit.IsValueShownAsLabel = false;

            deposit.Points.AddXY("01/10", 20000);
            deposit.Points.AddXY("01/12", 15000);
            deposit.Points.AddXY("01/14", 18000);

            chart.Series.Add(deposit);

            Series withdrawal = new Series("Withdrawals");
            withdrawal.ChartType = SeriesChartType.Line;
            withdrawal.Color = Color.FromArgb(255, 85, 85); 
            withdrawal.BorderWidth = 3;
            withdrawal.ChartArea = "MainArea";
            withdrawal.IsValueShownAsLabel = false;

            withdrawal.Points.AddXY("01/10", 8000);
            withdrawal.Points.AddXY("01/12", 12000);
            withdrawal.Points.AddXY("01/14", 6000);

            chart.Series.Add(withdrawal);

            deposit.BorderDashStyle = ChartDashStyle.Solid;
            withdrawal.BorderDashStyle = ChartDashStyle.Solid;

            deposit.ShadowColor = Color.FromArgb(50, deposit.Color);
            withdrawal.ShadowColor = Color.FromArgb(50, withdrawal.Color);

            deposit.ShadowOffset = 2;
            withdrawal.ShadowOffset = 2;

            deposit.BorderWidth = 4;
            withdrawal.BorderWidth = 4;
            deposit["LineTension"] = "0.5";     
            withdrawal["LineTension"] = "0.5";  
        }
*/
        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2HtmlLabel9_Click(object sender, EventArgs e)
        {

        }

        private void chartWithdrawals_Click(object sender, EventArgs e)
        {

        }
    }


}
