using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLinkBankSystem
{
    public class TransactionRepository
    {
        private readonly string _connectionString;
        private readonly AccountRepository _accountRepo;
        public TransactionRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["BankingDB"].ConnectionString;
            _accountRepo = new AccountRepository();
        }

        // ============================================
        // DEPOSIT MONEY
        // ============================================
        public Transaction Deposit(int accountId, decimal amount, string remarks = "")
        {
            if (amount <= 0)
            {
                throw new Exception("Deposit amount must be greater than zero.");
            }

            // Get current account balance
            decimal currentBalance = GetAccountBalance(accountId);

            // Calculate new balance
            decimal newBalance = currentBalance + amount;

            // Create transaction record
            Transaction transaction = new Transaction
            {
                AccountID = accountId,
                TransactionDate = DateTime.Now,
                TransactionType = "Deposit",
                Amount = amount,
                PreviousBalance = currentBalance,
                NewBalance = newBalance,
                Remarks = string.IsNullOrEmpty(remarks) ? "Deposit" : remarks
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Start transaction for data consistency
                    using (SqlTransaction sqlTransaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Insert transaction record
                            string insertQuery = @"INSERT INTO Transactions (AccountID, TransactionDate, TransactionType, Amount, PreviousBalance, NewBalance, Remarks)
                                                 VALUES (@AccountID, @TransactionDate, @TransactionType, @Amount, @PreviousBalance, @NewBalance, @Remarks);
                                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            using (SqlCommand cmd = new SqlCommand(insertQuery, conn, sqlTransaction))
                            {
                                cmd.Parameters.AddWithValue("@AccountID", transaction.AccountID);
                                cmd.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);
                                cmd.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);
                                cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
                                cmd.Parameters.AddWithValue("@PreviousBalance", transaction.PreviousBalance);
                                cmd.Parameters.AddWithValue("@NewBalance", transaction.NewBalance);
                                cmd.Parameters.AddWithValue("@Remarks", transaction.Remarks);

                                transaction.TransactionID = (int)cmd.ExecuteScalar();
                            }

                            // 2. Update account balance
                            string updateQuery = "UPDATE Accounts SET Balance = @NewBalance WHERE AccountID = @AccountID";

                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, sqlTransaction))
                            {
                                cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                                cmd.Parameters.AddWithValue("@AccountID", accountId);
                                cmd.ExecuteNonQuery();
                            }   

                            // Commit transaction
                            sqlTransaction.Commit();

                            return transaction;
                        }
                        catch (Exception)
                        {
                            sqlTransaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error processing deposit: " + ex.Message);
            }
        }

        // ============================================
        // WITHDRAW MONEY
        // ============================================
        public Transaction Withdraw(int accountId, decimal amount, string remarks = "")
        {
            if (amount <= 0)
            {
                throw new Exception("Withdrawal amount must be greater than zero.");
            }

            // Get current account balance
            decimal currentBalance = GetAccountBalance(accountId);

            // Check if sufficient balance
            if (amount > currentBalance)
            {
                throw new Exception($"Insufficient balance. Available: {currentBalance:C2}, Requested: {amount:C2}");
            }

            // Calculate new balance
            decimal newBalance = currentBalance - amount;

            // Create transaction record
            Transaction transaction = new Transaction
            {
                AccountID = accountId,
                TransactionDate = DateTime.Now,
                TransactionType = "Withdrawal",
                Amount = amount,
                PreviousBalance = currentBalance,
                NewBalance = newBalance,
                Remarks = string.IsNullOrEmpty(remarks) ? "Withdrawal" : remarks
            };

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Using SQL Transaction to ensure both database changes occur or none do
                    using (SqlTransaction sqlTransaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Insert transaction record
                            string insertQuery = @"INSERT INTO Transactions (AccountID, TransactionDate, TransactionType, Amount, PreviousBalance, NewBalance, Remarks)
                                                 VALUES (@AccountID, @TransactionDate, @TransactionType, @Amount, @PreviousBalance, @NewBalance, @Remarks);
                                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            using (SqlCommand cmd = new SqlCommand(insertQuery, conn, sqlTransaction))
                            {
                                cmd.Parameters.AddWithValue("@AccountID", transaction.AccountID);
                                cmd.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);
                                cmd.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);
                                cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
                                cmd.Parameters.AddWithValue("@PreviousBalance", transaction.PreviousBalance);
                                cmd.Parameters.AddWithValue("@NewBalance", transaction.NewBalance);
                                cmd.Parameters.AddWithValue("@Remarks", transaction.Remarks);

                                transaction.TransactionID = (int)cmd.ExecuteScalar();
                            }

                            // 2. Update account balance 
                            // Using the direct SQL logic inside the transaction scope, similar to Deposit, 
                            // to ensure Atomicity.
                            string updateQuery = "UPDATE Accounts SET Balance = @NewBalance WHERE AccountID = @AccountID";

                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, sqlTransaction))
                            {
                                cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                                cmd.Parameters.AddWithValue("@AccountID", accountId);
                                cmd.ExecuteNonQuery();
                            }

                            // Commit transaction
                            sqlTransaction.Commit();

                            return transaction;
                        }
                        catch (Exception)
                        {
                            sqlTransaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error processing withdrawal: " + ex.Message);
            }
            
        }

        // ============================================
        // GET ACCOUNT BALANCE
        // ============================================
        private decimal GetAccountBalance(int accountId)
        {
            string query = "SELECT Balance FROM Accounts WHERE AccountID = @AccountID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        throw new Exception("Account not found.");
                    }

                    return (decimal)result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting account balance: " + ex.Message);
            }
        }

        // ============================================
        // GET TRANSACTIONS BY ACCOUNT
        // ============================================
        public List<Transaction> GetTransactionsByAccountId(int accountId)
        {
            List<Transaction> transactions = new List<Transaction>();
            string query = "SELECT * FROM Transactions WHERE AccountID = @AccountID ORDER BY TransactionDate DESC";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(MapTransactionFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting transactions: " + ex.Message);
            }

            return transactions;
        }

        // ============================================
        // GET RECENT TRANSACTIONS (LAST N)
        // ============================================
        public List<Transaction> GetRecentTransactions(int accountId, int count = 10)
        {
            List<Transaction> transactions = new List<Transaction>();
            string query = $"SELECT TOP {count} * FROM Transactions WHERE AccountID = @AccountID ORDER BY TransactionDate DESC";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(MapTransactionFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting recent transactions: " + ex.Message);
            }

            return transactions;
        }

        // ============================================
        // GET TRANSACTIONS BY DATE RANGE
        // ============================================
        public List<Transaction> GetTransactionsByDateRange(int accountId, DateTime startDate, DateTime endDate)
        {
            List<Transaction> transactions = new List<Transaction>();
            string query = @"SELECT * FROM Transactions 
                           WHERE AccountID = @AccountID 
                           AND TransactionDate BETWEEN @StartDate AND @EndDate 
                           ORDER BY TransactionDate DESC";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(MapTransactionFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting transactions by date range: " + ex.Message);
            }

            return transactions;
        }

        // ============================================
        // GET TRANSACTION SUMMARY
        // ============================================
        public Dictionary<string, decimal> GetTransactionSummary(int accountId)
        {
            Dictionary<string, decimal> summary = new Dictionary<string, decimal>();

            string query = @"SELECT 
                               TransactionType,
                               COUNT(*) AS TransactionCount,
                               SUM(Amount) AS TotalAmount
                           FROM Transactions
                           WHERE AccountID = @AccountID
                           GROUP BY TransactionType";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string type = reader["TransactionType"].ToString();
                            decimal total = (decimal)reader["TotalAmount"];
                            summary[type] = total;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting transaction summary: " + ex.Message);
            }

            return summary;
        }

        private Transaction MapTransactionFromReader(SqlDataReader reader)
        {
            return new Transaction
            {
                TransactionID = (int)reader["TransactionID"],
                AccountID = (int)reader["AccountID"],
                TransactionDate = (DateTime)reader["TransactionDate"],
                TransactionType = reader["TransactionType"].ToString(),
                Amount = (decimal)reader["Amount"],
                PreviousBalance = (decimal)reader["PreviousBalance"],
                NewBalance = (decimal)reader["NewBalance"],
                Remarks = reader["Remarks"] != DBNull.Value ? reader["Remarks"].ToString() : ""
            };
        }
    }
}
