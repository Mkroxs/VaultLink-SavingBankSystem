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
        public UC_AdminDashboard()
        {
            InitializeComponent();
            StyleDashboardChart();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }

        private void UC_AdminDashboard_Load(object sender, EventArgs e)
        {
            UiHelpers.FixGuna2TextBoxVisibility(this);
        }
        private void StyleDashboardChart()
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
    }


}
