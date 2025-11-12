namespace VaultLinkBankSystem
{
    partial class frmDashBoard
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCustId = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lblCustAccountNum = new System.Windows.Forms.Label();
            this.btnCreateSavingsAcc = new System.Windows.Forms.Button();
            this.txtCustIDforSavingAccCreation = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(260, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Create Customer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(39, 250);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(266, 37);
            this.label2.TabIndex = 1;
            this.label2.Text = "Search Customer";
            // 
            // txtCustId
            // 
            this.txtCustId.Location = new System.Drawing.Point(91, 308);
            this.txtCustId.Name = "txtCustId";
            this.txtCustId.Size = new System.Drawing.Size(172, 20);
            this.txtCustId.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(19, 75);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(91, 370);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblCustAccountNum
            // 
            this.lblCustAccountNum.AutoSize = true;
            this.lblCustAccountNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustAccountNum.Location = new System.Drawing.Point(58, 451);
            this.lblCustAccountNum.Name = "lblCustAccountNum";
            this.lblCustAccountNum.Size = new System.Drawing.Size(53, 37);
            this.lblCustAccountNum.TabIndex = 5;
            this.lblCustAccountNum.Text = "....";
            // 
            // btnCreateSavingsAcc
            // 
            this.btnCreateSavingsAcc.Location = new System.Drawing.Point(563, 162);
            this.btnCreateSavingsAcc.Name = "btnCreateSavingsAcc";
            this.btnCreateSavingsAcc.Size = new System.Drawing.Size(98, 23);
            this.btnCreateSavingsAcc.TabIndex = 6;
            this.btnCreateSavingsAcc.Text = "Create Savings";
            this.btnCreateSavingsAcc.UseVisualStyleBackColor = true;
            this.btnCreateSavingsAcc.Click += new System.EventHandler(this.btnCreateSavingsAcc_Click);
            // 
            // txtCustIDforSavingAccCreation
            // 
            this.txtCustIDforSavingAccCreation.Location = new System.Drawing.Point(563, 125);
            this.txtCustIDforSavingAccCreation.Name = "txtCustIDforSavingAccCreation";
            this.txtCustIDforSavingAccCreation.Size = new System.Drawing.Size(172, 20);
            this.txtCustIDforSavingAccCreation.TabIndex = 7;
            this.txtCustIDforSavingAccCreation.TextChanged += new System.EventHandler(this.txtCustIDforSavingAccCreation_TextChanged);
            // 
            // frmDashBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 544);
            this.Controls.Add(this.txtCustIDforSavingAccCreation);
            this.Controls.Add(this.btnCreateSavingsAcc);
            this.Controls.Add(this.lblCustAccountNum);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtCustId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "frmDashBoard";
            this.Text = "frmDashBoard";
            this.Load += new System.EventHandler(this.frmDashBoard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCustId;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblCustAccountNum;
        private System.Windows.Forms.Button btnCreateSavingsAcc;
        private System.Windows.Forms.TextBox txtCustIDforSavingAccCreation;
    }
}