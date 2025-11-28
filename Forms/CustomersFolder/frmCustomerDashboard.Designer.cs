namespace VaultLinkBankSystem.Forms.Customer
{
    partial class frmCustomerDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.panelMain = new Guna.UI2.WinForms.Guna2Panel();
            this.panelTopbar = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.iconPictureBox1 = new FontAwesome.Sharp.IconPictureBox();
            this.panelTopbar.SuspendLayout();
            this.guna2Panel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.Transparent;
            this.panelMain.BorderRadius = 40;
            this.panelMain.BorderThickness = 100;
            this.panelMain.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(30)))), ((int)(((byte)(57)))));
            this.panelMain.CustomizableEdges.TopLeft = false;
            this.panelMain.CustomizableEdges.TopRight = false;
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(70)))));
            this.panelMain.Location = new System.Drawing.Point(20, 192);
            this.panelMain.Name = "panelMain";
            this.panelMain.Padding = new System.Windows.Forms.Padding(30, 30, 50, 5);
            this.panelMain.Size = new System.Drawing.Size(1560, 788);
            this.panelMain.TabIndex = 18;
            // 
            // panelTopbar
            // 
            this.panelTopbar.BackColor = System.Drawing.Color.Transparent;
            this.panelTopbar.BorderRadius = 40;
            this.panelTopbar.BorderThickness = 100;
            this.panelTopbar.Controls.Add(this.guna2Panel1);
            this.panelTopbar.Controls.Add(this.panel1);
            this.panelTopbar.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(30)))), ((int)(((byte)(57)))));
            this.panelTopbar.CustomBorderThickness = new System.Windows.Forms.Padding(0, 3, 3, 8);
            this.panelTopbar.CustomizableEdges.BottomLeft = false;
            this.panelTopbar.CustomizableEdges.BottomRight = false;
            this.panelTopbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopbar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(70)))));
            this.panelTopbar.Location = new System.Drawing.Point(20, 20);
            this.panelTopbar.Name = "panelTopbar";
            this.panelTopbar.Size = new System.Drawing.Size(1560, 172);
            this.panelTopbar.TabIndex = 16;
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.Controls.Add(this.guna2HtmlLabel1);
            this.guna2Panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.guna2Panel1.Location = new System.Drawing.Point(986, 0);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Padding = new System.Windows.Forms.Padding(40, 50, 40, 40);
            this.guna2Panel1.Size = new System.Drawing.Size(574, 172);
            this.guna2Panel1.TabIndex = 21;
            // 
            // guna2HtmlLabel1
            // 
            this.guna2HtmlLabel1.AutoSize = false;
            this.guna2HtmlLabel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guna2HtmlLabel1.Font = new System.Drawing.Font("Malgun Gothic", 24F, System.Drawing.FontStyle.Bold);
            this.guna2HtmlLabel1.ForeColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Location = new System.Drawing.Point(40, 50);
            this.guna2HtmlLabel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            this.guna2HtmlLabel1.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.guna2HtmlLabel1.Size = new System.Drawing.Size(494, 82);
            this.guna2HtmlLabel1.TabIndex = 20;
            this.guna2HtmlLabel1.Text = "Welcome, John Melbern!";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.iconPictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(456, 172);
            this.panel1.TabIndex = 14;
            // 
            // iconPictureBox1
            // 
            this.iconPictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.iconPictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.iconPictureBox1.BackgroundImage = global::VaultLinkBankSystem.Properties.Resources.ChatGPT_Image_Oct_30__2025__07_25_46_PM_removebg_preview_1;
            this.iconPictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.iconPictureBox1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.iconPictureBox1.IconChar = FontAwesome.Sharp.IconChar.None;
            this.iconPictureBox1.IconColor = System.Drawing.SystemColors.ButtonFace;
            this.iconPictureBox1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconPictureBox1.IconSize = 362;
            this.iconPictureBox1.Location = new System.Drawing.Point(-38, -80);
            this.iconPictureBox1.Name = "iconPictureBox1";
            this.iconPictureBox1.Size = new System.Drawing.Size(524, 362);
            this.iconPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.iconPictureBox1.TabIndex = 13;
            this.iconPictureBox1.TabStop = false;
            // 
            // frmCustomerDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(30)))), ((int)(((byte)(57)))));
            this.ClientSize = new System.Drawing.Size(1600, 1000);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTopbar);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmCustomerDashboard";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmCustomerDashboard";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.frmCustomerDashboard_Load);
            this.panelTopbar.ResumeLayout(false);
            this.guna2Panel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2Panel panelMain;
        private Guna.UI2.WinForms.Guna2Panel panelTopbar;
        private System.Windows.Forms.Panel panel1;
        private FontAwesome.Sharp.IconPictureBox iconPictureBox1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
    }
}
