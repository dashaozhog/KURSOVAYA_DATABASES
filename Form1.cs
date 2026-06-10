using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
namespace KURSOVAYA_DATABASES
{

    public partial class Form1 : Form
    {
        private string connString =
        "Host=localhost;" +
        "Port=5432;" +
        "Username=postgres;" +
        "Password=0979117981;" +
        "Database=postgres";
        public Form1()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    MessageBox.Show("З'єднання встановлено успішно!",
                    "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка підключення: " + ex.Message,
                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT * FROM patients";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        dataView.DataSource = table;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка під час завантаження: " + ex.Message);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Вибираємо всі таблиці з поточної схеми (наприклад, public)
                string sql = @"SELECT table_name 
                               FROM information_schema.tables 
                               WHERE table_schema = 'public' 
                               ORDER BY table_name;";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dropBox.Items.Add(reader.GetString(0));
                    }
                }
            }
        }
    }
}
