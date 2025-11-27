using iText.IO.Image;
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
            // 1. Clean the input: Remove spaces and dashes for easier checking
            // Example: User types "123-456", we turn it into "123456"
            string cleanId = idNumber.Replace("-", "").Replace(" ", "").Trim();

            string pattern = "";

            switch (idType)
            {
                case "National ID":
                    // PhilSys is 16 digits
                    pattern = @"^\d{16}$";
                    break;

                case "Passport":
                    // New PH Passports: Letter + 8 digits (e.g., P12345678)
                    // Older ones might vary, but this covers the standard 10-year passport.
                    pattern = @"^[A-Z]\d{8}$";
                    break;

                case "Driver’s License":
                    // Format: L02-12-123456. Cleaned: L0212123456 (1 Letter + 10 digits)
                    pattern = @"^[A-Z]\d{10}$";
                    break;

                case "UMID":
                    // Unified Multi-Purpose ID (12 digits)
                    pattern = @"^\d{12}$";
                    break;

                case "SSS ID":
                    // Social Security System (10 digits)
                    pattern = @"^\d{10}$";
                    break;

                case "GSIS ID":
                    // GSIS BP Number (10 to 12 digits)
                    pattern = @"^\d{10,12}$";
                    break;

                case "TIN ID":
                    // Tax Identification Number (9 to 12 digits including branch)
                    pattern = @"^\d{9,12}$";
                    break;

                case "Postal ID":
                    // New Postal ID (2 Letters + 9 Digits) OR Old (7-9 digits)
                    pattern = @"^([A-Z]{2}\d{9}|\d{7,9})$";
                    break;

                case "PRC License":
                    // Professional Regulation Commission (7 digits)
                    pattern = @"^\d{7}$";
                    break;

                case "Voter’s ID / COMELEC ID":
                    // Voter IDs are messy. We ensure it's alphanumeric and has length.
                    pattern = @"^[A-Z0-9]{15,25}$";
                    break;

                // LOOSE VALIDATION FOR NON-STANDARDIZED IDS
                // PWD, Senior Citizen, Student, and Company IDs vary wildly by City or School.
                // Strategy: Just ensure they aren't empty and don't contain symbols like @#$%.
                case "PWD ID":
                case "Senior Citizen ID":
                case "Student ID":
                case "Company ID":
                    pattern = @"^[a-zA-Z0-9]{4,20}$";
                    break;

                default:
                    // If they pick something weird, default to alphanumeric check
                    pattern = @"^[a-zA-Z0-9]{4,20}$";
                    break;
            }

            // Run the validation
            if (!Regex.IsMatch(cleanId, pattern))
            {
                return false;
            }

            return true;
        }

        private bool ValidateCurrentStep()
        {
            // STEP 0: BASIC INFO VALIDATION
            if (_currentStep == 0)
            {
                // 1. Validate Name (Letters and spaces only, no numbers)
                // Regex: ^[a-zA-Z\s]+$ means start to end only letters and whitespace
                if (!Regex.IsMatch(_ucBasicInfo.CustomerName, @"^[a-zA-Z\s\.\-]+$"))
                {
                    MessageBox.Show("Invalid Name. Please use letters only.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // 2. Validate Email
                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(_ucBasicInfo.CustomerEmail, emailPattern))
                {
                    MessageBox.Show("Invalid Email Address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // 3. Validate Phone (PH Format: 09xxxxxxxxx or +639xxxxxxxxx)
                // Regex logic: Matches 09 followed by 9 digits OR +639 followed by 9 digits
                string phonePattern = @"^(09|\+639)\d{9}$";
                if (!Regex.IsMatch(_ucBasicInfo.CustomerContactNumber, phonePattern))
                {
                    MessageBox.Show("Invalid Phone Number. Use format: 09123456789", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // STEP 1: ADDRESS VALIDATION
            if (_currentStep == 1)
            {
                if (string.IsNullOrWhiteSpace(_ucAddressInfo.CustomerAddress))
                {
                    MessageBox.Show("Address cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // STEP 2: IDENTITY VALIDATION
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

                // CALL THE NEW FUNCTION
                if (!ValidateID(selectedIdType, inputIdNumber))
                {
                    MessageBox.Show($"Invalid format for {selectedIdType}.\nPlease check the number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true; // All checks passed
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
            // 1. Final Validation Check
            if (!ValidateCurrentStep()) return;

            // 2. Admin Confirmation Dialog
            DialogResult dr = MessageBox.Show(
                "Are you sure you want to register this customer?\n\nPlease confirm all details are correct.",
                "Admin Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    // 3. Prepare the object
                    Customers testCustomer = new Customers()
                    {
                        CustomerCode = customerRepo.GenerateCustomerCode(),
                        FullName = _ucBasicInfo.CustomerName, // Ensure these properties exist in your UC
                        Address = _ucAddressInfo.CustomerAddress,
                        Email = _ucBasicInfo.CustomerEmail,
                        Phone = _ucBasicInfo.CustomerContactNumber,
                        Gender = _ucBasicInfo.CustomerGender,
                        BirthDate = _ucBasicInfo.CustomerBirthDate,
                        CivilStatus = _ucBasicInfo.CustomerCivilStatus,

                        // Save the path, OR save the binary data. 
                        // Ideally, copy the image to a specific "Images" folder in your project directory
                        ImagePath = imagePath ?? "default.jpg",

                        PIN = customerRepo.GeneratePIN(), // See Security Tip below!
                        EmploymentStatus = _ucIdentityVerification.CustomerEmploymentStatus,
                        EmployerName = "Elon Musk", // hardcoded? Make sure to map this to a textbox!
                        SourceOfFunds = _ucIdentityVerification.CustomerSourceOfFunds,
                        MonthlyIncomeRange = _ucIdentityVerification.CustomerMonthlyIncome,
                        IDType = _ucIdentityVerification.CustomerIDType,
                        IDNumber = _ucIdentityVerification.CustomerIDNumber,
                        IsKYCVerified = true, // Set to true since Admin is registering them personally
                        KYCVerifiedDate = DateTime.Now
                    };

                    // 4. Save to Database
                    customerRepo.CreateCustomer(testCustomer);

                    // 5. Success Message
                    MessageBox.Show($"Registration Successful!\n\nCustomer Code: {testCustomer.CustomerCode}",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 6. NOW we close the form
                    this.Close();
                }
                catch (Exception ex)
                {
                    // If DB fails, do NOT close the form. Let them try again.
                    MessageBox.Show($"System Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

                        // SECURITY FIX: Load image without locking the file
                        using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                        {
                            pbCustomerImage.Image = System.Drawing.Image.FromStream(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Invalid image format or file error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}