using System;
using System.Drawing;
using System.Windows.Forms;

namespace VaultLinkBankSystem.Forms.CustomersFolder
{
    public partial class frmCustomerPIN : Form
    {
        private CustomerRepository _customerRepo;
        private VaultLinkBankSystem.Customer _loggedInCustomer;

        private string newPinInput = "";
        private string oldPinVerified = ""; // Track if old PIN was verified
        private bool isFirstTimeLogin;
        private bool isConfirmStage = false;
        private bool isChangingPin = false;

        private Point nextPosition = new Point(310, 284);
        private Point confirmPosition = new Point(390, 284);

        public frmCustomerPIN(VaultLinkBankSystem.Customer customer, bool firstTimeLogin)
        {
            InitializeComponent();

            _customerRepo = new CustomerRepository();
            _loggedInCustomer = customer;
            isFirstTimeLogin = firstTimeLogin;

            this.Load += frmCustomerPIN_Load;
            txtHiddenPin.TextChanged += TxtHiddenPin_TextChanged;
            txtHiddenPin.KeyPress += TxtHiddenPin_KeyPress;
            btnConfirm.Click += btnConfirm_Click_1;
            btnGoBack.Click += btnGoBack_Click;
        }

        public frmCustomerPIN(VaultLinkBankSystem.Customer customer)
        {
            InitializeComponent();

            _customerRepo = new CustomerRepository();
            _loggedInCustomer = customer;
            isFirstTimeLogin = false;
            isChangingPin = true;

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

            if (isFirstTimeLogin)
            {
                iconPictureBox2.Visible = false;
                lblPinTitle.Text = "Create Your 6-Digit PIN:";
            }
            else if (isChangingPin)
            {
                iconPictureBox2.Visible = true;
                lblPinTitle.Text = "Enter Current PIN:";
            }
            else
            {
                lblPinTitle.Text = "Enter Your PIN:";
                iconPictureBox2.Visible = true;
            }

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
            if (txtHiddenPin.Text.Length < 6)
            {
                MessageBox.Show("Please enter a 6-digit PIN.", "Incomplete PIN",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // ========== FIRST-TIME LOGIN: Create new PIN ==========
                if (isFirstTimeLogin)
                {
                    if (!isConfirmStage)
                    {
                        newPinInput = txtHiddenPin.Text;
                        txtHiddenPin.Clear();
                        ResetCircles();

                        isConfirmStage = true;
                        lblPinTitle.Text = "Confirm Your PIN:";
                        UpdateButtonPosition();
                        return;
                    }
                    else
                    {
                        if (txtHiddenPin.Text == newPinInput)
                        {
                            bool success = _customerRepo.SetCustomerPIN(_loggedInCustomer.CustomerID, newPinInput);

                            if (success)
                            {
                                // Clear the must change flag
                                _customerRepo.ClearMustChangePIN(_loggedInCustomer.CustomerID);

                                MessageBox.Show("PIN changed successfully!", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Failed to save PIN. Please try again.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("PINs do not match. Please try again.", "Mismatch",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtHiddenPin.Clear();
                            ResetCircles();
                        }
                    }
                }
                // ========== CHANGING PIN: 3-stage process ==========
                else if (isChangingPin)
                {
                    // STAGE 1: Verify current PIN
                    if (string.IsNullOrEmpty(oldPinVerified))
                    {
                        bool isValid = _customerRepo.VerifyCustomerPIN(_loggedInCustomer.CustomerID, txtHiddenPin.Text);

                        if (isValid)
                        {
                            oldPinVerified = txtHiddenPin.Text;
                            txtHiddenPin.Clear();
                            ResetCircles();
                            lblPinTitle.Text = "Enter New PIN:";
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Current PIN is incorrect.", "Invalid PIN",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtHiddenPin.Clear();
                            ResetCircles();
                            return;
                        }
                    }

                    // STAGE 2: Enter new PIN
                    if (!isConfirmStage)
                    {
                        newPinInput = txtHiddenPin.Text;
                        txtHiddenPin.Clear();
                        ResetCircles();

                        isConfirmStage = true;
                        lblPinTitle.Text = "Confirm New PIN:";
                        UpdateButtonPosition();
                        return;
                    }

                    // STAGE 3: Confirm new PIN
                    if (isConfirmStage)
                    {
                        if (txtHiddenPin.Text != newPinInput)
                        {
                            MessageBox.Show("PINs do not match. Please try again.", "Mismatch",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtHiddenPin.Clear();
                            ResetCircles();
                            return;
                        }

                        bool success = _customerRepo.UpdateCustomerPIN(_loggedInCustomer.CustomerID,
                                                                       oldPIN: oldPinVerified,
                                                                       newPIN: newPinInput);

                        if (success)
                        {
                            MessageBox.Show("PIN changed successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update PIN.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                // ========== NORMAL LOGIN: Verify PIN ==========
                else
                {
                    bool isValid = _customerRepo.VerifyCustomerPIN(_loggedInCustomer.CustomerID, txtHiddenPin.Text);

                    if (isValid)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect PIN. Please try again.", "Invalid PIN",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtHiddenPin.Clear();
                        ResetCircles();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            txtHiddenPin.Focus();
        }

        private void btnGoBack_Click(object sender, EventArgs e)
        {
            if (isConfirmStage)
            {
                isConfirmStage = false;

                if (isFirstTimeLogin)
                {
                    lblPinTitle.Text = "Create Your 6-Digit PIN:";
                }
                else if (isChangingPin)
                {
                    lblPinTitle.Text = "Enter New PIN:";
                }

                newPinInput = "";
                txtHiddenPin.Clear();
                ResetCircles();
                UpdateButtonPosition();
                txtHiddenPin.Focus();
            }
        }

        private void iconPictureBox2_Click(object sender, EventArgs e)
        {
            if (!isFirstTimeLogin)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void frmCustomerPIN_Load_1(object sender, EventArgs e)
        {
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
        }
    }
}