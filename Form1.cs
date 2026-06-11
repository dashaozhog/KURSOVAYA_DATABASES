using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
namespace KURSOVAYA_DATABASES
{

    public partial class Form1 : Form
    {
        private DataBaseManagement DBman;
        private List<TabPage> tabPages = new List<TabPage>();
        public Form1()
        {
            InitializeComponent();
            DBman = new DataBaseManagement("Host=localhost;Port=5432;Username=postgres;Password=0979117981;Database=postgres");
            DBman.ConnectionChanged += OnConnChanged;

        }

        private async void connectButton_Click(object sender, EventArgs e)
        {

            await DBman.Connect();
            LoadTabs();

        }

        //private void loadButton_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        using (var conn = new NpgsqlConnection(connString))
        //        {
        //            conn.Open();
        //            string query = "SELECT * FROM patients";
        //            using (var cmd = new NpgsqlCommand(query, conn))
        //            using (var adapter = new NpgsqlDataAdapter(cmd))
        //            {
        //                DataTable table = new DataTable();
        //                adapter.Fill(table);
        //                dataView.DataSource = table;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("������� �� ��� ������������: " + ex.Message);
        //    }

        //}
        private void OnConnChanged(object sender, ConnectionEventArgs e)
        {
            if (e.IsConnected)
            {
                statusLabel.Text = "Connected";
                statusLabel.ForeColor = Color.Green;
                disconnectButton.Enabled = true;
                connectButton.Enabled = false;

                MessageBox.Show(e.Message, "Success!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                statusLabel.Text = "Disconnected";
                statusLabel.ForeColor = Color.Red;

                connectButton.Enabled = true;
                disconnectButton.Enabled = true;

                MessageBox.Show(e.Message, "No connection",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    private async void disconnectButton_Click(object sender, EventArgs e)
            {
                disconnectButton.Enabled = false;
                await DBman.Disconnect();
            }

        private void LoadTabs()
        {
            var TabPages = DBman.GetTableNames();
            int index = 0;
            tabControl1.Controls.Remove(tabPage1);
            foreach (var tab in TabPages)
            {
                TabPage tabP = new TabPage();
                tabP.Name = tab;
                tabP.Padding = new Padding(3);
                tabP.TabIndex = index;
                tabP.Text = char.ToUpper(tab[0]) + tab.Substring(1);
                tabP.UseVisualStyleBackColor = true;
                tabControl1.Controls.Add(tabP);
                tabPages.Add(tabP);
                index++;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }        
    }
}
