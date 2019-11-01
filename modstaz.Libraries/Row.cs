using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    public class Row
    {
        public int StorageAreaId { get; set; }

        public async Task<string> InsertRowAsync(JObject fields)
        {
            List<RowColumn> inputColumns = fields.Properties()
                .Select(x => new RowColumn { DisplayName = x.Name, Value = (string)x.Value })
                .ToList();

            Column column = new Column() { StorageAreaId = StorageAreaId };
            JArray columnObj = (JArray)JsonConvert.DeserializeObject(await column.GetStorageAreaColumnsAsync());

            List<RowColumn> columns = columnObj
                .Where(x => (bool)x["IsEditable"] == true)
                .Select(x => new RowColumn { Id = (int)x["ID"], DisplayName = (string)x["DisplayName"] })
                .ToList();

            List<RowColumn> updateColumns = (from i in inputColumns
                                                from c in columns.Where(x => i.DisplayName.ToLower() == x.DisplayName.ToLower() || i.DisplayName == x.Id.ToString())
                                                select new RowColumn() { Id = c.Id, DisplayName = c.DisplayName, Value = i.Value }).ToList();

            string columnIds = "";
            string values = "";

            foreach (RowColumn c in updateColumns)
            {
                columnIds += $" [{ c.Id }],";
                values += $"'{ c.Value }',";
            }

            columnIds = columnIds.TrimEnd(',');
            values = values.TrimEnd(',');

            string sql = $@"
                INSERT INTO [{ StorageAreaId }ROWS] 
                ( { columnIds } )
                VALUES ( { values } )";

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();

            }

            return sql;

        }

        class RowColumn
        {
            public int Id { get; set; }
            public string DisplayName { get; set; }
            public string Value { get; set; }
        }








    }
}
