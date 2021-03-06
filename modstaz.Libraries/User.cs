﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    public class User
    {
        public string EmailAddress { get; set; }

        public User() { }


        public async Task<int> CreateUser()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    INSERT INTO [Users] 
                                ([EmailAddress], 
                                 [CreatedOn]) 
                    output      inserted.Id
                    VALUES      (@EmailAddress, 
                                 Getutcdate())";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add(new SqlParameter { ParameterName = "@EmailAddress", SqlDbType = SqlDbType.NVarChar, Value = EmailAddress });

                var results = await command.ExecuteScalarAsync();

                if (!int.TryParse(results.ToString(), out int userId))
                {
                    throw new InvalidCastException("Unable to cast UserId returned from database to int");
                }
                else
                {
                    return userId;
                }
            }
        }

        public async Task<bool> DoesUserExistAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    SELECT CASE 
                             WHEN EXISTS (SELECT [Id] 
                                          FROM   [Users] 
                                          WHERE  [EmailAddress] = @EmailAddress) THEN Cast(1 AS BIT) 
                             ELSE Cast(0 AS BIT) 
                           END";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add(new SqlParameter { ParameterName = "@EmailAddress", SqlDbType = SqlDbType.NVarChar, Value = EmailAddress });

                return (bool)await command.ExecuteScalarAsync();
            }
        }

        public async Task<int> GetUserIdByEmailAddressAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    SELECT TOP 1 [Id] 
                    FROM   [Users] 
                    WHERE  [EmailAddress] = @EmailAddress";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add(new SqlParameter { ParameterName = "@EmailAddress", SqlDbType = SqlDbType.NVarChar, Value = EmailAddress });

                return (int)await command.ExecuteScalarAsync();
            }

        }

    }
}
