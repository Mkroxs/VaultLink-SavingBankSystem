using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLinkBankSystem
{
    public class TransactionWithCustomer
    {
        public Transaction Transaction { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
    }

}
