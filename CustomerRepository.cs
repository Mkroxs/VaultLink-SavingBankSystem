using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace VaultLinkBankSystem
{
    public class CustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository()
        {
            _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\JB\Source\Repos\VaultLink-SavingBankSystem\Banking.mdf;Integrated Security=True";
        }

        // Create new customer
        public int CreateCustomer(Customer customer)
        {
            string query = @"INSERT INTO Customers (CustomerCode, FullName, Address, Email, Phone, Gender, BirthDate, CivilStatus, ImagePath, CreatedAt)
                           VALUES (@CustomerCode, @FullName, @Address, @Email, @Phone, @Gender, @BirthDate, @CivilStatus, @ImagePath, @CreatedAt);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
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

        // Get customer by customer code
        public Customer GetCustomerByCode(string customerCode)
        {
            string query = "SELECT * FROM Customers WHERE CustomerCode = @CustomerCode";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerCode", customerCode);

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
                throw new Exception("Error getting customer by code: " + ex.Message);
            }

            return null;
        }

        // Search customers by name, email, or phone
        public List<Customer> SearchCustomers(string searchTerm)
        {
            List<Customer> customers = new List<Customer>();
            string query = @"SELECT * FROM Customers 
                           WHERE FullName LIKE @SearchTerm 
                           OR Email LIKE @SearchTerm 
                           OR Phone LIKE @SearchTerm 
                           OR CustomerCode LIKE @SearchTerm
                           ORDER BY CreatedAt DESC";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

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
                throw new Exception("Error searching customers: " + ex.Message);
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

        // Update customer
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
                               ImagePath = @ImagePath
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

        // Delete customer
        public bool DeleteCustomer(int customerId)
        {
            string query = "DELETE FROM Customers WHERE CustomerID = @CustomerID";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting customer: " + ex.Message);
            }
        }

        // Get total customer count
        public int GetCustomerCount()
        {
            string query = "SELECT COUNT(*) FROM Customers";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting customer count: " + ex.Message);
            }
        }

        // Generate unique customer code
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
                    return $"{prefix}{nextId:D6}"; // CUST000001, CUST000002, etc.
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating customer code: " + ex.Message);
            }
        }

        // Helper method to map SqlDataReader to Customer object
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
                CreatedAt = (DateTime)reader["CreatedAt"]
            };
        }
    }
}