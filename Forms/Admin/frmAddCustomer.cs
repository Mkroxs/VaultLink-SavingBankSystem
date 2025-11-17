using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaultLinkBankSystem.Forms.Admin
{
    public partial class frmAddCustomer : Form
    {
        CustomerRepository customerRepo = new CustomerRepository();
        public frmAddCustomer()
        {
            InitializeComponent();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                string gender = rbtMale.Checked ? "Male" : "Female";
                Customer newCustomer = new Customer
                {
                    CustomerCode = customerRepo.GenerateCustomerCode(),
                    FullName = txtFullName.Text,
                    Address = txtAddress.Text,
                    Email = txtEmail.Text,
                    Phone = txtPhoneNumber.Text,
                    Gender = gender,
                    BirthDate = dtpBirthDate.Value,
                    CivilStatus = cmbCivilStatus.SelectedItem.ToString(),
                    ImagePath = "john.png",
                    PIN = customerRepo.GeneratePIN(),
                    EmploymentStatus = cmbEmploymentStatus.SelectedItem.ToString(),
                    EmployerName = txtEmployerName.Text,
                    SourceOfFunds = cmbSourceOfFunds.Text,
                    MonthlyIncomeRange = cmbMonthlyIncomeRange.SelectedItem.ToString(),

                    IDType = cmbIDType.SelectedItem.ToString(),
                    IDNumber = txtIDNumber.Text,

                    IsKYCVerified = false,
                    KYCVerifiedDate = null
                };
                int customerId = customerRepo.CreateCustomer(newCustomer);

                Console.WriteLine("✅ Customer Registered Successfully!");
                Console.WriteLine("=====================================");
                Console.WriteLine($"Customer ID: {customerId}");
                Console.WriteLine($"Customer Code: {newCustomer.CustomerCode}");
                Console.WriteLine($"Full Name: {newCustomer.FullName}");
                Console.WriteLine($"Email: {newCustomer.Email}");
                Console.WriteLine($"PIN (for kiosk): {newCustomer.PIN}");
                Console.WriteLine($"KYC Status: {(newCustomer.IsKYCVerified ? "Verified ✅" : "Pending Verification ⏳")}");
                Console.WriteLine("=====================================");
                Console.WriteLine("\n⚠️ Customer cannot create accounts until KYC is verified by admin.");
            
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbSourceOfFunds_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
