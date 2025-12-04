using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLinkBankSystem
{
    public class Account
    {
        public int AccountID { get; set; }
        public int CustomerID { get; set; }
        public int? InterestRateID { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime? ClosedDate { get; set; }


        public decimal? InterestEarned { get; set; }

    }
}
