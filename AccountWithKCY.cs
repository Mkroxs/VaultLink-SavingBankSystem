using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLinkBankSystem
{
    public class AccountWithKYC
    {
        // Account Information
        public int AccountID { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; }
        public DateTime DateOpened { get; set; }

        // Customer Information
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }

        // KYC Information
        public int? KYCID { get; set; }
        public string EmploymentStatus { get; set; }
        public string EmployerName { get; set; }
        public string SourceOfFunds { get; set; }
        public string MonthlyIncomeRange { get; set; }
        public string AccountPurpose { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }
    }
}
