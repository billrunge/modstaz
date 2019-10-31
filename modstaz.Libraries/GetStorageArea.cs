using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    public class GetStorageArea
    {
        public int StorageAreaId { get; set; }

        public GetStorageArea() { }

        public GetStorageArea(int storageAreaId)
        {
            StorageAreaId = storageAreaId;
        }

        public async Task<string> GetStorageAreaAsync()
        {
            //List<KeyValuePair<int, string>> idColumns = await GetColumnsAsync(StorageAreaId);
            return await GetRowsAsync(await GetColumnsAsync());
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
    }
}
