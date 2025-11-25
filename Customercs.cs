using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLinkBankSystem
{
    public class Customers
    {
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string CivilStatus { get; set; }
        public string ImagePath { get; set; }
        public string PIN { get; set; }

        // KYC Information (Now part of customer)
        public string EmploymentStatus { get; set; }
        public string EmployerName { get; set; }
        public string SourceOfFunds { get; set; }
        public string MonthlyIncomeRange { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }

        // System Fields
        public bool IsKYCVerified { get; set; }
        public DateTime? KYCVerifiedDate { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
