using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VaultLinkBankSystem
{
    public partial class frmDashBoard : Form
    {
        CustomerRepository customerRepository = new CustomerRepository();
        Customer newCustomer;
        private int _custId;
        public frmDashBoard(Admin admin)
        {
            InitializeComponent();

        }

        private void frmDashBoard_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            newCustomer = new Customer
            {
                CustomerCode = "CUST010",
                FullName = "Mike",
                Address = "Manila",
                Email = "john@example.com",
                Phone = "09123456789",
                Gender = "Male",
                BirthDate = new DateTime(2000, 1, 1),
                CivilStatus = "Single",
                ImagePath = "images/john.png"
            };
            int num = customerRepository.CreateCustomer(newCustomer);
            if (num > 0)
            {
                MessageBox.Show("Customer created successfully!");
            }
            else
            {
                MessageBox.Show("Failed to create customer.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            string customerCode = txtCustId.Text;

            Customer ifFound = customerRepository.GetCustomerByCode(customerCode);
            if(ifFound != null)
            {
                lblCustAccountNum.Text = ifFound.FullName;
            }
        }

        private void btnCreateSavingsAcc_Click(object sender, EventArgs e)
        {
            string num2 = txtCustIDforSavingAccCreation.Text;

            // Safety check for conversion (optional, but good practice)
            if (!int.TryParse(num2, out int _custId))
            {
                MessageBox.Show("Please enter a valid Customer ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 1. Create the form object
            frmSavingAccountCreation frmSAC = new frmSavingAccountCreation(_custId);
            frmSAC.Show();
        }

        private void txtCustIDforSavingAccCreation_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
