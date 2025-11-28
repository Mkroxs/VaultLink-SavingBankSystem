using System;
using System.Windows.Forms;
using VaultLinkBankSystem.UserControls.Registration;
using VaultLinkBankSystem.Helpers;

namespace VaultLinkBankSystem.Forms.Admin
{

    public partial class frmRegistration : Form
    {
        CustomerRepository customerRepo = new CustomerRepository();

        private UC_BasicInfo _ucBasicInfo;
        private UC_AddressInfo _ucAddressInfo;
        private UC_IdentityVerification _ucIdentityVerification;

        private UserControl _currentPage;

        private int _currentStep = 0;

        public frmRegistration()
        {
            InitializeComponent();

            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            UiHelpers.EnableDoubleBufferingRecursive(this);
            UiHelpers.TryEnableComposited(this);

            PreloadAllUserControls();

            // Wire up the Load event
            this.Load += FrmRegistration_Load;
        }

       
        private void PreloadAllUserControls()
        {
            this.SuspendLayout();
            panelMainRegister.SuspendLayout();

            try
            {
                _ucBasicInfo = new UC_BasicInfo();
                _ucAddressInfo = new UC_AddressInfo();
                _ucIdentityVerification = new UC_IdentityVerification();

                UiHelpers.EnableDoubleBufferingRecursive(_ucBasicInfo);
                UiHelpers.EnableDoubleBufferingRecursive(_ucAddressInfo);
                UiHelpers.EnableDoubleBufferingRecursive(_ucIdentityVerification);

                _ucBasicInfo.Dock = DockStyle.Fill;
                _ucBasicInfo.Visible = false;
                panelMainRegister.Controls.Add(_ucBasicInfo);

                _ucAddressInfo.Dock = DockStyle.Fill;
                _ucAddressInfo.Visible = false;
                panelMainRegister.Controls.Add(_ucAddressInfo);

                _ucIdentityVerification.Dock = DockStyle.Fill;
                _ucIdentityVerification.Visible = false;
                panelMainRegister.Controls.Add(_ucIdentityVerification);

                panelMainRegister.PerformLayout();
                _ucBasicInfo.CreateControl();
                _ucBasicInfo.PerformLayout();

                _ucAddressInfo.CreateControl();
                _ucAddressInfo.PerformLayout();

                _ucIdentityVerification.CreateControl();
                _ucIdentityVerification.PerformLayout();
            }
            finally
            {
                panelMainRegister.ResumeLayout(false);
                this.ResumeLayout(false);
            }
        }

        private void FrmRegistration_Load(object sender, EventArgs e)
        {
            try
            {
                _currentStep = 0;
                UiHelpers.ShowPage(panelMainRegister, _ucBasicInfo, ref _currentPage);

                btnNext.Click += BtnNext_Click;
                btnPrevious.Click += BtnPrevious_Click;
                btnRegister.Click += BtnRegister_Click;

                UpdateButtonVisibility();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      
        private void UpdateButtonVisibility()
        {
            switch (_currentStep)
            {
                case 0: 
                    btnPrevious.Visible = false;
                    btnNext.Visible = true;
                    btnRegister.Visible = false;
                    break;

                case 1: 
                    btnPrevious.Visible = true;
                    btnNext.Visible = true;
                    btnRegister.Visible = false;
                    break;

                case 2: 
                    btnPrevious.Visible = true;
                    btnNext.Visible = false;
                    btnRegister.Visible = true;
                    break;
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_currentStep < 2)
            {
                _currentStep++;
                ShowCurrentStep();
            }
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentStep > 0)
            {
                _currentStep--;
                ShowCurrentStep();
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Registration submitted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
        }

     
        private void ShowCurrentStep()
        {
            switch (_currentStep)
            {
                case 0:
                    ShowBasicInfo();
                    break;
                case 1:
                    ShowAddressInfo();
                    break;
                case 2:
                    ShowIdentityVerification();
                    break;
            }

            UpdateButtonVisibility();
        }

        public void ShowBasicInfo()
        {
            UiHelpers.ShowPage(panelMainRegister, _ucBasicInfo, ref _currentPage);
        }

        public void ShowAddressInfo()
        {
            UiHelpers.ShowPage(panelMainRegister, _ucAddressInfo, ref _currentPage);
        }

        public void ShowIdentityVerification()
        {
            UiHelpers.ShowPage(panelMainRegister, _ucIdentityVerification, ref _currentPage);
        }

        private void iconPictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panelMainRegister_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnRegister_Click_1(object sender, EventArgs e)
        {
            VaultLinkBankSystem.Customer testCustomer = new VaultLinkBankSystem.Customer()

            {
                CustomerCode = customerRepo.GenerateCustomerCode(),
                FullName = _ucBasicInfo.CustomerName,
                Address = _ucAddressInfo.CustomerAddress,
                Email = _ucBasicInfo.CustomerEmail,
                Phone = _ucBasicInfo.CustomerContactNumber,
                Gender = _ucBasicInfo.CustomerGender,
                BirthDate = _ucBasicInfo.CustomerBirthDate,
                CivilStatus = _ucBasicInfo.CustomerCivilStatus,
                ImagePath = "john.jpg",
                PIN = customerRepo.GeneratePIN(),
                EmploymentStatus = _ucIdentityVerification.CustomerEmploymentStatus,
                EmployerName = "Elon Musk",
                SourceOfFunds = _ucIdentityVerification.CustomerSourceOfFunds,
                MonthlyIncomeRange = _ucIdentityVerification.CustomerMonthlyIncome,
                IDType = _ucIdentityVerification.CustomerIDType,
                IDNumber = _ucIdentityVerification.CustomerIDNumber,
                IsKYCVerified = false, 
                KYCVerifiedDate = null
            };

            customerRepo.CreateCustomer(testCustomer);

            this.Close();
        }
    }
}