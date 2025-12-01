using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VaultLinkBankSystem.Helpers;
using VaultLinkBankSystem.UserControls.Registration;

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

        private string imagePath = null;

        public frmRegistration()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterParent;

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            UiHelpers.EnableDoubleBufferingRecursive(this);
            UiHelpers.TryEnableComposited(this);

            PreloadAllUserControls();

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
                UiHelpers.ForceRender(_ucBasicInfo);
                UiHelpers.ForceRender(_ucAddressInfo);
                UiHelpers.ForceRender(_ucIdentityVerification);

                _currentStep = 0;
                _currentPage = UiHelpers.ShowPage(panelMainRegister, _ucBasicInfo, _currentPage);

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
            if (ValidateCurrentStep())
            {
                if (_currentStep < 2)
                {
                    _currentStep++;
                    ShowCurrentStep();
                }
            }
        }

        private bool ValidateID(string idType, string idNumber)
        {
            string cleanId = idNumber.Replace("-", "").Replace(" ", "").Trim();
            string pattern = "";

            switch (idType)
            {
                case "National ID":
                    pattern = @"^\d{16}$";
                    break;
                case "Passport":
                    pattern = @"^[A-Z]\d{8}$";
                    break;
                case "Driver’s License":
                    pattern = @"^[A-Z]\d{10}$";
                    break;
                case "UMID":
                    pattern = @"^\d{12}$";
                    break;
                case "SSS ID":
                    pattern = @"^\d{10}$";
                    break;
                case "GSIS ID":
                    pattern = @"^\d{10,12}$";
                    break;
                case "TIN ID":
                    pattern = @"^\d{9,12}$";
                    break;
                case "Postal ID":
                    pattern = @"^([A-Z]{2}\d{9}|\d{7,9})$";
                    break;
                case "PRC License":
                    pattern = @"^\d{7}$";
                    break;
                case "Voter’s ID / COMELEC ID":
                    pattern = @"^[A-Z0-9]{15,25}$";
                    break;
                case "PWD ID":
                case "Senior Citizen ID":
                case "Student ID":
                case "Company ID":
                    pattern = @"^[a-zA-Z0-9]{4,20}$";
                    break;
                default:
                    pattern = @"^[a-zA-Z0-9]{4,20}$";
                    break;
            }

            return Regex.IsMatch(cleanId, pattern);
        }

        private bool ValidateCurrentStep()
        {
            if (_currentStep == 0)
            {
                if (!Regex.IsMatch(_ucBasicInfo.CustomerName, @"^[a-zA-Z\s\.\-]+$"))
                {
                    MessageBox.Show("Invalid Name. Please use letters only.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(_ucBasicInfo.CustomerEmail, emailPattern))
                {
                    MessageBox.Show("Invalid Email Address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                string phonePattern = @"^(09|\+639)\d{9}$";
                if (!Regex.IsMatch(_ucBasicInfo.CustomerContactNumber, phonePattern))
                {
                    MessageBox.Show("Invalid Phone Number. Use format: 09123456789", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            if (_currentStep == 1)
            {
                if (string.IsNullOrWhiteSpace(_ucAddressInfo.CustomerAddress))
                {
                    MessageBox.Show("Address cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            if (_currentStep == 2)
            {
                string selectedIdType = _ucIdentityVerification.CustomerIDType;
                string inputIdNumber = _ucIdentityVerification.CustomerIDNumber;

                if (string.IsNullOrWhiteSpace(selectedIdType))
                {
                    MessageBox.Show("Please select an ID Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(inputIdNumber))
                {
                    MessageBox.Show("ID Number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!ValidateID(selectedIdType, inputIdNumber))
                {
                    MessageBox.Show($"Invalid format for {selectedIdType}. Please check the number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
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
            _currentPage = UiHelpers.ShowPage(panelMainRegister, _ucBasicInfo, _currentPage);
        }

        public void ShowAddressInfo()
        {
            _currentPage = UiHelpers.ShowPage(panelMainRegister, _ucAddressInfo, _currentPage);
        }

        public void ShowIdentityVerification()
        {
            _currentPage = UiHelpers.ShowPage(panelMainRegister, _ucIdentityVerification, _currentPage);
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
            try
            {
                VaultLinkBankSystem.Customer testCustomer = new VaultLinkBankSystem.Customer
                {
                    CustomerCode = customerRepo.GenerateCustomerCode(),
                    FullName = _ucBasicInfo.CustomerName,
                    Address = _ucAddressInfo.CustomerAddress,
                    Email = _ucBasicInfo.CustomerEmail,
                    Phone = _ucBasicInfo.CustomerContactNumber,
                    Gender = _ucBasicInfo.CustomerGender,
                    BirthDate = _ucBasicInfo.CustomerBirthDate,
                    CivilStatus = _ucBasicInfo.CustomerCivilStatus,
                    ImagePath = imagePath ?? "john.jpg",
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

                MessageBox.Show(
                    $"Registration Successful!\n\nCustomer Code: {testCustomer.CustomerCode}",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Error");
            }
        }

        private void frmRegistration_Load_1(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Customer Photo";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        imagePath = ofd.FileName;

                        using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                        {
                            pbCustomerImage.Image = System.Drawing.Image.FromStream(stream);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Invalid image format or file error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
