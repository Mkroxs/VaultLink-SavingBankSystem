using System;
using System.Collections.Generic;
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
            _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\JB\Source\Repos\VaultLink-SavingBankSystem\Banking.mdf;Integrated Security=True";
        }
        private int GetLatestInterestRateId()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 InterestRateID FROM InterestRates ORDER BY EffectiveDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        return Convert.ToInt32(result);
                    else
                        throw new Exception("No interest rate records found.");
                }
            }
        }
        // Create new account
        public int CreateAccount(Account account)
        {
            string query = @"INSERT INTO Accounts (CustomerID, InterestRateID, AccountNumber, Balance, Status, DateOpened)
                     VALUES (@CustomerID, @InterestRateID, @AccountNumber, @Balance, @Status, @DateOpened);
                     SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // 🔹 Always get the latest interest rate
                    int latestInterestRateId = GetLatestInterestRateId();

                    cmd.Parameters.AddWithValue("@CustomerID", account.CustomerID);
                    cmd.Parameters.AddWithValue("@InterestRateID", latestInterestRateId);
                    cmd.Parameters.AddWithValue("@AccountNumber", account.AccountNumber);
                    cmd.Parameters.AddWithValue("@Balance", account.Balance);
                    cmd.Parameters.AddWithValue("@Status", account.Status);
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

        // Get account by ID
        public Account GetAccountById(int accountId)
        {
            string query = "SELECT * FROM Accounts WHERE AccountID = @AccountID";

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
                            return MapAccountFromReader(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting account: " + ex.Message);
            }

            return null;
        }

        // Get account by account number
        public Account GetAccountByNumber(string accountNumber)
        {
            string query = "SELECT * FROM Accounts WHERE AccountNumber = @AccountNumber";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapAccountFromReader(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting account by number: " + ex.Message);
            }

            return null;
        }

        // Get all accounts for a customer
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
                throw new Exception("Error getting customer accounts: " + ex.Message);
            }

            return accounts;
        }

        // Get account with KYC information
        public AccountWithKYC GetAccountWithKYC(int accountId)
        {
            string query = @"SELECT a.*, c.FullName, k.*
                           FROM Accounts a
                           INNER JOIN Customers c ON a.CustomerID = c.CustomerID
                           LEFT JOIN CustomerKYC k ON a.AccountID = k.AccountID
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
                            return new AccountWithKYC
                            {
                                AccountID = (int)reader["AccountID"],
                                AccountNumber = reader["AccountNumber"].ToString(),
                                Balance = (decimal)reader["Balance"],
                                Status = reader["Status"].ToString(),
                                DateOpened = (DateTime)reader["DateOpened"],
                                CustomerID = (int)reader["CustomerID"],
                                CustomerName = reader["FullName"].ToString(),
                                KYCID = reader["KYCID"] != DBNull.Value ? (int?)reader["KYCID"] : null,
                                EmploymentStatus = reader["EmploymentStatus"] != DBNull.Value ? reader["EmploymentStatus"].ToString() : null,
                                EmployerName = reader["EmployerName"] != DBNull.Value ? reader["EmployerName"].ToString() : null,
                                SourceOfFunds = reader["SourceOfFunds"] != DBNull.Value ? reader["SourceOfFunds"].ToString() : null,
                                MonthlyIncomeRange = reader["MonthlyIncomeRange"] != DBNull.Value ? reader["MonthlyIncomeRange"].ToString() : null,
                                AccountPurpose = reader["AccountPurpose"] != DBNull.Value ? reader["AccountPurpose"].ToString() : null,
                                IDType = reader["IDType"] != DBNull.Value ? reader["IDType"].ToString() : null,
                                IDNumber = reader["IDNumber"] != DBNull.Value ? reader["IDNumber"].ToString() : null
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting account with KYC: " + ex.Message);
            }

            return null;
        }

        // Update account balance
        public bool UpdateBalance(int accountId, decimal newBalance)
        {
            string query = "UPDATE Accounts SET Balance = @Balance WHERE AccountID = @AccountID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@Balance", newBalance);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating balance: " + ex.Message);
            }
        }

        // Close account
        public bool CloseAccount(int accountId)
        {
            string query = "UPDATE Accounts SET Status = 'Closed', ClosedDate = @ClosedDate WHERE AccountID = @AccountID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@ClosedDate", DateTime.Now);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error closing account: " + ex.Message);
            }
        }

        // Generate unique account number
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
                    return $"{prefix}{nextId:D4}"; // ACC-2025-0001
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating account number: " + ex.Message);
            }
        }

        // Get all active accounts
        public List<Account> GetAllActiveAccounts()
        {
            List<Account> accounts = new List<Account>();
            string query = "SELECT * FROM Accounts WHERE Status = 'Active' ORDER BY DateOpened DESC";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
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
                throw new Exception("Error getting active accounts: " + ex.Message);
            }

            return accounts;
        }

        // Get account count for customer
        public int GetAccountCountByCustomerId(int customerId)
        {
            string query = "SELECT COUNT(*) FROM Accounts WHERE CustomerID = @CustomerID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting account count: " + ex.Message);
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
                Balance = (decimal)reader["Balance"],
                Status = reader["Status"].ToString(),
                DateOpened = (DateTime)reader["DateOpened"],
                ClosedDate = reader["ClosedDate"] != DBNull.Value ? (DateTime?)reader["ClosedDate"] : null
            };
        }
    }
}


