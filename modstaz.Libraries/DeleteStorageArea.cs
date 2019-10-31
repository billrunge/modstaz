using System;
using System.Data;
using System.Data.SqlClient;
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
                    IF Object_id('{ StorageAreaId }Columns', 'U') IS NOT NULL 
                      BEGIN 
                          DROP TABLE [{ StorageAreaId }Columns] 
                      END";
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();

                command.CommandText = $@"
                    IF Object_id('{ StorageAreaId }Rows', 'U') IS NOT NULL 
                      BEGIN 
                          DROP TABLE [{ StorageAreaId }Rows] 
                      END";
                
                await command.ExecuteNonQueryAsync();

                command.CommandText = $"DELETE FROM [StorageAreaAccess] WHERE StorageAreaID = @StorageAreaID";

                command.Parameters.Add(new SqlParameter { ParameterName = "@StorageAreaID", SqlDbType = SqlDbType.Int, Value = StorageAreaId });
                await command.ExecuteNonQueryAsync();

                command.CommandText = $"DELETE FROM [StorageAreas] WHERE ID = @StorageAreaID";
                await command.ExecuteNonQueryAsync();

            }
        }
    }
}
