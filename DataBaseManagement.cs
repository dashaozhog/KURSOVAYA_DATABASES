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
        public event EventHandler<DataEventArgs> DataChanged;


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

        public List<string> GetTableNames()
        {
            var tablesList = new List<string>();
            string sql = @"SELECT table_name 
                               FROM information_schema.tables 
                               WHERE table_schema = 'public' 
                               ORDER BY table_name;";

            using (var cmd = new NpgsqlCommand(sql, Connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tablesList.Add(reader.GetString(0));
                }

            }
            return tablesList;
        }

        public List<string> GetTableFields(TabPage tabpage)
        {
            var tableFields = new List<string>();
            if (tabpage != null) {

                string sql = $@"SELECT column_name
              FROM information_schema.columns
             WHERE table_schema = 'public'
               AND table_name   = '{tabpage.Name}';";
                using (var cmd = new NpgsqlCommand(sql, Connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tableFields.Add(reader.GetString(0));
                    }

                }

            }
            return tableFields;
        }

        public DataTable LoadTableData(string tableName)
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
        public string GetType(string tableName, string fieldName)
        {
            string query = $@"SELECT data_type
                            FROM information_schema.columns
                            WHERE table_name = '{tableName}'
                            AND column_name = '{fieldName}';";
            string type = "";
            using (var cmd = new NpgsqlCommand(query, Connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    type = reader.GetString(0);

                }

            }
            return type;
        }

        public async Task Add(string tableName, string fieldName, string data)
        {
            string query = "";
            if (GetType(tableName, fieldName) == "integer")
            {
                if (!int.TryParse(data, out int field))
                {
                    DataChanged?.Invoke(this, new DataEventArgs(
                        isSuccess: false,
                        message: "Data type is inappropriate"));
                    return;
                }
                query =
                        $"INSERT INTO {tableName} ({fieldName}) " +
                        $"VALUES (@{field})";
            }
            else {
                query =
                        $"INSERT INTO {tableName} ({fieldName}) " +
                        $"VALUES (@{data})";
            }


            using (var cmd = new NpgsqlCommand(query, Connection))
            {
                cmd.Parameters.AddWithValue(fieldName, data);
                int rows = cmd.ExecuteNonQuery();
                DataChanged?.Invoke(this, new DataEventArgs(
                    isSuccess: true,
                    message: "Data added successfully"));
            }

        }
    } 
}
