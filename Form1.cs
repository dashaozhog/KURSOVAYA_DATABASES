using Npgsql;
using System;
using System.Data;
using System.Reflection.Emit;
using System.Windows.Forms;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;
using Label = System.Windows.Forms.Label;
using TextBox = System.Windows.Forms.TextBox;


//выпадающее меню на вторичные ключи
//выбор даты на дату
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
            //DBman.DataChanged += 
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {

            await DBman.Connect();
            await LoadTabs();

        }

        private async void loadButton_Click(object sender, EventArgs e)
        {
            var tabname = tabControl1.SelectedTab;
             var table =await DBman.LoadTableData(tabname.Name);

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

        private async Task LoadTabs()
        {
            var TabPages = await DBman.GetTableNames();
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



        private async Task LoadTableFiels(TabPage tabpage)
        {
            int x = 24;
            int y = 71;
            var tableFields = await DBman.GetTableFields(tabpage);
            
            foreach (var f in tableFields)
            {
                string type = await DBman.GetColumnType(tabpage.Name, f);

                Label idLabel;
                TextBox txtbox = new TextBox();
                DateTimePicker dtp = new DateTimePicker();

                int x2 = x + (int)(fieldFontSize * f.Length);

                idLabel = new Label();
                idLabel.Text = f;
                tabpage.Controls.Add(idLabel);
                idLabel.Font = new Font("Cascadia Code", fieldFontSize, FontStyle.Regular, GraphicsUnit.Point, 204);
                idLabel.AutoSize = true;
                idLabel.Location = new Point(x, y);
                idLabel.Name = $"{f}Label";
                idLabel.TabIndex = 4;
                idLabel.Text = $"{f}: ";

                if (!await DBman.isForeign(tabpage.Name, f) && type != "timestamp without time zone")
                {
                    txtbox.Location = new Point(x2, y);
                txtbox.Name = $"{f}Box";
                txtbox.Size = new Size(100, 23);
                txtbox.TabIndex = 2;
                tabpage.Controls.Add(txtbox);

                if(await DBman.isPrimary(tabpage.Name, f))
                {
                    txtbox.Enabled = false;
                }

                if (f.ToLower() == "digital_sign" || f.ToLower().Contains("uuid"))
                {
                    
                    txtbox.ReadOnly = true;
                    Button btnGenerate = new Button();
                    btnGenerate.Text = "Generate";
                    btnGenerate.AutoSize = true;
                    btnGenerate.Location = new Point(x2 + txtbox.Width + 10, y - 2);
                    tabpage.Controls.Add(btnGenerate);

                    btnGenerate.Click += (sender, e) =>
                    {
                        txtbox.Text = Guid.NewGuid().ToString();
                    };
                    }
                }

                if (type=="timestamp without time zone")
                {
                    dtp.Location = new Point(x2, y);
                    dtp.Name = $"{f}dateTime";
                    dtp.Size = new Size(200, 23);
                    dtp.TabIndex = 11;

                    tabpage.Controls.Add(dtp);
                }

                if(await DBman.isForeign(tabpage.Name, f))
                {
                    var forvals = await DBman.GetForeignValues(tabpage.Name, f);
                    
                    var drop = new ComboBox();
                    drop.FormattingEnabled = true;
                    drop.Items.AddRange(forvals);
                    drop.Location = new Point(x2, y);
                    drop.Name = $"{f}DropDown";
                    drop.Size = new Size(121, 23);
                    drop.TabIndex = 11;

                    tabpage.Controls.Add(drop);
                }

                y += 35;
            }
        }

        private async Task DataGridSetup(TabPage tabpage)
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

        private async Task TabLoad()
        {
            TabPage selectedPage = tabControl1.SelectedTab;
            if (selectedPage == null) return;

            if (selectedPage.Controls.Count > 0) return;

            await LoadTableFiels(selectedPage);
            await DataGridSetup(selectedPage);
        }
        private async void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
                     
            await TabLoad();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void tabControl1_ControlAdded(object sender, ControlEventArgs e)
        {
            

        }

        private async void addButton_Click(object sender, EventArgs e)
        {
            {
                TabPage tab = tabControl1.SelectedTab;
                string table = tab.Name;

                var fieldValues = new Dictionary<string, string>();

                foreach (Control ctrl in tab.Controls)
                {
                    if (ctrl is TextBox txt)
                    {
                        string columnName = txt.Name.Replace("Box", "");

                        if (!string.IsNullOrWhiteSpace(txt.Text))
                        {
                            fieldValues[columnName] = txt.Text;
                        }
                    }
                }


                if (fieldValues.Count == 0)
                {
                    MessageBox.Show("Fill at least one field!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    await DBman.Add(table, fieldValues);
                    MessageBox.Show("Row added successfully!", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (FormatException ex)
                {
                    MessageBox.Show("Invalid value: " + ex.Message, "Type Error",
           MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Insert failed: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
    

