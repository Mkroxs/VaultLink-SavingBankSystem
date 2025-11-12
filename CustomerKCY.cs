using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLinkBankSystem
{
    public class CustomerKYC
    {
        public int KYCID { get; set; }
        public int AccountID { get; set; }
        public int CustomerID { get; set; }
        public string EmploymentStatus { get; set; }
        public string EmployerName { get; set; }
        public string SourceOfFunds { get; set; }
        public string MonthlyIncomeRange { get; set; }
        public string AccountPurpose { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
