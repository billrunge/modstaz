using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    public class Setup
    {
        public string SystemEmailAddress { get; set; } = "SYSTEM";
        public async Task CreateTablesAsync()
        {
            User user = new User() { EmailAddress = SystemEmailAddress };
            await CreateUsersTable();
            await user.CreateUser();
            await CreateStorageAreasTable();
            await CreateRolesTable();
            await SeedRolesTable();
            await CreateStorageAreaAcessTable();
            await CreateColumnTypesTable();
            await SeedColumnTypesTable();
        }

        private async Task CreateUsersTable()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = $@"
                    IF Object_id('Users', 'U') IS NULL 
                      BEGIN 
                          CREATE TABLE [Users] 
                            ( 
                               [ID]           [INT] IDENTITY(1, 1) NOT NULL, 
                               [EmailAddress] [NVARCHAR](255) NOT NULL, 
                               [CreatedOn]    [DATETIME] NOT NULL, 
                               CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ( [ID] ASC )WITH ( 
                               PAD_INDEX 
                               = 
                               OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
                               ALLOW_ROW_LOCKS = 
                               ON, ALLOW_PAGE_LOCKS = ON) 
                            ) 
                          ON [PRIMARY] 
                      END";
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task CreateStorageAreasTable()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = $@"
                    IF Object_id('StorageAreas', 'U') IS NULL 
                      BEGIN 
                          CREATE TABLE [StorageAreas] 
                            ( 
                               [ID]           [int] IDENTITY(1, 1) NOT NULL, 
                               [Name]         [NVARCHAR](255) NOT NULL, 
                               [CreatedBy]    [INT] FOREIGN KEY REFERENCES Users(ID), 
                               [CreatedOn]    [DATETIME] NOT NULL, 
                               [LastModified] [DATETIME] NOT NULL, 
                               CONSTRAINT [PK_StorageAreas] PRIMARY KEY CLUSTERED ( [ID] ASC ) WITH 
                               ( 
                               PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
                               ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
                            ) 
                          ON [PRIMARY] 
                      END ";
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task CreateRolesTable()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = $@"
                    IF Object_id('Roles', 'U') IS NULL 
                      BEGIN 
                          CREATE TABLE [Roles] 
                            ( 
                               [ID]   [int] IDENTITY(1, 1) NOT NULL, 
                               [Name] [NVARCHAR](255) NOT NULL, 
                               CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ( [ID] ASC ) WITH ( 
                               PAD_INDEX = 
                               OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
                               ALLOW_ROW_LOCKS = 
                               ON, ALLOW_PAGE_LOCKS = ON) 
                            ) 
                          ON [PRIMARY] 
                      END ";
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task SeedRolesTable()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = $@"
                    IF Object_id('Roles', 'U') IS NOT NULL 
                       AND NOT EXISTS (SELECT * 
                                       FROM   [Roles]) 
                      BEGIN 
                          INSERT INTO [Roles] 
                                      ([Name]) 
                          VALUES      ('Super User'), 
                                      ('Creator'), 
                                      ('Delete'), 
                                      ('Edit'), 
                                      ('Add'), 
                                      ('View') 
                      END";
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task CreateStorageAreaAcessTable()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = $@"
                    IF Object_id('StorageAreaAccess', 'U') IS NULL 
                      BEGIN 
                          CREATE TABLE [StorageAreaAccess] 
                            ( 
                               [ID]            [int] IDENTITY(1, 1) NOT NULL, 
                               [UserID]        [INT] FOREIGN KEY REFERENCES Users(ID), 
                               [StorageAreaID] [INT] FOREIGN KEY REFERENCES StorageAreas(ID), 
                               [RoleID]        [INT] FOREIGN KEY REFERENCES Roles(ID), 
                               CONSTRAINT [PK_StorageAreaAccess] PRIMARY KEY CLUSTERED ( [ID] ASC ) 
                               WITH ( 
                               PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
                               ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
                            ) 
                          ON [PRIMARY] 
                      END";
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task CreateColumnTypesTable()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = $@"
                    IF Object_id('ColumnTypes', 'U') IS NULL 
                      BEGIN 
                          CREATE TABLE [ColumnTypes] 
                            ( 
                               [ID]          [int] IDENTITY(1, 1) NOT NULL, 
                               [Name]        [NVARCHAR](255) NOT NULL, 
                               [SqlDataType] [NVARCHAR](50) NOT NULL,  CONSTRAINT [PK_ColumnTypes] 
                               PRIMARY KEY CLUSTERED ( [ID] ASC )WITH (PAD_INDEX = OFF, 
                               STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = 
                               ON, 
                               ALLOW_PAGE_LOCKS = ON) 
                            ) 
                          ON [PRIMARY] 
                      END";
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task SeedColumnTypesTable()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = $@"
                    IF Object_id('ColumnTypes', 'U') IS NOT NULL 
                       AND NOT EXISTS (SELECT TOP 1 [ID] 
                                       FROM   [ColumnTypes]) 
                      BEGIN 
                          INSERT INTO [ColumnTypes] 
                                      ([Name], 
                                       [SqlDataType]) 
                          VALUES      ('Yes/No', 
                                       'bit'), 
                                      ('Integer', 
                                       'int'), 
                                      ('Decimal', 
                                       'float'), 
                                      ('Small Text', 
                                       'nvarchar(255)'), 
                                      ('Big Text', 
                                       'nvarchar(MAX)'), 
                                      ('GUID', 
                                       'uniqueidentifier'), 
                                      ('XML', 
                                       'xml'), 
                                      ('Date/Time', 
                                       'datetime') 
                      END";
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
