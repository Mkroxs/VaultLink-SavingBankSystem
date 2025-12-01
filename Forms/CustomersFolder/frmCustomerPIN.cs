using System;
using System.Drawing;
using System.Windows.Forms;

namespace VaultLinkBankSystem.Forms.CustomersFolder
{
    public partial class frmCustomerPIN : Form
    {
        private string savedOldPin = "123456";
        private string newPinInput = "";
        private bool isEnteringOldPin = true;
        private bool isConfirmStage = false;

        private Point nextPosition = new Point(310, 284);
        private Point confirmPosition = new Point(390, 284);

        public frmCustomerPIN()
        {
            InitializeComponent();

            this.Load += frmCustomerPIN_Load;
            txtHiddenPin.TextChanged += TxtHiddenPin_TextChanged;
            txtHiddenPin.KeyPress += TxtHiddenPin_KeyPress;

            btnConfirm.Click += btnConfirm_Click_1;
            btnGoBack.Click += btnGoBack_Click;
        }

        private void frmCustomerPIN_Load(object sender, EventArgs e)
        {
            MakeCircle(p1);
            MakeCircle(p2);
            MakeCircle(p3);
            MakeCircle(p4);
            MakeCircle(p5);
            MakeCircle(p6);

            txtHiddenPin.Focus();
            this.KeyPreview = true;
            this.ActiveControl = txtHiddenPin;

            lblPinTitle.Text = "Enter Current PIN:";
            UpdateButtonPosition();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData >= Keys.D0 && keyData <= Keys.D9) ||
                (keyData >= Keys.NumPad0 && keyData <= Keys.NumPad9))
            {
                if (txtHiddenPin.Text.Length < 6)
                {
                    string num = keyData.ToString().Replace("D", "").Replace("NumPad", "");
                    txtHiddenPin.Text += num;
                }
                return true;
            }

            if (keyData == Keys.Back && txtHiddenPin.Text.Length > 0)
            {
                txtHiddenPin.Text = txtHiddenPin.Text.Remove(txtHiddenPin.Text.Length - 1);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MakeCircle(Guna.UI2.WinForms.Guna2Panel panel)
        {
            panel.FillColor = Color.FromArgb(224, 224, 224);
        }

        private void TxtHiddenPin_TextChanged(object sender, EventArgs e)
        {
            string pin = txtHiddenPin.Text;

            ResetCircles();

            if (pin.Length >= 1) p1.FillColor = Color.FromArgb(20, 55, 90);
            if (pin.Length >= 2) p2.FillColor = Color.FromArgb(20, 55, 90);
            if (pin.Length >= 3) p3.FillColor = Color.FromArgb(20, 55, 90);
            if (pin.Length >= 4) p4.FillColor = Color.FromArgb(20, 55, 90);
            if (pin.Length >= 5) p5.FillColor = Color.FromArgb(20, 55, 90);
            if (pin.Length >= 6) p6.FillColor = Color.FromArgb(20, 55, 90);
        }

        private void TxtHiddenPin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void ResetCircles()
        {
            Color gray = Color.FromArgb(224, 224, 224);
            p1.FillColor = gray;
            p2.FillColor = gray;
            p3.FillColor = gray;
            p4.FillColor = gray;
            p5.FillColor = gray;
            p6.FillColor = gray;
        }

        private void UpdateButtonPosition()
        {
            if (!isConfirmStage)
            {
                btnConfirm.Text = "Next";
                btnConfirm.Location = nextPosition;
                btnGoBack.Visible = false;
            }
            else
            {
                btnConfirm.Text = "Confirm";
                btnConfirm.Location = confirmPosition;
                btnGoBack.Visible = true;
            }
        }

        private void btnConfirm_Click_1(object sender, EventArgs e)
        {
            if (isEnteringOldPin)
            {
                if (txtHiddenPin.Text.Length < 6)
                {
                    MessageBox.Show("Please enter your current 6-digit PIN.");
                    return;
                }

                if (txtHiddenPin.Text != savedOldPin)
                {
                    MessageBox.Show("Incorrect current PIN.");
                    txtHiddenPin.Clear();
                    ResetCircles();
                    return;
                }

                isEnteringOldPin = false;
                lblPinTitle.Text = "Enter New PIN:";
                txtHiddenPin.Clear();
                ResetCircles();
                return;
            }

            if (!isConfirmStage)
            {
                if (txtHiddenPin.Text.Length < 6)
                {
                    MessageBox.Show("Please enter a 6-digit PIN.");
                    return;
                }

                newPinInput = txtHiddenPin.Text;
                txtHiddenPin.Clear();
                ResetCircles();

                isConfirmStage = true;
                lblPinTitle.Text = "Confirm New PIN:";
                UpdateButtonPosition();
                return;
            }

            if (txtHiddenPin.Text == newPinInput)
            {
                MessageBox.Show("PIN successfully updated!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("PIN does not match.");
                txtHiddenPin.Clear();
                ResetCircles();
            }

            txtHiddenPin.Focus();
        }

        private void btnGoBack_Click(object sender, EventArgs e)
        {
            if (isConfirmStage)
            {
                isConfirmStage = false;
                lblPinTitle.Text = "Enter New PIN:";
                newPinInput = "";
            }
            else
            {
                isEnteringOldPin = true;
                lblPinTitle.Text = "Enter Current PIN:";
            }

            txtHiddenPin.Clear();
            ResetCircles();
            UpdateButtonPosition();
            txtHiddenPin.Focus();
        }

        private void iconPictureBox2_Click(object sender, EventArgs e)
        {
            //if first time login hide this iconPictureBox2
            this.Hide();
        }
    }
}
