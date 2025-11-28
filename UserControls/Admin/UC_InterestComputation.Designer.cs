namespace VaultLinkBankSystem.UserControls.Admin
{
    partial class UC_InterestComputation
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.guna2Panel7 = new Guna.UI2.WinForms.Guna2Panel();
            this.btnComputeInterest = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Panel3 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Panel6 = new Guna.UI2.WinForms.Guna2Panel();
            this.dtpSelectMonth = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel5 = new Guna.UI2.WinForms.Guna2Panel();
            this.chkApplyToAll = new Guna.UI2.WinForms.Guna2CheckBox();
            this.guna2Panel4 = new Guna.UI2.WinForms.Guna2Panel();
            this.dvgListOfCustomers = new Guna.UI2.WinForms.Guna2DataGridView();
            this.lblAccountCount = new System.Windows.Forms.Label();
            this.lblTotalInterest = new System.Windows.Forms.Label();
            this.lblInterestRate = new System.Windows.Forms.Label();
            this.lblSelectedMonth = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.guna2Panel7.SuspendLayout();
            this.guna2Panel3.SuspendLayout();
            this.guna2Panel6.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.guna2Panel1.SuspendLayout();
            this.guna2Panel2.SuspendLayout();
            this.guna2Panel5.SuspendLayout();
            this.guna2Panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dvgListOfCustomers)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.guna2Panel7, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.guna2Panel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.guna2Panel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.guna2Panel4, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(19, 16);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(946, 574);
            this.tableLayoutPanel1.TabIndex = 12;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // guna2Panel7
            // 
            this.guna2Panel7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.guna2Panel7.Controls.Add(this.lblSelectedMonth);
            this.guna2Panel7.Controls.Add(this.lblInterestRate);
            this.guna2Panel7.Controls.Add(this.lblTotalInterest);
            this.guna2Panel7.Controls.Add(this.lblAccountCount);
            this.guna2Panel7.Controls.Add(this.btnComputeInterest);
            this.guna2Panel7.Location = new System.Drawing.Point(2, 527);
            this.guna2Panel7.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.guna2Panel7.Name = "guna2Panel7";
            this.guna2Panel7.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.guna2Panel7.Size = new System.Drawing.Size(940, 50);
            this.guna2Panel7.TabIndex = 7;
            // 
            // btnComputeInterest
            // 
            this.btnComputeInterest.BorderRadius = 20;
            this.btnComputeInterest.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnComputeInterest.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnComputeInterest.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnComputeInterest.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnComputeInterest.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnComputeInterest.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(55)))), ((int)(((byte)(90)))));
            this.btnComputeInterest.Font = new System.Drawing.Font("Malgun Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnComputeInterest.ForeColor = System.Drawing.Color.White;
            this.btnComputeInterest.Location = new System.Drawing.Point(670, 4);
            this.btnComputeInterest.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnComputeInterest.Name = "btnComputeInterest";
            this.btnComputeInterest.Size = new System.Drawing.Size(266, 42);
            this.btnComputeInterest.TabIndex = 13;
            this.btnComputeInterest.Text = "Compute Interest";
            this.btnComputeInterest.Click += new System.EventHandler(this.btnComputeInterest_Click);
            // 
            // guna2Panel3
            // 
            this.guna2Panel3.Controls.Add(this.guna2HtmlLabel2);
            this.guna2Panel3.Controls.Add(this.guna2Panel6);
            this.guna2Panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.guna2Panel3.Location = new System.Drawing.Point(2, 58);
            this.guna2Panel3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.guna2Panel3.Name = "guna2Panel3";
            this.guna2Panel3.Padding = new System.Windows.Forms.Padding(0, 0, 11, 0);
            this.guna2Panel3.Size = new System.Drawing.Size(1036, 55);
            this.guna2Panel3.TabIndex = 5;
            // 
            // guna2HtmlLabel2
            // 
            this.guna2HtmlLabel2.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.guna2HtmlLabel2.Font = new System.Drawing.Font("Malgun Gothic", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel2.ForeColor = System.Drawing.Color.White;
            this.guna2HtmlLabel2.Location = new System.Drawing.Point(0, 2);
            this.guna2HtmlLabel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            this.guna2HtmlLabel2.Size = new System.Drawing.Size(83, 21);
            this.guna2HtmlLabel2.TabIndex = 1;
            this.guna2HtmlLabel2.Text = "Select Date:";
            // 
            // guna2Panel6
            // 
            this.guna2Panel6.Controls.Add(this.dtpSelectMonth);
            this.guna2Panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.guna2Panel6.Location = new System.Drawing.Point(0, 23);
            this.guna2Panel6.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.guna2Panel6.Name = "guna2Panel6";
            this.guna2Panel6.Size = new System.Drawing.Size(1025, 32);
            this.guna2Panel6.TabIndex = 2;
            // 
            // dtpSelectMonth
            // 
            this.dtpSelectMonth.Animated = true;
            this.dtpSelectMonth.AutoRoundedCorners = true;
            this.dtpSelectMonth.Checked = true;
            this.dtpSelectMonth.CustomFormat = "MMMM, yyyy";
            this.dtpSelectMonth.Dock = System.Windows.Forms.DockStyle.Left;
            this.dtpSelectMonth.FillColor = System.Drawing.Color.White;
            this.dtpSelectMonth.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpSelectMonth.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpSelectMonth.Location = new System.Drawing.Point(0, 0);
            this.dtpSelectMonth.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dtpSelectMonth.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtpSelectMonth.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtpSelectMonth.Name = "dtpSelectMonth";
            this.dtpSelectMonth.Size = new System.Drawing.Size(320, 32);
            this.dtpSelectMonth.TabIndex = 0;
            this.dtpSelectMonth.Value = new System.DateTime(2025, 11, 19, 18, 46, 30, 454);
            this.dtpSelectMonth.ValueChanged += new System.EventHandler(this.dtpSelectMonth_ValueChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.guna2Panel1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1036, 52);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(273, 2);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(761, 0);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.Controls.Add(this.guna2HtmlLabel1);
            this.guna2Panel1.Location = new System.Drawing.Point(2, 2);
            this.guna2Panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.guna2Panel1.Size = new System.Drawing.Size(267, 48);
            this.guna2Panel1.TabIndex = 2;
            // 
            // guna2HtmlLabel1
            // 
            this.guna2HtmlLabel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.guna2HtmlLabel1.Font = new System.Drawing.Font("Malgun Gothic", 18F, System.Drawing.FontStyle.Bold);
            this.guna2HtmlLabel1.ForeColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Location = new System.Drawing.Point(4, 4);
            this.guna2HtmlLabel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            this.guna2HtmlLabel1.Size = new System.Drawing.Size(247, 40);
            this.guna2HtmlLabel1.TabIndex = 1;
            this.guna2HtmlLabel1.Text = "Interest Computation";
            // 
            // guna2Panel2
            // 
            this.guna2Panel2.AutoSize = true;
            this.guna2Panel2.Controls.Add(this.guna2Panel5);
            this.guna2Panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2Panel2.Location = new System.Drawing.Point(2, 117);
            this.guna2Panel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.guna2Panel2.Name = "guna2Panel2";
            this.guna2Panel2.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.guna2Panel2.Size = new System.Drawing.Size(1036, 44);
            this.guna2Panel2.TabIndex = 5;
            // 
            // guna2Panel5
            // 
            this.guna2Panel5.Controls.Add(this.chkApplyToAll);
            this.guna2Panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2Panel5.Location = new System.Drawing.Point(8, 8);
            this.guna2Panel5.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.guna2Panel5.Name = "guna2Panel5";
            this.guna2Panel5.Size = new System.Drawing.Size(1020, 28);
            this.guna2Panel5.TabIndex = 6;
            // 
            // chkApplyToAll
            // 
            this.chkApplyToAll.AutoSize = true;
            this.chkApplyToAll.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.chkApplyToAll.CheckedState.BorderRadius = 0;
            this.chkApplyToAll.CheckedState.BorderThickness = 0;
            this.chkApplyToAll.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.chkApplyToAll.Dock = System.Windows.Forms.DockStyle.Left;
            this.chkApplyToAll.Font = new System.Drawing.Font("Malgun Gothic", 10.2F, System.Drawing.FontStyle.Bold);
            this.chkApplyToAll.ForeColor = System.Drawing.Color.White;
            this.chkApplyToAll.Location = new System.Drawing.Point(0, 0);
            this.chkApplyToAll.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkApplyToAll.Name = "chkApplyToAll";
            this.chkApplyToAll.Size = new System.Drawing.Size(333, 28);
            this.chkApplyToAll.TabIndex = 0;
            this.chkApplyToAll.Text = "Apply interest to all active savings accounts ";
            this.chkApplyToAll.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.chkApplyToAll.UncheckedState.BorderRadius = 0;
            this.chkApplyToAll.UncheckedState.BorderThickness = 0;
            this.chkApplyToAll.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.chkApplyToAll.CheckedChanged += new System.EventHandler(this.chkApplyToAll_CheckedChanged);
            // 
            // guna2Panel4
            // 
            this.guna2Panel4.AutoSize = true;
            this.guna2Panel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.guna2Panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(70)))));
            this.guna2Panel4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.guna2Panel4.BorderRadius = 20;
            this.guna2Panel4.BorderThickness = 15;
            this.guna2Panel4.Controls.Add(this.dvgListOfCustomers);
            this.guna2Panel4.Location = new System.Drawing.Point(2, 165);
            this.guna2Panel4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.guna2Panel4.Name = "guna2Panel4";
            this.guna2Panel4.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.guna2Panel4.Size = new System.Drawing.Size(946, 358);
            this.guna2Panel4.TabIndex = 12;
            // 
            // dvgListOfCustomers
            // 
            this.dvgListOfCustomers.AllowUserToAddRows = false;
            this.dvgListOfCustomers.AllowUserToDeleteRows = false;
            this.dvgListOfCustomers.AllowUserToOrderColumns = true;
            this.dvgListOfCustomers.AllowUserToResizeColumns = false;
            this.dvgListOfCustomers.AllowUserToResizeRows = false;
            dataGridViewCellStyle13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dvgListOfCustomers.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle13;
            this.dvgListOfCustomers.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle14.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Malgun Gothic", 10.2F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle14.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dvgListOfCustomers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle14;
            this.dvgListOfCustomers.ColumnHeadersHeight = 35;
            this.dvgListOfCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(45)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(45)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(45)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dvgListOfCustomers.DefaultCellStyle = dataGridViewCellStyle15;
            this.dvgListOfCustomers.EnableHeadersVisualStyles = true;
            this.dvgListOfCustomers.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(70)))));
            this.dvgListOfCustomers.Location = new System.Drawing.Point(8, 8);
            this.dvgListOfCustomers.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dvgListOfCustomers.Name = "dvgListOfCustomers";
            this.dvgListOfCustomers.ReadOnly = true;
            this.dvgListOfCustomers.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dvgListOfCustomers.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle16.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dvgListOfCustomers.RowHeadersDefaultCellStyle = dataGridViewCellStyle16;
            this.dvgListOfCustomers.RowHeadersVisible = false;
            this.dvgListOfCustomers.RowHeadersWidth = 51;
            this.dvgListOfCustomers.RowTemplate.Height = 24;
            this.dvgListOfCustomers.Size = new System.Drawing.Size(928, 340);
            this.dvgListOfCustomers.TabIndex = 10;
            this.dvgListOfCustomers.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dvgListOfCustomers.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dvgListOfCustomers.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dvgListOfCustomers.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dvgListOfCustomers.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dvgListOfCustomers.ThemeStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dvgListOfCustomers.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(70)))));
            this.dvgListOfCustomers.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dvgListOfCustomers.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dvgListOfCustomers.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dvgListOfCustomers.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dvgListOfCustomers.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dvgListOfCustomers.ThemeStyle.HeaderStyle.Height = 35;
            this.dvgListOfCustomers.ThemeStyle.ReadOnly = true;
            this.dvgListOfCustomers.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dvgListOfCustomers.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dvgListOfCustomers.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dvgListOfCustomers.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dvgListOfCustomers.ThemeStyle.RowsStyle.Height = 24;
            this.dvgListOfCustomers.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dvgListOfCustomers.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            // 
            // lblAccountCount
            // 
            this.lblAccountCount.AutoSize = true;
            this.lblAccountCount.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccountCount.ForeColor = System.Drawing.SystemColors.Window;
            this.lblAccountCount.Location = new System.Drawing.Point(16, 17);
            this.lblAccountCount.Name = "lblAccountCount";
            this.lblAccountCount.Size = new System.Drawing.Size(38, 29);
            this.lblAccountCount.TabIndex = 14;
            this.lblAccountCount.Text = ".....";
            // 
            // lblTotalInterest
            // 
            this.lblTotalInterest.AutoSize = true;
            this.lblTotalInterest.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalInterest.ForeColor = System.Drawing.SystemColors.Window;
            this.lblTotalInterest.Location = new System.Drawing.Point(165, 17);
            this.lblTotalInterest.Name = "lblTotalInterest";
            this.lblTotalInterest.Size = new System.Drawing.Size(38, 29);
            this.lblTotalInterest.TabIndex = 15;
            this.lblTotalInterest.Text = ".....";
            // 
            // lblInterestRate
            // 
            this.lblInterestRate.AutoSize = true;
            this.lblInterestRate.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInterestRate.ForeColor = System.Drawing.SystemColors.Window;
            this.lblInterestRate.Location = new System.Drawing.Point(332, 17);
            this.lblInterestRate.Name = "lblInterestRate";
            this.lblInterestRate.Size = new System.Drawing.Size(38, 29);
            this.lblInterestRate.TabIndex = 16;
            this.lblInterestRate.Text = ".....";
            // 
            // lblSelectedMonth
            // 
            this.lblSelectedMonth.AutoSize = true;
            this.lblSelectedMonth.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedMonth.ForeColor = System.Drawing.SystemColors.Window;
            this.lblSelectedMonth.Location = new System.Drawing.Point(458, 21);
            this.lblSelectedMonth.Name = "lblSelectedMonth";
            this.lblSelectedMonth.Size = new System.Drawing.Size(38, 29);
            this.lblSelectedMonth.TabIndex = 17;
            this.lblSelectedMonth.Text = ".....";
            // 
            // UC_InterestComputation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(70)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "UC_InterestComputation";
            this.Padding = new System.Windows.Forms.Padding(19, 16, 15, 16);
            this.Size = new System.Drawing.Size(980, 606);
            this.Load += new System.EventHandler(this.UC_InterestComputation_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.guna2Panel7.ResumeLayout(false);
            this.guna2Panel7.PerformLayout();
            this.guna2Panel3.ResumeLayout(false);
            this.guna2Panel3.PerformLayout();
            this.guna2Panel6.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel1.PerformLayout();
            this.guna2Panel2.ResumeLayout(false);
            this.guna2Panel5.ResumeLayout(false);
            this.guna2Panel5.PerformLayout();
            this.guna2Panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dvgListOfCustomers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel4;
        private Guna.UI2.WinForms.Guna2DataGridView dvgListOfCustomers;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel3;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel5;
        private Guna.UI2.WinForms.Guna2CheckBox chkApplyToAll;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpSelectMonth;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel6;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel7;
        private Guna.UI2.WinForms.Guna2Button btnComputeInterest;
        private System.Windows.Forms.Label lblAccountCount;
        private System.Windows.Forms.Label lblSelectedMonth;
        private System.Windows.Forms.Label lblInterestRate;
        private System.Windows.Forms.Label lblTotalInterest;
    }
}
