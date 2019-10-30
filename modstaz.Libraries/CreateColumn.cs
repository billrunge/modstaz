using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    public class CreateColumn
    {
        public string DisplayName { get; set; }
        public int StorageAreaId { get; set; }
        public int ColumnTypeId { get; set; }
        public bool IsEditable { get; set; } = false;
        public int ColumnId { get; private set; }
        public string SqlDataType { get; private set; }
        public bool CreateColumnInRowsTable { get; set; } = true;

        public CreateColumn(string displayName, int storageAreaId, int columnTypeId, bool isEditable)
        {
            DisplayName = displayName;
            StorageAreaId = storageAreaId;
            ColumnTypeId = columnTypeId;
            IsEditable = isEditable;
        }
        public CreateColumn() { }

        public async Task CreateColumnAsync()
        {
            ColumnId = 0;
            SqlDataType = String.Empty;
            ColumnId = await CreateColumnIdAsync();
            SqlDataType = await GetSqlDataTypeFromColumnTypeId();
            if (CreateColumnInRowsTable)
            {
                await CreateColumnInRowTableAsync();
            }
        }

        async Task<int> CreateColumnIdAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();

                string sql = $@"
                    INSERT INTO [{ StorageAreaId }Columns] 
                                ([DisplayName], 
                                 [ColumnTypeID], 
                                 [IsEditable], 
                                 [CreatedOn], 
                                 [LastModified]) 
                    VALUES      (@DisplayName, 
                                 @ColumnType, 
                                 { (IsEditable ? 1 : 0) }, 
                                 Getutcdate(), 
                                 Getutcdate());


                    SELECT SCOPE_IDENTITY(); ";

                SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add(new SqlParameter { ParameterName = "@DisplayName", SqlDbType = SqlDbType.NVarChar, Value = DisplayName });
                command.Parameters.Add(new SqlParameter { ParameterName = "@ColumnType", SqlDbType = SqlDbType.Int, Value = ColumnTypeId });

                return Convert.ToInt32(await command.ExecuteScalarAsync());

            }
        }

        async Task CreateColumnInRowTableAsync()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = $@"
                    ALTER TABLE [{ StorageAreaId }Rows] 
                      ADD [{ColumnId}] { SqlDataType }";
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }


        async Task<string> GetSqlDataTypeFromColumnTypeId()
        {
            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                string sql = @"
                    SELECT [SqlDataType] 
                    FROM   [ColumnTypes] 
                    WHERE  ID = @ColumnTypeID";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@ColumnTypeID", SqlDbType = SqlDbType.Int, Value = ColumnTypeId });

                return (string)await command.ExecuteScalarAsync();
            }
        }

    }
}
