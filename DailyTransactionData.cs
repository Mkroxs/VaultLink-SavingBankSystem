using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLinkBankSystem
{
    public class DailyTransactionData
    {
        public DateTime Date { get; set; }
        public string DateLabel { get; set; }
        public int DepositCount { get; set; }
        public int WithdrawalCount { get; set; }
    }
}
