using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace modstaz.Libraries
{
    public class Row
    {
        public int StorageAreaId { get; set; }
        public ILogger Log { get; set; }
        public async Task<string> InsertRowAsync(JObject fields)
        {
            List<RowColumn> inputColumns = fields.Properties()
                .Select(x => new RowColumn { DisplayName = x.Name, Value = (string)x.Value })
                .ToList();

            Column column = new Column() { StorageAreaId = StorageAreaId };
            JArray columnObj = (JArray)JsonConvert.DeserializeObject(await column.GetStorageAreaColumnsAsync());

            List<RowColumn> columns = columnObj
                .Where(x => (bool)x["IsEditable"] == true)
                .Select(x => new RowColumn { ColumnId = (int)x["ID"], DisplayName = (string)x["DisplayName"], ColumnTypeId = (int)x["ColumnTypeID"] })
                .ToList();

            List<RowColumn> updateColumns = (from i in inputColumns
                                             from c in columns.Where(x => i.DisplayName.ToLower() == x.DisplayName.ToLower() || i.DisplayName == x.ColumnId.ToString())
                                             select new RowColumn() { ColumnId = c.ColumnId, DisplayName = c.DisplayName, Value = i.Value, ColumnTypeId = c.ColumnTypeId }).ToList();

            string columnIds = string.Empty;
            string values = string.Empty;

            foreach (RowColumn c in updateColumns)
            {
                Log.LogInformation($"Column ID: { c.ColumnId }, Column Type ID: { c.ColumnTypeId }");
                columnIds += $" [{ c.ColumnId }],";
                if (c.ColumnTypeId == 1)
                {
                    values += $"{ c.Value },";
                }
                else
                {
                    values += $"'{ c.Value }',";
                }
            }

            columnIds = columnIds.TrimEnd(',');
            values = values.TrimEnd(',');

            string sql = $@"
                INSERT INTO [{ StorageAreaId }ROWS] 
                            ([2], [3], { columnIds }) 
                VALUES      ( Getutcdate(), Getutcdate(), { values } ) ";

            Log.LogInformation(sql);

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }

            return sql;
        }

        public async Task<string> EditRowAsync(int rowId, JObject fields)
        {
            List<RowColumn> inputColumns = fields.Properties()
                .Select(x => new RowColumn { DisplayName = x.Name, Value = (string)x.Value })
                .ToList();

            Column column = new Column() { StorageAreaId = StorageAreaId };
            JArray columnObj = (JArray)JsonConvert.DeserializeObject(await column.GetStorageAreaColumnsAsync());

            List<RowColumn> columns = columnObj
                .Where(x => (bool)x["IsEditable"] == true)
                .Select(x => new RowColumn { ColumnId = (int)x["ID"], DisplayName = (string)x["DisplayName"] })
                .ToList();

            List<RowColumn> updateColumns = (from i in inputColumns
                                             from c in columns.Where(x => (i.DisplayName.ToLower() == x.DisplayName.ToLower() || i.DisplayName == x.ColumnId.ToString()))
                                             select new RowColumn() { ColumnId = c.ColumnId, DisplayName = c.DisplayName, Value = i.Value, ColumnTypeId = c.ColumnTypeId }).ToList();

            string values = string.Empty;

            foreach (RowColumn c in updateColumns)
            {
                values += $" [{ c.ColumnId }] = '{ c.Value }',";
            }
            values = values.TrimEnd(',');

            string sql = $@"
                UPDATE [{ StorageAreaId }Rows] 
                SET    { values } 
                WHERE  [1] = @RowID";

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter { ParameterName = "@RowID", SqlDbType = SqlDbType.Int, Value = rowId });
                await command.ExecuteNonQueryAsync();
            }

            return sql;
        }

        public async Task DeleteRowsByIdAsync(int[] rowIds)
        {
            string rowIdsString = "";

            foreach (int rowId in rowIds)
            {
                rowIdsString += $" { rowId.ToString() },";
            }
            rowIdsString = $"{rowIdsString.TrimEnd(',')}";

            string sql = $@"
                DELETE FROM [{ StorageAreaId }Rows]
                WHERE       [1] IN ({ rowIdsString })";

            using (SqlConnection connection = new SqlConnection() { ConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") })
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
            }
        }



        class RowColumn
        {
            public int ColumnId { get; set; }
            public int RowId { get; set; }
            public int ColumnTypeId { get; set; }
            public string DisplayName { get; set; }
            public string Value { get; set; }
        }
    }
}
