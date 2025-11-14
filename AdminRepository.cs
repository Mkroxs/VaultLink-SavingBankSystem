using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace VaultLinkBankSystem
{
    public class AdminRepository
    {
        private readonly string _connectionString =
    @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Banking.mdf;Integrated Security=True;";
        public Admin Login(string username, string password)
        {
            Admin admin = null;
            

            // First, retrieve the admin by username only
            string query = "SELECT * FROM Admin WHERE AdminUsername = @username";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        admin = new Admin
                        {
                            AdminID = (int)reader["AdminID"],
                            AdminUsername = reader["AdminUsername"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        };
                    }
                }
            }

            if (admin != null && admin.PasswordHash == password)
            {
                return admin;
            }

            return null;
        }
    }
}
