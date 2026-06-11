using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KURSOVAYA_DATABASES
{
    public class DataBaseManagement
    {
        public NpgsqlConnection Connection { get; private set; }

        public event EventHandler<ConnectionEventArgs> ConnectionChanged;

        private string connString;


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
            catch(Exception ex)
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

        



    }
}
