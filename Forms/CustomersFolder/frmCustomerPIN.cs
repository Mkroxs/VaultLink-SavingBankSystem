using System;
using System.Drawing;
using System.Windows.Forms;

namespace VaultLinkBankSystem.Forms.CustomersFolder
{
    public partial class frmCustomerPIN : Form
    {
        private string savedOldPin = "123456";

        private string oldPinInput = "";
        private string newPinInput = "";
        private bool isEnteringOldPin = true;
        private bool isConfirmStage = false;

        private Point nextPosition = new Point(314, 352);
        private Point confirmPosition = new Point(436, 352);

        public frmCustomerPIN()
        {
            InitializeComponent();

            this.Load += FrmCustomerPIN_Load;
            txtHiddenPin.TextChanged += TxtHiddenPin_TextChanged;
            txtHiddenPin.KeyPress += TxtHiddenPin_KeyPress;
        }

        private void FrmCustomerPIN_Load(object sender, EventArgs e)
        {
            MakePanelCircle(p1);
            MakePanelCircle(p2);
            MakePanelCircle(p3);
            MakePanelCircle(p4);
            MakePanelCircle(p5);
            MakePanelCircle(p6);

            txtHiddenPin.Focus();
            this.KeyPreview = true;
            this.ActiveControl = txtHiddenPin;
            btnPosition();

            // Start in "Enter Old PIN" mode
            lblPinTitle.Text = "Enter Current PIN:";
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

        private void MakePanelCircle(Panel p)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, p.Width, p.Height);
            p.Region = new Region(path);
            p.BackColor = Color.Gray;
        }

        private void TxtHiddenPin_TextChanged(object sender, EventArgs e)
        {
            string pin = txtHiddenPin.Text;

            ClearCircles();

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
            {
                e.Handled = true;
            }
        }

        private void btnPosition()
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

        private void ClearCircles()
        {
            Color emptyColor = Color.Gray;

            p1.FillColor = emptyColor;
            p2.FillColor = emptyColor;
            p3.FillColor = emptyColor;
            p4.FillColor = emptyColor;
            p5.FillColor = emptyColor;
            p6.FillColor = emptyColor;
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
                    ClearCircles();
                    return;
                }

                isEnteringOldPin = false;
                lblPinTitle.Text = "Enter New PIN:";
                txtHiddenPin.Clear();
                ClearCircles();
                txtHiddenPin.Focus();
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
                ClearCircles();

                isConfirmStage = true;
                lblPinTitle.Text = "Confirm New PIN:";
                btnPosition();
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
                ClearCircles();
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
            ClearCircles();
            btnPosition();
            txtHiddenPin.Focus();
        }
    }
}
