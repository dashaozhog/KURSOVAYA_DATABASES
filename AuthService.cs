using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KURSOVAYA_DATABASES
{
    public class AuthService
    {
        private readonly DataBaseManagement DBman;

        public event EventHandler<LoginEventArgs> LoginCompleted;

        public AuthService(DataBaseManagement dbman)
        {
            DBman = dbman;
        }

        private async Task<string> GetPassword(string login)
        {
            if (DBman.Connection == null || DBman.Connection.State != ConnectionState.Open)
            {
                await DBman.Connect();
            }

            using var cmd = new NpgsqlCommand(
                "SELECT user_password FROM employees WHERE login = @login LIMIT 1",
                DBman.Connection
            );
                cmd.Parameters.AddWithValue("@login", login);

                var result = await cmd.ExecuteScalarAsync();
                return result as string;
            

        }

        public async Task Login(string login, string password)
        {
            try
            {
                string storedPass = await GetPassword(login);

                if (storedPass == null)
                {
                    LoginCompleted?.Invoke(this, new LoginEventArgs(
                        success: false,
                        message: "Wrong login or password",
                        login: null));
                }

                bool isValid = storedPass.Equals(password);

                if (isValid)
                {
                    LoginCompleted?.Invoke(this, new LoginEventArgs(
                        success: true,
                        message: "Login completed",
                        login: login));
                }
                else
                {
                    LoginCompleted?.Invoke(this, new LoginEventArgs(
                        success: false,
                        message: "Wrong login or password",
                        login: null));
                }

            }
            catch(Exception ex)
            {
                LoginCompleted?.Invoke(this, new LoginEventArgs(
                        success: false,
                        message: "Error: "+ex.Message,
                        login: null));
            }
        }
    }
}
