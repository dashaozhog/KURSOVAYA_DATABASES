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

            var columns = string.Join(", ", fieldValues.Keys);
            var paramNames = string.Join(", ", fieldValues.Keys.Select(k => "@" + k));
            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({paramNames})";

            using var cmd = new NpgsqlCommand(query, Connection);

            foreach (var kvp in fieldValues)
            {
                string columnName = kvp.Key;
                string rawValue = kvp.Value;

                
                columnTypes.TryGetValue(columnName, out string pgType);

                
                object typedValue = ConvertValue(rawValue, pgType);
                cmd.Parameters.AddWithValue("@" + columnName, typedValue);
            }

            await cmd.ExecuteNonQueryAsync();

            //DataChanged?.Invoke(this, new DataEventArgs(
            //    isSuccess: true,
            //    message: $"Row inserted into {tableName}"
            //));

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

        public async Task<List<int>> GetForeignValues(string tableName, string columnName) {

            string refTable = "";
            var dict = new List<string>();
            string sql = $@"SELECT ccu.table_name AS foreign_table
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY'
  AND tc.table_name = '{tableName}' 
  AND kcu.column_name = '{columnName}'; ";

            using var cmd = new NpgsqlCommand(sql, Connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                refTable = reader.GetString(0);
            }
            reader?.DisposeAsync();
             string sql2 = $@"SELECT * from {tableName}  ";

            using var cmd2 = new NpgsqlCommand(sql2, Connection);
            using var reader2 = await cmd2.ExecuteReaderAsync();
            while (await reader2.ReadAsync())
                dict.Add(reader2.GetString(0));

                return dict;
        }
    } 
}
