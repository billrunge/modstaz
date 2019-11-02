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
            Access access = new Access() { StorageAreaId = StorageAreaId, UserId = UserId };
            UserRole roles = new UserRole();
            await access.AddAccessAsync(StorageAreaId, UserId, (int)Roles.Creator);
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

        public async Task<string> GetStorageAreaAsync()
        {
            return await GetRowsAsync(await GetColumnsAsync());
        }
        public async Task<string> GetStorsageAreasAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = @"
                        SELECT [ID], 
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

        private async Task<List<KeyValuePair<int, string>>> GetColumnsAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    SELECT [ID], 
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
        private async Task<string> GetRowsAsync(List<KeyValuePair<int, string>> idColumn)
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
						OUTPUT      INSERTED.ID 
						VALUES      (@Name, 
									 @UserID, 
									 Getutcdate(), 
									 Getutcdate()) ";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add(new SqlParameter { ParameterName = "@Name", SqlDbType = SqlDbType.NVarChar, Value = name });
                command.Parameters.Add(new SqlParameter { ParameterName = "@UserID", SqlDbType = SqlDbType.Int, Value = userId });

                var results = await command.ExecuteScalarAsync();

                if (!int.TryParse(results.ToString(), out int storageAreaId))
                {
                    throw new InvalidCastException("Unable to cast StorageAreaID returned from database to int");
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
							 [ID]           [int] IDENTITY(1, 1) NOT NULL, 
							 [DisplayName]  [nvarchar](255) NOT NULL UNIQUE, 
							 [ColumnTypeID] [int] NOT NULL,
							 [IsEditable]   [bit] NOT NULL, 
							 [CreatedOn]    [datetime] NOT NULL, 
							 [LastModified] [datetime] NOT NULL, 
							 PRIMARY KEY CLUSTERED ( [ID] ASC )WITH (STATISTICS_NORECOMPUTE = OFF, 
							 IGNORE_DUP_KEY = OFF) ON [PRIMARY] 
						  ) 
						ON [PRIMARY]; 

						ALTER TABLE [dbo].[{ storageAreaId }Columns]  WITH CHECK ADD  CONSTRAINT [FK__{ storageAreaId }Columns__ColumnTypeID] FOREIGN KEY([ColumnTypeID])
						REFERENCES [dbo].[ColumnTypes] ([ID])

						ALTER TABLE [dbo].[{ storageAreaId }Columns] CHECK CONSTRAINT [FK__{ storageAreaId }Columns__ColumnTypeID]";

                SqlCommand command = new SqlCommand(sql, connection);

                await command.ExecuteNonQueryAsync();

                Column column = new Column()
                {
                    StorageAreaId = storageAreaId,
                    IsEditable = false,
                    ColumnTypeId = 2,
                    DisplayName = "ID",
                    CreateColumnInRowsTable = false
                };
                await column.CreateColumnAsync();

                column.ColumnTypeId = 8;
                column.DisplayName = "Created On";
                column.CreateColumnInRowsTable = true;
                await column.CreateColumnAsync();
                column.DisplayName = "Last Modified";
                await column.CreateColumnAsync();
            }
        }
    }
}
