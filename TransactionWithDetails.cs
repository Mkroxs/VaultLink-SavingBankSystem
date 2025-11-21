using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLinkBankSystem
{
    public class TransactionWithDetails
    {
        public int TransactionID { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal NewBalance { get; set; }
        public string Remarks { get; set; }

        // Account Info
        public string AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
    }
}
