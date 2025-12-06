using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using VaultLinkBankSystem.Helpers;

namespace VaultLinkBankSystem
{
    public class CustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["BankingDB"].ConnectionString;
        }

        public int CreateCustomer(Customer customer, string plainPassword)
        {
            string query = @"INSERT INTO Customers (
    CustomerCode, FullName, Address, Email, Phone, Gender, BirthDate, CivilStatus, ImagePath,
    PasswordHash, PINHash,
    EmploymentStatus, SourceOfFunds, MonthlyIncomeRange, IDType, IDNumber,
    IsKYCVerified, KYCVerifiedDate, CreatedAt
)
VALUES (
    @CustomerCode, @FullName, @Address, @Email, @Phone, @Gender, @BirthDate, @CivilStatus, @ImagePath,
    @PasswordHash, @PINHash,
    @EmploymentStatus, @SourceOfFunds, @MonthlyIncomeRange, @IDType, @IDNumber,
    @IsKYCVerified, @KYCVerifiedDate, @CreatedAt
);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                string hashedPassword = PasswordHashHelper.HashPassword(plainPassword);

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerCode", customer.CustomerCode);
                    cmd.Parameters.AddWithValue("@FullName", customer.FullName);
                    cmd.Parameters.AddWithValue("@Address", customer.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", customer.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", customer.Gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BirthDate", customer.BirthDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CivilStatus", customer.CivilStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath", customer.ImagePath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    cmd.Parameters.AddWithValue("@PINHash", DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmploymentStatus", customer.EmploymentStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SourceOfFunds", customer.SourceOfFunds ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MonthlyIncomeRange", customer.MonthlyIncomeRange ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IDType", customer.IDType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IDNumber", customer.IDNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsKYCVerified", customer.IsKYCVerified);
                    cmd.Parameters.AddWithValue("@KYCVerifiedDate", customer.KYCVerifiedDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    conn.Open();
                    int customerId = (int)cmd.ExecuteScalar();
                    return customerId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating customer: " + ex.Message);
            }
        }

        public bool IsEmailExists(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(1) FROM Customers WHERE Email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public Customer KioskPasswordLogin(string email, string password)
        {
            string query = "SELECT * FROM Customers WHERE Email = @Email";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var customer = MapCustomerFromReader(reader);

                            if (!customer.IsKYCVerified)
                            {
                                throw new Exception("Account not verified. Please contact administrator.");
                            }

                            if (PasswordHashHelper.VerifyPassword(password, customer.PasswordHash))
                            {
                                return customer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login: " + ex.Message);
            }

            return null;
        }

        public bool IsFirstTimeLogin(int customerId)
        {
            string query = "SELECT PINHash FROM Customers WHERE CustomerID = @CustomerID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result == null || result == DBNull.Value || string.IsNullOrEmpty(result.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking login status: " + ex.Message);
            }
        }

        public bool SetCustomerPIN(int customerId, string pin)
        {
            string query = @"UPDATE Customers 
                           SET PINHash = @PINHash 
                           WHERE CustomerID = @CustomerID";

            try
            {
                string hashedPIN = PasswordHashHelper.HashPIN(pin);

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@PINHash", hashedPIN);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error setting PIN: " + ex.Message);
            }
        }

        public bool VerifyCustomerPIN(int customerId, string pin)
        {
            string query = "SELECT PINHash FROM Customers WHERE CustomerID = @CustomerID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value || string.IsNullOrEmpty(result.ToString()))
                    {
                        return false;
                    }

                    string storedHash = result.ToString();
                    return PasswordHashHelper.VerifyPIN(pin, storedHash);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error verifying PIN: " + ex.Message);
            }
        }

        public bool UpdateCustomerPIN(int customerId, string oldPIN, string newPIN)
        {
            try
            {
                if (!VerifyCustomerPIN(customerId, oldPIN))
                {
                    throw new Exception("Current PIN is incorrect");
                }

                string newHashedPIN = PasswordHashHelper.HashPIN(newPIN);

                string query = @"UPDATE Customers 
                               SET PINHash = @PINHash 
                               WHERE CustomerID = @CustomerID";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@PINHash", newHashedPIN);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating PIN: " + ex.Message);
            }
        }

        public bool UpdateCustomerPassword(int customerId, string oldPassword, string newPassword)
        {
            try
            {
                var customer = GetCustomerById(customerId);

                if (customer == null)
                    throw new Exception("Customer not found");

                if (!PasswordHashHelper.VerifyPassword(oldPassword, customer.PasswordHash))
                {
                    throw new Exception("Current password is incorrect");
                }

                string newHashedPassword = PasswordHashHelper.HashPassword(newPassword);

                string query = @"UPDATE Customers 
                               SET PasswordHash = @PasswordHash 
                               WHERE CustomerID = @CustomerID";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@PasswordHash", newHashedPassword);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating password: " + ex.Message);
            }
        }

        public bool UpdateCustomerPassword(int customerId, string newPassword)
        {
            try
            {
                string newHashedPassword = PasswordHashHelper.HashPassword(newPassword);

                string query = @"UPDATE Customers 
                                 SET PasswordHash = @PasswordHash,
                                     MustChangePassword = 1
                                 WHERE CustomerID = @CustomerID";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@PasswordHash", newHashedPassword);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error resetting password: " + ex.Message);
            }
        }

        public Customer GetCustomerById(int customerId)
        {
            string query = "SELECT * FROM Customers WHERE CustomerID = @CustomerID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapCustomerFromReader(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting customer: " + ex.Message);
            }

            return null;
        }

        public bool UpdateCustomer(Customer customer)
        {
            string query = @"UPDATE Customers
                            SET FullName = @FullName,
                            Address = @Address,
                            Email = @Email,
                            Phone = @Phone,
                            Gender = @Gender,
                            BirthDate = @BirthDate,
                            CivilStatus = @CivilStatus,
                            ImagePath = @ImagePath,
                            EmploymentStatus = @EmploymentStatus,
                            SourceOfFunds = @SourceOfFunds,
                            MonthlyIncomeRange = @MonthlyIncomeRange,
                            IDType = @IDType,
                            IDNumber = @IDNumber
                            WHERE CustomerID = @CustomerID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                    cmd.Parameters.AddWithValue("@FullName", customer.FullName);
                    cmd.Parameters.AddWithValue("@Address", customer.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", customer.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", customer.Gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BirthDate", customer.BirthDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CivilStatus", customer.CivilStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath", customer.ImagePath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmploymentStatus", customer.EmploymentStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SourceOfFunds", customer.SourceOfFunds ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MonthlyIncomeRange", customer.MonthlyIncomeRange ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IDType", customer.IDType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IDNumber", customer.IDNumber ?? (object)DBNull.Value);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating customer: " + ex.Message);
            }
        }

        public bool VerifyKYC(int customerId)
        {
            string query = @"UPDATE Customers 
                           SET IsKYCVerified = 1, 
                               KYCVerifiedDate = @VerifiedDate 
                           WHERE CustomerID = @CustomerID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@VerifiedDate", DateTime.Now);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error verifying KYC: " + ex.Message);
            }
        }

        public string GenerateCustomerCode()
        {
            string prefix = "CUST";
            string query = "SELECT MAX(CustomerCode) FROM Customers";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    int nextNumber = 1;

                    if (result != DBNull.Value && result != null)
                    {
                        string lastCode = result.ToString();
                        string numberPart = lastCode.Substring(4);
                        nextNumber = int.Parse(numberPart) + 1;
                    }

                    return $"{prefix}{nextNumber:D6}";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating customer code: " + ex.Message);
            }
        }

        public string ResetCustomerPassword(int customerId)
        {
            try
            {
                string tempPassword = GenerateTemporaryPassword();
                string hashedPassword = PasswordHashHelper.HashPassword(tempPassword);

                string query = @"UPDATE Customers 
                       SET PasswordHash = @PasswordHash,
                           MustChangePassword = 1
                       WHERE CustomerID = @CustomerID";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return tempPassword;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error resetting password: " + ex.Message);
            }
        }

        public string ResetCustomerPIN(int customerId)
        {
            try
            {
                string tempPIN = GenerateTemporaryPIN();
                string hashedPIN = PasswordHashHelper.HashPIN(tempPIN);

                string query = @"UPDATE Customers 
                       SET PINHash = @PINHash,
                           MustChangePIN = 1
                       WHERE CustomerID = @CustomerID";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@PINHash", hashedPIN);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return tempPIN;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error resetting PIN: " + ex.Message);
            }
        }

        private string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            Random random = new Random();
            return new string(Enumerable.Range(0, 8)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }

        private string GenerateTemporaryPIN()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public bool MustChangePassword(int customerId)
        {
            string query = "SELECT MustChangePassword FROM Customers WHERE CustomerID = @CustomerID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    return result != null && result != DBNull.Value && (bool)result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking password change requirement: " + ex.Message);
            }
        }

        public bool MustChangePIN(int customerId)
        {
            string query = "SELECT MustChangePIN FROM Customers WHERE CustomerID = @CustomerID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    return result != null && result != DBNull.Value && (bool)result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking PIN change requirement: " + ex.Message);
            }
        }

        public bool ClearMustChangePassword(int customerId)
        {
            string query = "UPDATE Customers SET MustChangePassword = 0 WHERE CustomerID = @CustomerID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error clearing password change flag: " + ex.Message);
            }
        }

        public bool ClearMustChangePIN(int customerId)
        {
            string query = "UPDATE Customers SET MustChangePIN = 0 WHERE CustomerID = @CustomerID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error clearing PIN change flag: " + ex.Message);
            }
        }

        public List<Customer> SearchCustomers(string searchText)
        {
            List<Customer> customers = new List<Customer>();
            string query = @"SELECT * FROM Customers 
                     WHERE FullName LIKE @SearchText 
                     OR CustomerCode LIKE @SearchText 
                     OR Phone LIKE @SearchText 
                     OR Email LIKE @SearchText";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchText", $"%{searchText}%");

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(MapCustomerFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error during customer search: " + ex.Message);
            }

            return customers;
        }

        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();
            string query = "SELECT * FROM Customers ORDER BY CreatedAt DESC";

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
                            customers.Add(MapCustomerFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all customers: " + ex.Message);
            }

            return customers;
        }

        private Customer MapCustomerFromReader(SqlDataReader reader)
        {
            return new Customer
            {
                CustomerID = (int)reader["CustomerID"],
                CustomerCode = reader["CustomerCode"].ToString(),
                FullName = reader["FullName"].ToString(),
                Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : null,
                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : null,
                Gender = reader["Gender"] != DBNull.Value ? reader["Gender"].ToString() : null,
                BirthDate = reader["BirthDate"] != DBNull.Value ? (DateTime?)reader["BirthDate"] : null,
                CivilStatus = reader["CivilStatus"] != DBNull.Value ? reader["CivilStatus"].ToString() : null,
                ImagePath = reader["ImagePath"] != DBNull.Value ? reader["ImagePath"].ToString() : null,
                PasswordHash = reader["PasswordHash"] != DBNull.Value ? reader["PasswordHash"].ToString() : null,
                PINHash = reader["PINHash"] != DBNull.Value ? reader["PINHash"].ToString() : null,
                EmploymentStatus = reader["EmploymentStatus"] != DBNull.Value ? reader["EmploymentStatus"].ToString() : null,
                SourceOfFunds = reader["SourceOfFunds"] != DBNull.Value ? reader["SourceOfFunds"].ToString() : null,
                MonthlyIncomeRange = reader["MonthlyIncomeRange"] != DBNull.Value ? reader["MonthlyIncomeRange"].ToString() : null,
                IDType = reader["IDType"] != DBNull.Value ? reader["IDType"].ToString() : null,
                IDNumber = reader["IDNumber"] != DBNull.Value ? reader["IDNumber"].ToString() : null,
                IsKYCVerified = (bool)reader["IsKYCVerified"],
                KYCVerifiedDate = reader["KYCVerifiedDate"] != DBNull.Value ? (DateTime?)reader["KYCVerifiedDate"] : null,
                CreatedAt = (DateTime)reader["CreatedAt"]
            };
        }
    }
}
