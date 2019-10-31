using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    public class GetStorageAreaColumns
    {
        public int StorageAreaId { get; set; }

        public GetStorageAreaColumns() { }
        public GetStorageAreaColumns(int storageAreaId)
        {
            StorageAreaId = storageAreaId;
        }

        public async Task<string> GetStorageAreaColumnsAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                SELECT [ID], 
                       [DisplayName], 
                       [ColumnTypeID], 
                       [IsEditable],
                       [CreatedOn], 
                       [LastModified] 
                FROM   [{ StorageAreaId }Columns]";

                SqlCommand command = new SqlCommand(sql, connection);

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
