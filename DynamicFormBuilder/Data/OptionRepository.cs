using System.Data;
using DynamicFormBuilder.Models;
using Microsoft.Data.SqlClient;

namespace DynamicFormBuilder.Data
{
    public class OptionRepository
    {
        private readonly string _connectionString;
        public OptionRepository(string cs) => _connectionString = cs;

        public List<OptionDto> GetOptionValues(int optionId)
        {
            var list = new List<OptionDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetOptionValues", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@OptionId", optionId);

            conn.Open();

            using var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new OptionDto
                {
                    OptionValueId = rdr.GetInt32(0),
                    OptionId = rdr.GetInt32(1),
                    Value = rdr.GetString(2)
                });
            }

            return list;
        }
    }
}
