using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLinkBankSystem
{
    public class CustomerKYCRepository
    {
        private readonly string _connectionString;

        public CustomerKYCRepository()
        {
            _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\JB\Source\Repos\VaultLink-SavingBankSystem\Banking.mdf;Integrated Security=True";
        }

        // Create KYC record for an account
        public int CreateKYC(CustomerKYC kyc)
        {
            string query = @"INSERT INTO CustomerKYC (AccountID, CustomerID, EmploymentStatus, EmployerName, 
                           SourceOfFunds, MonthlyIncomeRange, AccountPurpose, IDType, IDNumber, CreatedAt)
                           VALUES (@AccountID, @CustomerID, @EmploymentStatus, @EmployerName, 
                           @SourceOfFunds, @MonthlyIncomeRange, @AccountPurpose, @IDType, @IDNumber, @CreatedAt);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", kyc.AccountID);
                    cmd.Parameters.AddWithValue("@CustomerID", kyc.CustomerID);
                    cmd.Parameters.AddWithValue("@EmploymentStatus", kyc.EmploymentStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployerName", kyc.EmployerName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SourceOfFunds", kyc.SourceOfFunds ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MonthlyIncomeRange", kyc.MonthlyIncomeRange ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountPurpose", kyc.AccountPurpose ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IDType", kyc.IDType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IDNumber", kyc.IDNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    conn.Open();
                    int kycId = (int)cmd.ExecuteScalar();
                    return kycId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating KYC record: " + ex.Message);
            }
        }

        // Get KYC by Account ID
        public CustomerKYC GetKYCByAccountId(int accountId)
        {
            string query = "SELECT * FROM CustomerKYC WHERE AccountID = @AccountID";

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
                            return MapKYCFromReader(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting KYC record: " + ex.Message);
            }

            return null;
        }

        // Update KYC information
        public bool UpdateKYC(CustomerKYC kyc)
        {
            string query = @"UPDATE CustomerKYC 
                           SET EmploymentStatus = @EmploymentStatus,
                               EmployerName = @EmployerName,
                               SourceOfFunds = @SourceOfFunds,
                               MonthlyIncomeRange = @MonthlyIncomeRange,
                               AccountPurpose = @AccountPurpose,
                               IDType = @IDType,
                               IDNumber = @IDNumber
                           WHERE AccountID = @AccountID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", kyc.AccountID);
                    cmd.Parameters.AddWithValue("@EmploymentStatus", kyc.EmploymentStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployerName", kyc.EmployerName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SourceOfFunds", kyc.SourceOfFunds ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MonthlyIncomeRange", kyc.MonthlyIncomeRange ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AccountPurpose", kyc.AccountPurpose ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IDType", kyc.IDType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IDNumber", kyc.IDNumber ?? (object)DBNull.Value);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating KYC record: " + ex.Message);
            }
        }

        // Check if account has KYC
        public bool HasKYC(int accountId)
        {
            string query = "SELECT COUNT(*) FROM CustomerKYC WHERE AccountID = @AccountID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking KYC record: " + ex.Message);
            }
        }

        // Delete KYC record
        public bool DeleteKYC(int accountId)
        {
            string query = "DELETE FROM CustomerKYC WHERE AccountID = @AccountID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting KYC record: " + ex.Message);
            }
        }

        private CustomerKYC MapKYCFromReader(SqlDataReader reader)
        {
            return new CustomerKYC
            {
                KYCID = (int)reader["KYCID"],
                AccountID = (int)reader["AccountID"],
                CustomerID = (int)reader["CustomerID"],
                EmploymentStatus = reader["EmploymentStatus"] != DBNull.Value ? reader["EmploymentStatus"].ToString() : null,
                EmployerName = reader["EmployerName"] != DBNull.Value ? reader["EmployerName"].ToString() : null,
                SourceOfFunds = reader["SourceOfFunds"] != DBNull.Value ? reader["SourceOfFunds"].ToString() : null,
                MonthlyIncomeRange = reader["MonthlyIncomeRange"] != DBNull.Value ? reader["MonthlyIncomeRange"].ToString() : null,
                AccountPurpose = reader["AccountPurpose"] != DBNull.Value ? reader["AccountPurpose"].ToString() : null,
                IDType = reader["IDType"] != DBNull.Value ? reader["IDType"].ToString() : null,
                IDNumber = reader["IDNumber"] != DBNull.Value ? reader["IDNumber"].ToString() : null,
                CreatedAt = (DateTime)reader["CreatedAt"]
            };
        }
    }
}
