using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    public class DeleteStorageArea
    {
        public int StorageAreaId { get; set; }

        public DeleteStorageArea() { }

        public DeleteStorageArea(int storageAreaId)
        {
            StorageAreaId = storageAreaId;
        }

        public async Task DeleteStorageAreaAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = $@"
                    DROP TABLE [{ StorageAreaId }Columns]
                    DROP TABLE [{ StorageAreaId }Rows]
                    DELETE FROM [StorageAreaAccess] WHERE StorageAreaID = @StorageAreaID
                    DELETE FROM [StorageAreas] WHERE ID = @StorageAreaID";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@StorageAreaID", SqlDbType = SqlDbType.Int, Value = StorageAreaId });
                await command.ExecuteNonQueryAsync();
            }
        }

    }
}
