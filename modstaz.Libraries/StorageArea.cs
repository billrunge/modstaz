﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using static modstaz.Libraries.UserRole;

namespace modstaz.Libraries
{
    public class StorageArea
    {
        public string StorageAreaName { get; set; }
        public int UserId { get; set; }
        public int StorageAreaId { get; set; }
        public ILogger Logger { get; set; }

        public StorageArea(string storageAreaName, int userId)
        {
            StorageAreaName = storageAreaName;
            UserId = userId;
        }
        public StorageArea()
        {

        }

        async public Task CreateStorageAreaAsync()
        {
            StorageAreaId = await CreateStorageAreaIdAsync(StorageAreaName, UserId);
            await CreateRowsTableAsync(StorageAreaId);
            await CreateColumnsTableAsync(StorageAreaId);
            await CreateViewsTablesAsync(StorageAreaId);
            Access access = new Access() { StorageAreaId = StorageAreaId, UserId = UserId };
            await access.AddAccessAsync(StorageAreaId, UserId, (int)Roles.Creator);
        }

        public async Task DeleteStorageAreaAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    IF Object_id('{ StorageAreaId }ViewColumns', 'U') IS NOT NULL 
                      BEGIN 
                          DROP TABLE [{ StorageAreaId }ViewColumns] 
                      END";
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();

                command.CommandText = $@"
                    IF Object_id('{ StorageAreaId }ViewConditions', 'U') IS NOT NULL 
                      BEGIN 
                          DROP TABLE [{ StorageAreaId }ViewConditions] 
                      END";

                await command.ExecuteNonQueryAsync();

                command.CommandText = $@"                
                    IF Object_id('{ StorageAreaId }Columns', 'U') IS NOT NULL 
                      BEGIN 
                          DROP TABLE [{ StorageAreaId }Columns] 
                      END";

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

                command.CommandText = $@"
                    IF Object_id('{ StorageAreaId }Views', 'U') IS NOT NULL 
                      BEGIN 
                          DROP TABLE [{ StorageAreaId }Views] 
                      END";

                await command.ExecuteNonQueryAsync();

                command.CommandText = $"DELETE FROM [StorageAreas] WHERE ID = @StorageAreaID";
                await command.ExecuteNonQueryAsync();

            }
        }

        public async Task<string> GetStorageAreaAsync(int viewId)
        {
            //return await GetRowsAsync(await GetColumnsAsync());
            View view = new View() { StorageAreaId = StorageAreaId }; 
            return await GetRowsAsync(await view.GetViewColumnsAsync(viewId));
        }

        public async Task<string> GetStorageAreaRowAsync(int rowId, bool onlyShowEditable)
        {
            if (onlyShowEditable)
            {
                return await GetRowColumnsByIdAsync(rowId, await GetEditableColumnsAsync());
            }else
            {
                return await GetRowColumnsByIdAsync(rowId, await GetColumnsAsync());
            }

        }
        public async Task<string> GetStorsageAreasAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = @"
                        SELECT [Id], 
                               [Name], 
                               [CreatedBy], 
                               [CreatedOn], 
                               [LastModified] 
                        FROM   [StorageAreas]";

                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);

                    return JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                }
            }
        }
        public async Task<string> GetStorsageAreasByUserIdAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = @"
                        SELECT S.[Id], 
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

        private async Task<List<KeyValuePair<int, string>>> GetColumnsAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    SELECT [Id], 
                           [DisplayName] 
                    FROM   [{ StorageAreaId }Columns] ";

                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);

                    List<KeyValuePair<int, string>> idColumnList = new List<KeyValuePair<int, string>>();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (!int.TryParse(row["ID"].ToString(), out int columnId))
                        {
                            throw new InvalidCastException("Unable to cast Column ID returned from Database to int");
                        }

                        KeyValuePair<int, string> idColumnPair = new KeyValuePair<int, string>(columnId, row["DisplayName"].ToString());

                        idColumnList.Add(idColumnPair);
                    }

                    return idColumnList;
                }
            }
        }

        private async Task<List<KeyValuePair<int, string>>> GetEditableColumnsAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    SELECT [Id], 
                           [DisplayName] 
                    FROM   [{ StorageAreaId }Columns]
                    WHERE  [IsEditable] = 1";

                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);

                    List<KeyValuePair<int, string>> idColumnList = new List<KeyValuePair<int, string>>();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (!int.TryParse(row["ID"].ToString(), out int columnId))
                        {
                            throw new InvalidCastException("Unable to cast Column ID returned from Database to int");
                        }

                        KeyValuePair<int, string> idColumnPair = new KeyValuePair<int, string>(columnId, row["DisplayName"].ToString());

                        idColumnList.Add(idColumnPair);
                    }

                    return idColumnList;
                }
            }
        }
        private async Task<string> GetRowsAsync(List<ViewColumn> viewColumns)
        {

            string columnString = "";
            foreach (ViewColumn vc in viewColumns)
            {
                columnString += $" [{ vc.ColumnId }] AS [{ vc.DisplayName }],";
            }
            columnString = columnString.TrimEnd(',');

            Logger.LogInformation(columnString);

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    SELECT { columnString } 
                    FROM   [{ StorageAreaId }Rows]";

                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);

                    return JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                }
            }
        }
        //private async Task<string> GetRowsAsync(List<KeyValuePair<int, string>> idColumn)
        //{
        //    string columnString = $"[{ idColumn[idColumn.Count - 1].Key.ToString() }] AS [{ idColumn[idColumn.Count - 1].Value }]";

        //    for (int i = idColumn.Count - 2; i >= 0; i--)
        //    {
        //        columnString = $"[{ idColumn[i].Key.ToString() }] AS [{ idColumn[i].Value }], " + columnString;
        //    }

        //    using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
        //    {
        //        await connection.OpenAsync();

        //        string sql = $@"
        //            SELECT { columnString } 
        //            FROM   [{ StorageAreaId }Rows]";

        //        SqlCommand command = new SqlCommand(sql, connection);

        //        using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
        //        {
        //            DataTable dataTable = new DataTable();
        //            dataTable.Load(dataReader);

        //            return JsonConvert.SerializeObject(dataTable, Formatting.Indented);
        //        }
        //    }
        //}

        private async Task<string> GetRowByIdAsync(int rowId, List<KeyValuePair<int, string>> idColumn)
        {
            string columnString = $"[{ idColumn[idColumn.Count - 1].Key.ToString() }] AS [{ idColumn[idColumn.Count - 1].Value }]";

            for (int i = idColumn.Count - 2; i >= 0; i--)
            {
                columnString = $"[{ idColumn[i].Key.ToString() }] AS [{ idColumn[i].Value }], " + columnString;
            }

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    SELECT { columnString } 
                    FROM   [{ StorageAreaId }Rows]
                    WHERE [1] = @RowId";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add(new SqlParameter
                {
                    ParameterName = "@RowId",
                    SqlDbType = SqlDbType.Int,
                    Value = rowId
                });

                using (SqlDataReader dataReader = await command.ExecuteReaderAsync())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);

                    return JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                }
            }
        }

        private async Task<string> GetRowColumnsByIdAsync(int rowId, List<KeyValuePair<int, string>> idColumns)
        {
            List<Row.RowColumn> rowColumnsList = new List<Row.RowColumn>();


            foreach (KeyValuePair<int, string> idColumn in idColumns)
            {
                int columnId = idColumn.Key;
                Column column = new Column() { StorageAreaId = StorageAreaId };
                Row.RowColumn rowColumn = new Row.RowColumn()
                {
                    ColumnId = columnId,
                    DisplayName = idColumn.Value,
                    RowId = rowId,
                    ColumnTypeId = await column.GetColumnTypeIdByColumnId(columnId)
                };

                using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
                {
                    await connection.OpenAsync();

                    string sql = $@"
                    SELECT [{ columnId }]
                    FROM   [{ StorageAreaId }Rows]
                    WHERE [1] = @RowId";

                    SqlCommand command = new SqlCommand(sql, connection);

                    command.Parameters.Add(new SqlParameter
                    {
                        ParameterName = "@RowId",
                        SqlDbType = SqlDbType.Int,
                        Value = rowId
                    });
                    object result = await command.ExecuteScalarAsync();
                    rowColumn.Value = result.ToString();
                    rowColumnsList.Add(rowColumn);
                }
            }
            return JsonConvert.SerializeObject(rowColumnsList);
        }

        async Task<int> CreateStorageAreaIdAsync(string name, int userId)
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
						INSERT INTO [StorageAreas] 
									([Name], 
									 [CreatedBy], 
									 [CreatedOn], 
									 [LastModified]) 
						OUTPUT      INSERTED.Id
						VALUES      (@Name, 
									 @UserId, 
									 Getutcdate(), 
									 Getutcdate()) ";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add(new SqlParameter { ParameterName = "@Name", SqlDbType = SqlDbType.NVarChar, Value = name });
                command.Parameters.Add(new SqlParameter { ParameterName = "@UserId", SqlDbType = SqlDbType.Int, Value = userId });

                var results = await command.ExecuteScalarAsync();

                if (!int.TryParse(results.ToString(), out int storageAreaId))
                {
                    throw new InvalidCastException("Unable to cast StorageAreaId returned from database to int");
                }
                else
                {
                    return storageAreaId;
                }
            }

        }
        async Task CreateRowsTableAsync(int storageAreaId)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
                await connection.OpenAsync();

                string sql = $@"
						SET ANSI_NULLS ON;

						SET QUOTED_IDENTIFIER ON;

						CREATE TABLE [dbo].[{ storageAreaId }Rows](
							[1] [int] IDENTITY(1,1) NOT NULL,
						PRIMARY KEY CLUSTERED 
						(
							[1] ASC
						)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
						) ON [PRIMARY];";

                SqlCommand command = new SqlCommand(sql, connection);

                await command.ExecuteNonQueryAsync();

            }
        }
        async Task CreateColumnsTableAsync(int storageAreaId)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
                connection.Open();

                string sql = $@"
						SET ANSI_NULLS ON; 
						SET QUOTED_IDENTIFIER ON; 

						CREATE TABLE [dbo].[{ storageAreaId }Columns] 
						  ( 
							 [Id]           [INT] IDENTITY(1, 1) NOT NULL, 
							 [DisplayName]  [NVARCHAR](255) NOT NULL UNIQUE, 
							 [ColumnTypeId] [INT] NOT NULL,
							 [IsEditable]   [BIT] NOT NULL, 
							 [CreatedOn]    [DATETIME] NOT NULL, 
							 [LastModified] [DATETIME] NOT NULL, 
							 PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (STATISTICS_NORECOMPUTE = OFF, 
							 IGNORE_DUP_KEY = OFF) ON [PRIMARY] 
						  ) 
						ON [PRIMARY]; 

						ALTER TABLE [dbo].[{ storageAreaId }Columns]  WITH CHECK ADD  CONSTRAINT [FK__{ storageAreaId }Columns__ColumnTypeId] FOREIGN KEY([ColumnTypeId])
						REFERENCES [dbo].[ColumnTypes] ([Id])

						ALTER TABLE [dbo].[{ storageAreaId }Columns] CHECK CONSTRAINT [FK__{ storageAreaId }Columns__ColumnTypeId]";

                SqlCommand command = new SqlCommand(sql, connection);

                await command.ExecuteNonQueryAsync();

                Column column = new Column()
                {
                    StorageAreaId = storageAreaId,
                    IsEditable = false,
                    ColumnTypeId = (int)ColumnType.Types.Integer,
                    DisplayName = "Id",
                    CreateColumnInRowsTable = false
                };
                await column.CreateColumnAsync();

                column.ColumnTypeId = (int)ColumnType.Types.DateTime;
                column.DisplayName = "Created On";
                column.CreateColumnInRowsTable = true;
                await column.CreateColumnAsync();
                column.DisplayName = "Last Modified";
                await column.CreateColumnAsync();
            }
        }

        async Task CreateViewsTablesAsync(int storageAreaId)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
                connection.Open();

                string sql = $@"
						SET ANSI_NULLS ON; 
						SET QUOTED_IDENTIFIER ON; 

						CREATE TABLE [dbo].[{ storageAreaId }Views] 
						  ( 
							 [Id]           [INT] IDENTITY(1, 1) NOT NULL, 
							 [Name]         [NVARCHAR](255) NOT NULL,
							 [CreatedOn]    [DATETIME] NOT NULL, 
							 [LastModified] [DATETIME] NOT NULL, 
							 PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (STATISTICS_NORECOMPUTE = OFF, 
							 IGNORE_DUP_KEY = OFF) ON [PRIMARY] 
						  ) 
						ON [PRIMARY];

						CREATE TABLE [dbo].[{ storageAreaId }ViewColumns] 
						  ( 
							 [Id]           [INT] IDENTITY(1, 1) NOT NULL, 
							 [ViewId]       [INT] FOREIGN KEY REFERENCES [{ storageAreaId }Views](Id),
							 [ColumnId]     [INT] FOREIGN KEY REFERENCES [{ storageAreaId }Columns](Id), 
							 [Order]        [INT] NOT NULL, 
							 [CreatedOn]    [DATETIME] NOT NULL, 
							 [LastModified] [DATETIME] NOT NULL, 
							 PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (STATISTICS_NORECOMPUTE = OFF, 
							 IGNORE_DUP_KEY = OFF) ON [PRIMARY] 
						  ) 
						ON [PRIMARY];

						CREATE TABLE [dbo].[{ storageAreaId }ViewConditions] 
						  ( 
							 [Id]           [INT] IDENTITY(1, 1) NOT NULL, 
							 [ViewId]       [INT] FOREIGN KEY REFERENCES [{ storageAreaId }Views](Id),
							 [ColumnId]     [INT] FOREIGN KEY REFERENCES [{ storageAreaId }Columns](Id), 
							 [Condition]    [NVARCHAR](500) NOT NULL, 
							 [CreatedOn]    [DATETIME] NOT NULL, 
							 [LastModified] [DATETIME] NOT NULL, 
							 PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (STATISTICS_NORECOMPUTE = OFF, 
							 IGNORE_DUP_KEY = OFF) ON [PRIMARY] 
						  ) 
						ON [PRIMARY];";

                SqlCommand command = new SqlCommand(sql, connection);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
