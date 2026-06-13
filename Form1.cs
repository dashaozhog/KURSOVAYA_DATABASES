using Npgsql;
using System;
using System.Data;
using System.Reflection.Emit;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Label = System.Windows.Forms.Label;
using TextBox = System.Windows.Forms.TextBox;
namespace KURSOVAYA_DATABASES
{

    public partial class Form1 : Form
    {
        private DataBaseManagement DBman;
        private List<TabPage> tabPages = new List<TabPage>();
        private float fieldFontSize = 14.25F;
        private Dictionary<TabPage, DataGridView> tabDataGridDict = new Dictionary<TabPage, DataGridView>();
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

        private void loadButton_Click(object sender, EventArgs e)
        {
            var tabname = tabControl1.SelectedTab;
            var table = DBman.LoadTableData(tabname.Name);

            tabDataGridDict[tabname].DataSource = table;

        }


        private void OnConnChanged(object sender, ConnectionEventArgs e)
        {
            if (e.IsConnected)
            {
                statusLabel.Text = "Connected";
                statusLabel.ForeColor = Color.Green;
                disconnectButton.Enabled = true;
                connectButton.Enabled = false;
                loadButton.Enabled = true;


                MessageBox.Show(e.Message, "Success!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                statusLabel.Text = "Disconnected";
                statusLabel.ForeColor = Color.Red;

                connectButton.Enabled = true;
                disconnectButton.Enabled = false;
                loadButton.Enabled = false;

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



        private void LoadTableFiels(TabPage tabpage)
        {
            int x = 24;
            int y = 71;
            var tableFields = DBman.GetTableFields(tabpage);
            foreach (var f in tableFields)
            {
                Label idLabel = new Label();
                idLabel.Text = f;
                tabpage.Controls.Add(idLabel);
                idLabel.Font = new Font("Cascadia Code", fieldFontSize, FontStyle.Regular, GraphicsUnit.Point, 204);
                idLabel.AutoSize = true;
                idLabel.Location = new Point(x, y);
                idLabel.Name = $"{f}Label";
                idLabel.TabIndex = 4;
                idLabel.Text = $"{f}: ";

                TextBox txtbox = new TextBox();
                txtbox.Location = new Point((int)(x + fieldFontSize * f.Length), y);
                txtbox.Name = $"{f}Box";
                txtbox.Size = new Size(100, 23);
                txtbox.TabIndex = 2;
                tabpage.Controls.Add(txtbox);

                y += 35;
            }
        }

        private void DataGridSetup(TabPage tabpage)
        {
            DataGridView dgv = new DataGridView();
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.BackgroundColor = Color.White;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.Font = new Font("Segoe UI", 9F);
            dgv.Location = new Point(400, 57);
            dgv.Name = $"dgv{tabpage.Name}";
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.Size = new Size(710, 460);
            dgv.TabIndex = 3;
            tabpage.Controls.Add(dgv);

            try
            { tabDataGridDict.Add(tabpage, dgv); }
            catch { }

        }

        private void TabLoad()
        {
            TabPage selectedPage = tabControl1.SelectedTab;

            LoadTableFiels(selectedPage);
            DataGridSetup(selectedPage);
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabLoad();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void tabControl1_ControlAdded(object sender, ControlEventArgs e)
        {
            TabLoad();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            DBman.Add("departments", "personal_id");
        }
    }
}
