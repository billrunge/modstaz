using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    class Access
    {
        public int StorageAreaId { get; set; }
        public int UserId { get; set; }

        public async Task AddAccessAsync(int storageAreaId, int userId)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
                await connection.OpenAsync();

                string sql = $@"
						INSERT INTO [StorageAreaAccess] 
									([UserID], 
									 [StorageAreaID], 
									 [RoleID]) 
						VALUES     (@UserID, 
									@StorageAreaID, 
									2) ";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add(new SqlParameter { ParameterName = "@StorageAreaID", SqlDbType = SqlDbType.Int, Value = StorageAreaId });
                command.Parameters.Add(new SqlParameter { ParameterName = "@UserID", SqlDbType = SqlDbType.Int, Value = UserId });

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
