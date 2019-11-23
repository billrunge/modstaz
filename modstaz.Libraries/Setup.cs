using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using static modstaz.Libraries.UserRole;

namespace modstaz.Libraries
{
    public class Setup
    {
        public string SystemEmailAddress { get; set; } = "SYSTEM";
        public ILogger Log { get; set; }
        public int StorageAreaId { get; set; }
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

        public async Task DropAllModstazTables()
        {
            StorageArea storageArea = new StorageArea();
            JArray storageAreas = (JArray)JsonConvert.DeserializeObject(await storageArea.GetStorsageAreasAsync());

            foreach (JObject s in storageAreas)
            {
                storageArea.StorageAreaId = (int)s["Id"];
                await storageArea.DeleteStorageAreaAsync();
            }

            List<string> tables = new List<string>
            {
                "StorageAreaAccess",
                "Roles",
                "ColumnTypes",
                "StorageAreas",
                "Users"
            };

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                foreach (string table in tables)
                {
                    SqlCommand command = new SqlCommand(CreateDropTableString(table), connection);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private string CreateDropTableString(string tableName)
        {
            return $@"
                    IF Object_id('{ tableName }', 'U') IS NOT NULL 
                      BEGIN 
                          DROP TABLE [{tableName}] 
                      END";

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
                               [Id]           [INT] IDENTITY(1, 1) NOT NULL, 
                               [EmailAddress] [NVARCHAR](255) NOT NULL UNIQUE, 
                               [CreatedOn]    [DATETIME] NOT NULL, 
                               CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH ( 
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
            string sql = $@"
                    IF Object_id('StorageAreas', 'U') IS NULL 
                      BEGIN 
                          CREATE TABLE [StorageAreas] 
                            ( 
                               [Id]           [INT] IDENTITY(1, 1) NOT NULL, 
                               [Name]         [NVARCHAR](255) NOT NULL, 
                               [CreatedBy]    [INT] FOREIGN KEY REFERENCES Users(Id), 
                               [CreatedOn]    [DATETIME] NOT NULL, 
                               [LastModified] [DATETIME] NOT NULL, 
                               CONSTRAINT [PK_StorageAreas] PRIMARY KEY CLUSTERED ( [Id] ASC ) WITH 
                               ( 
                               PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
                               ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
                            ) 
                          ON [PRIMARY] 
                      END";

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task CreateRolesTable()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                string sql = $@"
                    IF Object_id('Roles', 'U') IS NULL 
                      BEGIN 
                          CREATE TABLE [Roles] 
                            ( 
                               [Id]   [INT] IDENTITY(1, 1) NOT NULL, 
                               [Name] [NVARCHAR](255) NOT NULL, 
                               CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ( [Id] ASC ) WITH ( 
                               PAD_INDEX = 
                               OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
                               ALLOW_ROW_LOCKS = 
                               ON, ALLOW_PAGE_LOCKS = ON) 
                            ) 
                          ON [PRIMARY] 
                      END";

                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }   
        private async Task SeedRolesTable()
        {
            string values = string.Empty;

            UserRole userRole = new UserRole();
            List<Role> roles = userRole.RolesList;

            foreach (Role role in roles)
            {
                values += $@" ('{ role.Name }'),";
            }

            values = values.TrimEnd(',');

            string sql = $@"
                    IF Object_id('Roles', 'U') IS NOT NULL 
                       AND NOT EXISTS (SELECT TOP 1 [Id] 
                                       FROM   [Roles]) 
                      BEGIN 
                          INSERT INTO [Roles] 
                                      ([Name]) 
                          VALUES      { values }
                      END";

            Log.LogInformation(sql);

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task CreateStorageAreaAcessTable()
        {
            string sql = $@"
                    IF Object_id('StorageAreaAccess', 'U') IS NULL 
                      BEGIN 
                          CREATE TABLE [StorageAreaAccess] 
                            ( 
                               [Id]            [INT] IDENTITY(1, 1) NOT NULL, 
                               [UserId]        [INT] FOREIGN KEY REFERENCES Users(Id), 
                               [StorageAreaId] [INT] FOREIGN KEY REFERENCES StorageAreas(Id), 
                               [RoleId]        [INT] FOREIGN KEY REFERENCES Roles(Id), 
                               CONSTRAINT [PK_StorageAreaAccess] PRIMARY KEY CLUSTERED ( [Id] ASC ) 
                               WITH ( 
                               PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
                               ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
                            ) 
                          ON [PRIMARY] 
                      END";

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task CreateColumnTypesTable()
        {
            string sql = $@"
                    IF Object_id('ColumnTypes', 'U') IS NULL 
                      BEGIN 
                          CREATE TABLE [ColumnTypes] 
                            ( 
                               [Id]          [int] IDENTITY(1, 1) NOT NULL, 
                               [Name]        [NVARCHAR](255) NOT NULL, 
                               [SqlDataType] [NVARCHAR](50) NOT NULL,  CONSTRAINT [PK_ColumnTypes] 
                               PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, 
                               STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = 
                               ON, 
                               ALLOW_PAGE_LOCKS = ON) 
                            ) 
                          ON [PRIMARY] 
                      END";

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
        private async Task SeedColumnTypesTable()
        {
            string values = string.Empty;

            ColumnType columnType = new ColumnType();
            List<ColumnType.Type> types = columnType.TypesList;

            foreach (ColumnType.Type type in types)
            {
                values += $@" ('{ type.Name }', '{ type.SqlDataType }'),";
            }

            values = values.TrimEnd(',');

            string sql = $@"
                    IF Object_id('ColumnTypes', 'U') IS NOT NULL 
                       AND NOT EXISTS (SELECT TOP 1 [Id] 
                                       FROM   [ColumnTypes]) 
                      BEGIN 
                          INSERT INTO [ColumnTypes] 
                                      ([Name], 
                                       [SqlDataType]) 
                          VALUES      { values }
                      END";

            Log.LogInformation(sql);

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
