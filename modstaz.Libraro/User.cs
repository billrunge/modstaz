using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    class User
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
                    output      inserted.ID 
                    VALUES      (@EmailAddress, 
                                 Getutcdate())";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add(new SqlParameter { ParameterName = "@EmailAddress", SqlDbType = SqlDbType.NVarChar, Value = EmailAddress });

                var results = await command.ExecuteScalarAsync();

                if (!int.TryParse(results.ToString(), out int userId))
                {
                    throw new InvalidCastException("Unable to cast UserID returned from database to int");
                }
                else
                {
                    return userId;
                }
            }
        }
    }
}
