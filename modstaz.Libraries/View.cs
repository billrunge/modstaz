using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    class View
    {
        public int StorageAreaId { get; set; }

        public async Task CreateViewAsync(string name)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                        INSERT INTO [{ StorageAreaId }Views] 
                                    ([Name], 
                                     [CreatedOn], 
                                     [LastModified]) 
                        VALUES      (@ViewName, 
                                     Getutcdate(), 
                                     Getutcdate())";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@ViewName", SqlDbType = SqlDbType.NVarChar, Value = name });
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task CreateViewColumnAsync(int viewId, int columnId, int order)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                        INSERT INTO [{ StorageAreaId }ViewColumns] 
                                    ([ViewId], 
                                     [ColumnId], 
                                     [Order], 
                                     [CreatedOn], 
                                     [LastModified]) 
                        VALUES      (@ViewId, 
                                     @ColumnId, 
                                     @Order, 
                                     Getutcdate(), 
                                     Getutcdate()) ";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@ViewId", SqlDbType = SqlDbType.Int, Value = viewId });
                command.Parameters.Add(new SqlParameter { ParameterName = "@ColumnId", SqlDbType = SqlDbType.Int, Value = columnId });
                command.Parameters.Add(new SqlParameter { ParameterName = "@Order", SqlDbType = SqlDbType.Int, Value = order });
                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task CreateViewConditionAsync(int viewId, int columnId, string condition)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                        INSERT INTO [{ StorageAreaId }ViewConditions] 
                                    ([ViewId], 
                                     [ColumnId], 
                                     [Condition], 
                                     [CreatedOn], 
                                     [LastModified]) 
                        VALUES      (@ViewId, 
                                     @ColumnId, 
                                     @Condition, 
                                     Getutcdate(), 
                                     Getutcdate())";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@ViewId", SqlDbType = SqlDbType.Int, Value = viewId });
                command.Parameters.Add(new SqlParameter { ParameterName = "@ColumnId", SqlDbType = SqlDbType.Int, Value = columnId });
                command.Parameters.Add(new SqlParameter { ParameterName = "@Order", SqlDbType = SqlDbType.NVarChar, Value = condition });
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task GetViewColumnsAsync(int viewId)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                        SELECT [ColumnId] 
                        FROM   [1ViewColumns] 
                        WHERE  [ViewId] = @ViewId 
                        ORDER  BY [Order]";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@ViewId", SqlDbType = SqlDbType.Int, Value = viewId });
                
            }
        }



    }

    public class ViewColumn
    {
        public int ViewId { get; set; }
    }
}
