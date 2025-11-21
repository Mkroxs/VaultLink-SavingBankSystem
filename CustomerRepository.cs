using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace VaultLinkBankSystem
{
    public class CustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["BankingDB"].ConnectionString;
        }

        // Create customer WITH KYC (all in one step)
        public int CreateCustomer(Customer customer)
        {
            string query = @"INSERT INTO Customers (
                CustomerCode, FullName, Address, Email, Phone, Gender, BirthDate, CivilStatus, ImagePath, PIN,
                EmploymentStatus, EmployerName, SourceOfFunds, MonthlyIncomeRange, IDType, IDNumber,
                IsKYCVerified, KYCVerifiedDate, CreatedAt
            )
            VALUES (
                @CustomerCode, @FullName, @Address, @Email, @Phone, @Gender, @BirthDate, @CivilStatus, @ImagePath, @PIN,
                @EmploymentStatus, @EmployerName, @SourceOfFunds, @MonthlyIncomeRange, @IDType, @IDNumber,
                @IsKYCVerified, @KYCVerifiedDate, @CreatedAt
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                // Auto-generate PIN if not provided
                if (string.IsNullOrEmpty(customer.PIN))
                {
                    customer.PIN = GeneratePIN();
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Basic info
                    cmd.Parameters.AddWithValue("@CustomerCode", customer.CustomerCode);
                    cmd.Parameters.AddWithValue("@FullName", customer.FullName);
                    cmd.Parameters.AddWithValue("@Address", customer.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", customer.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Gender", customer.Gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BirthDate", customer.BirthDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CivilStatus", customer.CivilStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath", customer.ImagePath ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PIN", customer.PIN);

                    // KYC info
                    cmd.Parameters.AddWithValue("@EmploymentStatus", customer.EmploymentStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployerName", customer.EmployerName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SourceOfFunds", customer.SourceOfFunds ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MonthlyIncomeRange", customer.MonthlyIncomeRange ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IDType", customer.IDType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IDNumber", customer.IDNumber ?? (object)DBNull.Value);

                    // System fields
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

        // Get customer by ID
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

        // Update customer (including KYC info)
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
                               EmployerName = @EmployerName,
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
                    cmd.Parameters.AddWithValue("@EmployerName", customer.EmployerName ?? (object)DBNull.Value);
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

        // Verify KYC
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

        // Kiosk Login
        public Customer KioskLogin(string email, string pin)
        {
            string query = "SELECT * FROM Customers WHERE Email = @Email AND PIN = @PIN";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@PIN", pin);

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
                throw new Exception("Error during kiosk login: " + ex.Message);
            }

            return null;
        }

        // Generate PIN
        public string GeneratePIN()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        // Generate Customer Code
        public string GenerateCustomerCode()
        {
            string prefix = "CUST";
            string query = "SELECT MAX(CustomerID) FROM Customers";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    int nextId = (result != DBNull.Value && result != null) ? Convert.ToInt32(result) + 1 : 1;
                    return $"{prefix}{nextId:D6}";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating customer code: " + ex.Message);
            }
        }

        // Search customers
        public List<Customer> SearchCustomers(string searchText)
        {
            List<Customer> customers = new List<Customer>();
            // Use '%' for SQL LIKE wildcards. Search fields: FullName, CustomerCode, Phone, Email
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
                // Log error or rethrow
                throw new Exception("Error during customer search: " + ex.Message);
            }

            return customers;
        }

        // Get all customers
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
                PIN = reader["PIN"] != DBNull.Value ? reader["PIN"].ToString() : null,
                EmploymentStatus = reader["EmploymentStatus"] != DBNull.Value ? reader["EmploymentStatus"].ToString() : null,
                EmployerName = reader["EmployerName"] != DBNull.Value ? reader["EmployerName"].ToString() : null,
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