using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultLinkBankSystem
{
    public class InterestRateRepository
    {
        private readonly string _connectionString;

        public InterestRateRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["BankingDB"].ConnectionString;
        }
        public decimal GetAnnualRateById(int rateId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT AnnualRate FROM InterestRates WHERE InterestRateID = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", rateId);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDecimal(result) / 100 : 0;
                    // divide by 100 to convert: 3.25 → 0.0325
                }
            }
        }

        public decimal GetCurrentAnnualRate()
        {
            decimal rate = 0m;

            string query = @"
        SELECT TOP 1 AnnualRate
        FROM InterestRates
        ORDER BY EffectiveDate DESC;
    ";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    rate = Convert.ToDecimal(result);
            }

            return rate / 100m; // Convert percentage (e.g., 3.25) → 0.0325
        }

    }
}
