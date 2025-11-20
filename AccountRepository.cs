using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLinkBankSystem
{
    public class AccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["BankingDB"].ConnectionString;
        }

        // Create account (much simpler now - no KYC needed!)
        public int CreateAccount(int customerId, decimal initialBalance, string accountType = "Savings")
        {
            // Check if customer exists and has verified KYC
            CustomerRepository customerRepo = new CustomerRepository();
            Customer customer = customerRepo.GetCustomerById(customerId);

            if (customer == null)
            {
                throw new Exception("Customer not found.");
            }

            if (!customer.IsKYCVerified)
            {
                throw new Exception("Customer KYC is not verified. Please complete KYC verification first.");
            }

            string query = @"INSERT INTO Accounts (CustomerID, InterestRateID, AccountNumber, AccountType, Balance, Status, DateOpened)
                           VALUES (@CustomerID, @InterestRateID, @AccountNumber, @AccountType, @Balance, 'Active', @DateOpened);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@InterestRateID", 3); // Current interest rate
                    cmd.Parameters.AddWithValue("@AccountNumber", GenerateAccountNumber());
                    cmd.Parameters.AddWithValue("@AccountType", accountType);
                    cmd.Parameters.AddWithValue("@Balance", initialBalance);
                    cmd.Parameters.AddWithValue("@DateOpened", DateTime.Now);

                    conn.Open();
                    int accountId = (int)cmd.ExecuteScalar();
                    return accountId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating account: " + ex.Message);
            }
        }

        // Get all accounts for customer
        public List<Account> GetAccountsByCustomerId(int customerId)
        {
            List<Account> accounts = new List<Account>();
            string query = "SELECT * FROM Accounts WHERE CustomerID = @CustomerID ORDER BY DateOpened DESC";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accounts.Add(MapAccountFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting accounts: " + ex.Message);
            }

            return accounts;
        }

        // Get account with customer info
        public AccountWithCustomer GetAccountWithCustomer(int accountId)
        {
            string query = @"SELECT a.*, c.FullName, c.CustomerCode, c.Email, c.Phone
                           FROM Accounts a
                           INNER JOIN Customers c ON a.CustomerID = c.CustomerID
                           WHERE a.AccountID = @AccountID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new AccountWithCustomer
                            {
                                AccountID = (int)reader["AccountID"],
                                AccountNumber = reader["AccountNumber"].ToString(),
                                AccountType = reader["AccountType"].ToString(),
                                Balance = (decimal)reader["Balance"],
                                Status = reader["Status"].ToString(),
                                DateOpened = (DateTime)reader["DateOpened"],
                                CustomerID = (int)reader["CustomerID"],
                                CustomerName = reader["FullName"].ToString(),
                                CustomerCode = reader["CustomerCode"].ToString(),
                                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                                Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : null
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting account with customer: " + ex.Message);
            }

            return null;
        }

        public string GenerateAccountNumber()
        {
            string prefix = "ACC-" + DateTime.Now.Year + "-";
            string query = "SELECT MAX(AccountID) FROM Accounts";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    int nextId = (result != DBNull.Value && result != null) ? Convert.ToInt32(result) + 1 : 1;
                    return $"{prefix}{nextId:D4}";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating account number: " + ex.Message);
            }
        }

        private Account MapAccountFromReader(SqlDataReader reader)
        {
            return new Account
            {
                AccountID = (int)reader["AccountID"],
                CustomerID = (int)reader["CustomerID"],
                InterestRateID = reader["InterestRateID"] != DBNull.Value ? (int?)reader["InterestRateID"] : null,
                AccountNumber = reader["AccountNumber"].ToString(),
                AccountType = reader["AccountType"].ToString(),
                Balance = (decimal)reader["Balance"],
                Status = reader["Status"].ToString(),
                DateOpened = (DateTime)reader["DateOpened"],
                ClosedDate = reader["ClosedDate"] != DBNull.Value ? (DateTime?)reader["ClosedDate"] : null
            };
        }
    }
}



