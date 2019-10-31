using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

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

        public StorageArea(int storageAreaId)
        {
            StorageAreaId = storageAreaId;
        }

        public StorageArea()
        {

        }

        async public Task CreateStorageAreaAsync()
        {
            StorageAreaId = await CreateStorageAreaIdAsync(StorageAreaName, UserId);
            await CreateRowsTableAsync(StorageAreaId);
            await CreateColumnsTableAsync(StorageAreaId);
            await AddAccessAsync(StorageAreaId, UserId);

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
							 [DisplayName]  [nvarchar](255) NOT NULL, 
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

        async Task AddAccessAsync(int storageAreaId, int userId)
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

                command.Parameters.Add(new SqlParameter { ParameterName = "@StorageAreaID", SqlDbType = SqlDbType.Int, Value = storageAreaId });
                command.Parameters.Add(new SqlParameter { ParameterName = "@UserID", SqlDbType = SqlDbType.Int, Value = userId });

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
