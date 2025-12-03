using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLinkBankSystem
{
    public class TransactionWithAccount
    {
        public Transaction Transaction { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
    }
}
