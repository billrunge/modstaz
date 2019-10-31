using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    public class GetStorageAreas
    {
        public int UserId { get; set; }

        public GetStorageAreas() { }

        public GetStorageAreas(int userId)
        {
            UserId = userId;
        }

        public async Task<string> GetStorsageAreasAsync()
        {

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = @"
                        SELECT S.[ID], 
                               S.[Name], 
                               S.[CreatedBy], 
                               S.[CreatedOn], 
                               S.[LastModified] 
                        FROM   [StorageAreas] S 
                               INNER JOIN [StorageAreaAccess] SA 
                                       ON SA.StorageAreaID = S.ID 
                        WHERE  SA.UserID = @UserID";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@UserID",
                    SqlDbType = SqlDbType.Int,
                    Value = UserId
                });

                using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);

                    return JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                }
            }
        }
    }
}
