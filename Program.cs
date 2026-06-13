namespace KURSOVAYA_DATABASES
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();

            var dbService = new DataBaseManagement("Host=localhost;Database=postgres;Username=postgres;Password=0979117981");
            var authService = new AuthService(dbService);

            // Connect first, then show login
            dbService.Connect().GetAwaiter().GetResult();

            Application.Run(new LoginForm(authService));
        }
    }
}