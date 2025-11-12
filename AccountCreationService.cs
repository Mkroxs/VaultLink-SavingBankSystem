using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace VaultLinkBankSystem
{
    public class AccountCreationService
    {
        private readonly AccountRepository _accountRepo;
        private readonly CustomerKYCRepository _kycRepo;
        private readonly string _connectionString;

        public AccountCreationService()
        {
            _accountRepo = new AccountRepository();
            _kycRepo = new CustomerKYCRepository();
            _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\JB\Source\Repos\VaultLink-SavingBankSystem\Banking.mdf;Integrated Security=True";
        }

        // ✅ Create Account with KYC in one transaction + InterestRateID
        public int CreateAccountWithKYC(int customerId, decimal initialBalance, CustomerKYC kycInfo, int interestRateId)
        {
            try
            {
                if (initialBalance < 0)
                    throw new Exception("Initial balance cannot be negative.");

                // Step 1: Create account
                Account newAccount = new Account
                {
                    CustomerID = customerId,
                    InterestRateID = interestRateId, // ✅ use latest InterestRateID dynamically
                    AccountNumber = _accountRepo.GenerateAccountNumber(),
                    Balance = initialBalance,
                    Status = "Active",
                    DateOpened = DateTime.Now
                };

                int accountId = _accountRepo.CreateAccount(newAccount);
                if (accountId <= 0)
                    throw new Exception("Failed to create account.");

                // Step 2: Create KYC record linked to the account
                kycInfo.AccountID = accountId;
                kycInfo.CustomerID = customerId;

                int kycId = _kycRepo.CreateKYC(kycInfo);
                if (kycId <= 0)
                    throw new Exception("Failed to create KYC record.");

                return accountId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in account creation: " + ex.Message);
            }
        }

        // ✅ Get all accounts for a customer with KYC info
        public List<AccountWithKYC> GetCustomerAccountsWithKYC(int customerId)
        {
            List<AccountWithKYC> accountsWithKYC = new List<AccountWithKYC>();

            try
            {
                List<Account> accounts = _accountRepo.GetAccountsByCustomerId(customerId);

                foreach (var account in accounts)
                {
                    var accountWithKYC = _accountRepo.GetAccountWithKYC(account.AccountID);
                    if (accountWithKYC != null)
                        accountsWithKYC.Add(accountWithKYC);
                }

                return accountsWithKYC;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting customer accounts: " + ex.Message);
            }
        }

        // ✅ Check if customer can create more accounts
        public bool CanCreateAccount(int customerId, int maxAccountsPerCustomer = 5)
        {
            try
            {
                int accountCount = _accountRepo.GetAccountCountByCustomerId(customerId);
                return accountCount < maxAccountsPerCustomer;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking account limit: " + ex.Message);
            }
        }

        // ✅ Get the most recent InterestRateID
        public int GetLatestInterestRateId()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 InterestRateID FROM InterestRates ORDER BY EffectiveDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();

                if (result != null)
                    return Convert.ToInt32(result);
                else
                    throw new Exception("No Interest Rate found in database.");
            }
        }
    }
}
