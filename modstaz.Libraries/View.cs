using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    public class View
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
        //public async Task CreateViewConditionAsync(int viewId, int columnId, string condition)
        //{
        //    using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
        //    {
        //        await connection.OpenAsync();

        //        string sql = $@"
        //                INSERT INTO [{ StorageAreaId }ViewConditions] 
        //                            ([ViewId], 
        //                             [ColumnId], 
        //                             [Condition], 
        //                             [CreatedOn], 
        //                             [LastModified]) 
        //                VALUES      (@ViewId, 
        //                             @ColumnId, 
        //                             @Condition, 
        //                             Getutcdate(), 
        //                             Getutcdate())";

        //        SqlCommand command = new SqlCommand(sql, connection);
        //        command.Parameters.Add(new SqlParameter { ParameterName = "@ViewId", SqlDbType = SqlDbType.Int, Value = viewId });
        //        command.Parameters.Add(new SqlParameter { ParameterName = "@ColumnId", SqlDbType = SqlDbType.Int, Value = columnId });
        //        command.Parameters.Add(new SqlParameter { ParameterName = "@Order", SqlDbType = SqlDbType.NVarChar, Value = condition });
        //        await command.ExecuteNonQueryAsync();
        //    }
        //}

        public async Task<List<ViewColumn>> GetViewColumnsAsync(int viewId)
        {
            List<ViewColumn> viewColumns = new List<ViewColumn>() { new ViewColumn() { ColumnId = 1, DisplayName = "Id", Order = 0, ViewId = viewId } };

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                        SELECT C.[DisplayName], 
                               VC.[ColumnId], 
                               VC.[Order] 
                        FROM   [{ StorageAreaId }ViewColumns] VC 
                               INNER JOIN [{ StorageAreaId }Columns] C 
                                       ON VC.[ColumnId] = C.[Id] 
                        WHERE  VC.[ViewId] = @ViewId
                        ORDER BY VC.[Order] ASC";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@ViewId", SqlDbType = SqlDbType.Int, Value = viewId });
                DataTable viewColumnsDataTable = new DataTable();
                using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    viewColumnsDataTable.Load(dataReader);

                    foreach (DataRow row in viewColumnsDataTable.Rows)
                    {
                        ViewColumn viewColumn = new ViewColumn()
                        {
                            ViewId = viewId,
                            Order = (int)row["Order"],
                            ColumnId = (int)row["ColumnId"],
                            DisplayName = (string)row["DisplayName"]
                        };
                        viewColumns.Add(viewColumn);
                    }
                    return viewColumns;
                }
            }

        }

        public async Task<string> GetViewsAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                        SELECT [Id], 
                               [Name], 
                               [CreatedOn], 
                               [LastModified] 
                        FROM   [{ StorageAreaId }Views] ";

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

    public class ViewColumn
    {
        public int ViewId { get; set; }
        public int ColumnId { get; set; }
        public string DisplayName { get; set; }
        public int Order { get; set; }
    }



}
