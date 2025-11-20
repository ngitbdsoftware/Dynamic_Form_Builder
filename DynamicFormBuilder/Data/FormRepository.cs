using System.Data;
using DynamicFormBuilder.Models;
using Microsoft.Data.SqlClient;

namespace DynamicFormBuilder.Data
{
    public class FormRepository
    {
        private readonly string _connectionString;
        public FormRepository(string cs) => _connectionString = cs;

        public int SaveForm(string title, List<FieldDto> fields)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var tran = conn.BeginTransaction();
            try
            {
                int formId;

                // Save Form
                using (var cmd = new SqlCommand("sp_SaveForm", conn, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Title", title);

                    var outParam = new SqlParameter("@NewFormId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(outParam);
                    cmd.ExecuteNonQuery();

                    formId = (int)outParam.Value;
                }

                // Save Fields
                foreach (var f in fields)
                {
                    using var cmd = new SqlCommand("sp_SaveFormField", conn, tran);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FormId", formId);
                    cmd.Parameters.AddWithValue("@Label", f.Label);
                    cmd.Parameters.AddWithValue("@IsRequired", f.IsRequired);

                    cmd.Parameters.AddWithValue(
                        "@SelectedOptionValueId",
                        (object?)f.SelectedOptionValueId ?? DBNull.Value
                    );

                    cmd.ExecuteNonQuery();
                }

                tran.Commit();
                return formId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public (List<FormDto> Data, int RecordsTotal, int RecordsFiltered)
            GetFormsPaged(int start, int length, string? search)
        {
            var list = new List<FormDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetFormsPaged", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Start", start);
            cmd.Parameters.AddWithValue("@Length", length);
            cmd.Parameters.AddWithValue("@Search", (object?)search ?? DBNull.Value);

            var outTotal = new SqlParameter("@RecordsTotal", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            var outFiltered = new SqlParameter("@RecordsFiltered", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outTotal);
            cmd.Parameters.Add(outFiltered);

            conn.Open();

            using var rdr = cmd.ExecuteReader();

            // read rows
            while (rdr.Read())
            {
                list.Add(new FormDto
                {
                    FormId = rdr.GetInt32(0),
                    Title = rdr.IsDBNull(1) ? "" : rdr.GetString(1),
                    CreatedDate = rdr.IsDBNull(2) ? (DateTime?)null : rdr.GetDateTime(2)
                });
            }

            // CLOSE reader first
            rdr.Close();

            // NOW output parameters are available
            int total = (outTotal.Value == DBNull.Value || outTotal.Value == null) ? 0 : (int)outTotal.Value;
            int filtered = (outFiltered.Value == DBNull.Value || outFiltered.Value == null) ? 0 : (int)outFiltered.Value;

            return (list, total, filtered);

        }

        public FormDto? GetFormWithFields(int formId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetFormWithFields", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@FormId", formId);

            conn.Open();

            using var rdr = cmd.ExecuteReader();

            FormDto? form = null;
            if (rdr.Read())
            {
                form = new FormDto
                {
                    FormId = rdr.GetInt32(0),
                    Title = rdr.GetString(1)
                };
            }

            if (form == null) return null;

            if (rdr.NextResult())
            {
                while (rdr.Read())
                {
                    form.Fields.Add(new FieldDto
                    {
                        FieldId = rdr.GetInt32(0),
                        Label = rdr.GetString(2),
                        IsRequired = rdr.GetBoolean(3),
                        SelectedOptionValueId = rdr.IsDBNull(4)
                            ? null
                            : (int?)rdr.GetInt32(4)
                    });
                }
            }

            return form;
        }
    }
}
