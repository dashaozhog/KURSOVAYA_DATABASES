using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KURSOVAYA_DATABASES
{
    public class DataBaseManagement
    {
        public NpgsqlConnection Connection { get; private set; }

        public event EventHandler<ConnectionEventArgs> ConnectionChanged;
        //public event EventHandler<DataEventArgs> DataChanged;


        public string connString;


        public DataBaseManagement(string connString)
        {
            this.connString = connString;
        }

        public async Task Connect()
        {
            try
            {

                Connection = new NpgsqlConnection(connString);
                await Connection.OpenAsync();

                ConnectionChanged?.Invoke(this, new ConnectionEventArgs(
                    isConnected: true,
                    message: "Connection successful!"));
            }
            catch (Exception ex)
            {
                Connection?.Dispose();
                Connection = null;

                ConnectionChanged?.Invoke(this, new ConnectionEventArgs(
                    isConnected: false,
                    message: "Connection failed: " + ex.Message));
            }
        }

        public async Task Disconnect()
        {
            await Connection.CloseAsync();

            try
            {
                ConnectionChanged?.Invoke(this, new ConnectionEventArgs(
                    isConnected: false,
                    message: "Disonnected successfully!"));
                Connection?.Dispose();
            }
            catch (Exception ex)
            {
                Connection?.Dispose();
                Connection = null;

                ConnectionChanged?.Invoke(this, new ConnectionEventArgs(
                    isConnected: false,
                    message: "Disconnection failed: " + ex.Message));
            }
        }

        public async Task<List<string>> GetTableNames()
        {
            var tablesList = new List<string>();
            string sql = @"SELECT table_name 
                               FROM information_schema.tables 
                               WHERE table_schema = 'public' 
                               ORDER BY table_name;";

            using (var cmd = new NpgsqlCommand(sql, Connection))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    tablesList.Add(reader.GetString(0));
                }

            }
            return tablesList;
        }

        public async Task<List<string>> GetTableFields(TabPage tabpage)
        {
            var tableFields = new List<string>();
            if (tabpage != null) {

                string sql = $@"SELECT column_name
              FROM information_schema.columns
             WHERE table_schema = 'public'
               AND table_name   = '{tabpage.Name}';";
                using (var cmd = new NpgsqlCommand(sql, Connection))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tableFields.Add(reader.GetString(0));
                    }

                }

            }
            return tableFields;
        }

        public async Task<DataTable> LoadTableData(string tableName)
        {
            string query = $"SELECT * FROM {tableName}";
            using (var cmd = new NpgsqlCommand(query, Connection))
            using (var adapter = new NpgsqlDataAdapter(cmd))
            {

                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public async Task<string> GetColumnType(string tableName, string columnName)
        {
            string colType = "";
            string sql = @"SELECT data_type 
                   FROM information_schema.columns
                   WHERE table_schema = 'public' AND table_name = @tableName AND column_name=@columnName";

            using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("@tableName", tableName);
            cmd.Parameters.AddWithValue("@columnName", columnName);


            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                colType = reader.GetString(0);

            return colType;
        }

        public async Task<Dictionary<string, string>> GetColumnTypes(string tableName)
        {
            var columnTypes = new Dictionary<string, string>();

            string sql = @"SELECT column_name, data_type 
                   FROM information_schema.columns
                   WHERE table_schema = 'public' AND table_name = @tableName";

            using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("@tableName", tableName);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                columnTypes[reader.GetString(0)] = reader.GetString(1);

            return columnTypes;
        }

        public async Task<bool> Add(string tableName, Dictionary<string, string> fieldValues)
        {
            var columnTypes = await GetColumnTypes(tableName);

            var columns = string.Join(", ", fieldValues.Keys.Select(k => $"\"{k}\""));
            var paramNames = string.Join(", ", Enumerable.Range(0, fieldValues.Count).Select(i => $"@p{i}"));

            string query = $"INSERT INTO \"{tableName}\" ({columns}) VALUES ({paramNames});";

            using (var cmd = new NpgsqlCommand(query, Connection))
            {
                int index = 0;


                foreach (var kvp in fieldValues)
                {
                    string columnName = kvp.Key;
                    string rawValue = kvp.Value;

                    columnTypes.TryGetValue(columnName, out string pgType);

                    object typedValue = ConvertValue(rawValue, pgType);

                    cmd.Parameters.AddWithValue($"@p{index}", typedValue ?? DBNull.Value);

                    index++;
                }

                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }

        private object ConvertValue(string rawValue, string pgType)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return DBNull.Value; 

            switch (pgType)
            {
                case "integer":
                case "smallint":
                case "bigint":
                    if (int.TryParse(rawValue, out int intVal))
                        return intVal;
                    throw new FormatException($"Expected a whole number, got: '{rawValue}'");

                case "numeric":
                case "real":
                case "double precision":
                    if (double.TryParse(rawValue, out double dblVal))
                        return dblVal;
                    throw new FormatException($"Expected a decimal number, got: '{rawValue}'");

                case "boolean":
                    if (bool.TryParse(rawValue, out bool boolVal))
                        return boolVal;
                    throw new FormatException($"Expected true/false, got: '{rawValue}'");

                case "date":
                    if (DateTime.TryParse(rawValue, out DateTime dateVal))
                        return dateVal;
                    throw new FormatException($"Expected a date (e.g. 2024-01-31), got: '{rawValue}'");

                case "timestamp without time zone":
                case "timestamp with time zone":
                    if (DateTime.TryParse(rawValue, out DateTime tsVal))
                        return tsVal;
                    throw new FormatException($"Expected a timestamp, got: '{rawValue}'");

                case "character varying":
                case "text":
                case "char":
                default:
                    return rawValue; 
            }
        }

        public  async Task<bool> isPrimary(string tableName, string columnName)
        {            
            string sql = $@"SELECT kcu.column_name
FROM information_schema.table_constraints tc
JOIN information_schema.key_column_usage kcu
     ON tc.constraint_name = kcu.constraint_name
     AND tc.table_schema = kcu.table_schema
WHERE tc.constraint_type = 'PRIMARY KEY'
  AND tc.table_name = '{tableName}'
  AND kcu.column_name = '{columnName}';
";

            using var cmd = new NpgsqlCommand(sql, Connection);
            string i = "";
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                 i = reader.GetString(0);
            if(i!="") return true;
            return false;
        }

        public async Task<bool> isForeign(string tableName, string columnName)
        {
            string sql = $@"SELECT kcu.column_name
FROM information_schema.table_constraints tc
JOIN information_schema.key_column_usage kcu
     ON tc.constraint_name = kcu.constraint_name
     AND tc.table_schema = kcu.table_schema
WHERE tc.constraint_type = 'FOREIGN KEY'
  AND tc.table_name = '{tableName}'
  AND kcu.column_name = '{columnName}';
";

            using var cmd = new NpgsqlCommand(sql, Connection);
            string i = "";
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                i = reader.GetString(0);
            if (i != "") return true;
            return false;
        }

        //        public async Task<List<string>> GetForeignValues(string tableName, string columnName) {

        //            string refTable = "";
        //            var dict = new List<string>();
        //            string sql = $@"SELECT ccu.table_name AS foreign_table
        //FROM information_schema.table_constraints AS tc
        //JOIN information_schema.key_column_usage AS kcu
        //  ON tc.constraint_name = kcu.constraint_name
        //JOIN information_schema.constraint_column_usage AS ccu
        //  ON ccu.constraint_name = tc.constraint_name
        //WHERE tc.constraint_type = 'FOREIGN KEY'
        //  AND tc.table_name = '{tableName}' 
        //  AND kcu.column_name = '{columnName}'; ";

        //            using var cmd = new NpgsqlCommand(sql, Connection);
        //            using var reader = await cmd.ExecuteReaderAsync();
        //            while(await reader.ReadAsync())
        //            {
        //                refTable = reader.GetString(0);
        //            }
        //            await reader?.CloseAsync();
        //             string sql2 = $@"SELECT c.column_name, c.data_type
        //FROM information_schema.table_constraints tc 
        //JOIN information_schema.constraint_column_usage AS ccu USING (constraint_schema, constraint_name) 
        //JOIN information_schema.columns AS c ON c.table_schema = tc.constraint_schema
        //  AND tc.table_name = c.table_name AND ccu.column_name = c.column_name
        //WHERE constraint_type = 'PRIMARY KEY' and tc.table_name = '{refTable}'; ";

        //            string column = "";

        //            using var cmd2 = new NpgsqlCommand(sql2, Connection);
        //            using var reader2 = await cmd2.ExecuteReaderAsync();
        //            while (await reader2.ReadAsync())
        //                column = reader2.GetValue(0).ToString();
        //            await reader2?.CloseAsync();

        //            string sql3 = $@"SELECT {column} from {refTable}";

        //            using var cmd3 = new NpgsqlCommand(sql3, Connection);
        //            using var reader3 = await cmd3.ExecuteReaderAsync();
        //            while (await reader3.ReadAsync())
        //                dict.Add(reader3.GetValue(0).ToString());
        //            await reader3?.CloseAsync();
        //            return dict;
        //        }

        public async Task<Dictionary<string, string>> GetForeignValues(string tableName, string columnName)
        {
            string refTable = "";
            var dict = new Dictionary<string, string>();

            
            string sql = $@"SELECT ccu.table_name AS foreign_table
    FROM information_schema.table_constraints AS tc
    JOIN information_schema.key_column_usage AS kcu
      ON tc.constraint_name = kcu.constraint_name
    JOIN information_schema.constraint_column_usage AS ccu
      ON ccu.constraint_name = tc.constraint_name
    WHERE tc.constraint_type = 'FOREIGN KEY'
      AND tc.table_name = '{tableName}' 
      AND kcu.column_name = '{columnName}'; ";

            using (var cmd = new NpgsqlCommand(sql, Connection))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    refTable = reader.GetString(0);
                }
            } 

            if (string.IsNullOrEmpty(refTable)) return dict;

           
            string sql2 = $@"SELECT kcu.column_name
    FROM information_schema.table_constraints tc 
    JOIN information_schema.key_column_usage AS kcu 
      ON tc.constraint_name = kcu.constraint_name AND tc.table_schema = kcu.table_schema
    WHERE tc.constraint_type = 'PRIMARY KEY' AND tc.table_name = '{refTable}'; ";

            string pkColumn = "";
            using (var cmd2 = new NpgsqlCommand(sql2, Connection))
            using (var reader2 = await cmd2.ExecuteReaderAsync())
            {
                while (await reader2.ReadAsync())
                    pkColumn = reader2.GetValue(0).ToString();
            }

            if (string.IsNullOrEmpty(pkColumn)) return dict;

           
            string sql3 = $@"SELECT column_name 
    FROM information_schema.columns 
    WHERE table_schema = 'public' AND table_name = '{refTable}'
    ORDER BY ordinal_position LIMIT 1 OFFSET 2;";

            string textColumn = "";
            using (var cmd3 = new NpgsqlCommand(sql3, Connection))
            using (var reader3 = await cmd3.ExecuteReaderAsync())
            {
                while (await reader3.ReadAsync())
                    textColumn = reader3.GetValue(0).ToString();
            }

           
            if (string.IsNullOrEmpty(textColumn)) textColumn = pkColumn;

            
            string sql4 = $"SELECT \"{pkColumn}\", \"{textColumn}\" FROM \"{refTable}\";";

            using (var cmd4 = new NpgsqlCommand(sql4, Connection))
            using (var reader4 = await cmd4.ExecuteReaderAsync())
            {
                while (await reader4.ReadAsync())
                {
                    string key = reader4.GetValue(0).ToString();
                    string name = reader4.IsDBNull(1) ? "No Name" : reader4.GetValue(1).ToString();

                    string value = $"[{key}] {name}";

                    if (!dict.ContainsKey(key))
                    {
                        dict.Add(key, value);
                    }
                }
            }

            return dict;
        }
        private async Task<string> GetPrimaryKeyColumn(string tableName){
            string sql = @"SELECT kcu.column_name
                   FROM information_schema.table_constraints tc
                   JOIN information_schema.key_column_usage kcu
                        ON tc.constraint_name = kcu.constraint_name
                        AND tc.table_schema = kcu.table_schema
                   WHERE tc.constraint_type = 'PRIMARY KEY'
                     AND tc.table_name = @tableName;";
            using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("tableName", tableName);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetString(0);
            }
            return null;
        }
    
        public async Task Update(string tableName, Dictionary<string,string> fieldValues)
        {
            string primaryKey = await GetPrimaryKeyColumn(tableName);
            if (string.IsNullOrEmpty(primaryKey))
            {
                throw new Exception($"Cannot update table '{tableName}' because it doesn't have a primary key defined.");
            }

            if(!fieldValues.ContainsKey(primaryKey))
            {
                throw new Exception($"Primary key value for '{primaryKey}' is missing from the data update payload.");
            }

            string primaryKeyValue = fieldValues[primaryKey];

            var setters = new List<string>();
            foreach(var kvp in fieldValues)
            {
                if (kvp.Key.Equals(primaryKey, StringComparison.OrdinalIgnoreCase))
                    continue;

                setters.Add($"\"{kvp.Key}\" = @{kvp.Key}");
            }

            if (setters.Count == 0) return;

            string setSql = string.Join(", ", setters);
            string sql = $"UPDATE \"{tableName}\" SET {setSql} WHERE \"{primaryKey}\" = @pk_value;";

            using var cmd = new NpgsqlCommand(sql, Connection);

            var columnTypes = await GetColumnTypes(tableName);

            foreach (var kvp in fieldValues)
            {
                if (kvp.Key.Equals(primaryKey, StringComparison.OrdinalIgnoreCase))
                    continue;

                string type = columnTypes.ContainsKey(kvp.Key) ? columnTypes[kvp.Key] : "text";
                object typedValue = ConvertValue(kvp.Value, type);

                cmd.Parameters.AddWithValue(kvp.Key, typedValue ?? DBNull.Value);
            }

            string pkType = columnTypes.ContainsKey(primaryKey) ? columnTypes[primaryKey] : "integer";
            cmd.Parameters.AddWithValue("pk_value", ConvertValue(primaryKeyValue, pkType));

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(string tableName, string primaryKeyValue)
        {

            var PKname = await GetPrimaryKeyColumn(tableName);

            if (string.IsNullOrEmpty(PKname))
            {
                throw new Exception($"No primary key detected!!");
            }

            string query = $"DELETE FROM \"{tableName}\" WHERE {PKname} = @id";
            using (var cmd = new NpgsqlCommand(query, Connection))
            {
                var columnTypes = await GetColumnTypes(tableName);
                string type = columnTypes.ContainsKey(PKname) ? columnTypes[PKname] : "integer";
                object typedValue = ConvertValue(primaryKeyValue, type);

                cmd.Parameters.AddWithValue("id", typedValue ?? DBNull.Value);
                int rows = await cmd.ExecuteNonQueryAsync();

                if (rows == 0)
                {
                    throw new Exception("No record was found with the specified ID.");
                }
            }
        }

   
        
    }
} 

