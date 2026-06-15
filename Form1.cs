using Npgsql;
using System;
using System.Data;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.Xml.Linq;
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
            if (tabname == null) return;

            var table = await DBman.LoadTableData(tabname.Name);

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
                addButton.Enabled = true;
                updateButton.Enabled = true;
                deleteButton.Enabled = true;
                clearButton.Enabled = true;

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
                addButton.Enabled = false;
                updateButton.Enabled = false;
                deleteButton.Enabled = false;


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

                int x2 = x + (int)(fieldFontSize * f.Length)+15;

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
                    txtbox.TabIndex = 10;
                    tabpage.Controls.Add(txtbox);

                    if (await DBman.isPrimary(tabpage.Name, f))
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

                if (type == "timestamp without time zone")
                {
                    dtp.Location = new Point(x2, y);
                    dtp.Name = $"{f}dateTime";
                    dtp.Size = new Size(200, 23);
                    dtp.TabIndex = 11;

                    tabpage.Controls.Add(dtp);
                }

                if (await DBman.isForeign(tabpage.Name, f))
                {
                    var forvals = await DBman.GetForeignValues(tabpage.Name, f);

                    var drop = new ComboBox();
                    drop.FormattingEnabled = true;
                    drop.Items.AddRange(forvals.ToArray());
                    drop.Location = new Point(x2, y);
                    drop.Name = $"{f}DropDown";
                    drop.Size = new Size(200, 23);
                    drop.TabIndex = 11;
                    drop.DropDownStyle = ComboBoxStyle.DropDownList;

                    drop.DataSource = new BindingSource(forvals, null);
                    drop.DisplayMember = "Value";
                    drop.ValueMember = "Key";

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
            dgv.CellClick += dgvClick;
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

            TabPage tab = tabControl1.SelectedTab;
            string table = tab.Name;

            var fieldValues = new Dictionary<string, string>();

            foreach (Control ctrl in tab.Controls)
            {
                if (ctrl is TextBox txt)
                {
                    string columnName = txt.Name.Replace("Box", "");

                    if (!string.IsNullOrWhiteSpace(columnName) && !string.IsNullOrWhiteSpace(txt.Text))
                    {
                        fieldValues[columnName] = txt.Text;
                    }
                }
                if (ctrl is DateTimePicker dtp)
                {
                    string columnName = dtp.Name.Replace("dateTime", "");
                    if (!string.IsNullOrWhiteSpace(dtp.Value.Date.ToString()) && !string.IsNullOrWhiteSpace(columnName))
                    {
                        fieldValues[columnName] = dtp.Value.Date.ToString();
                    }
                }
                if (ctrl is ComboBox drp)
                {
                    string columnName = drp.Name.Replace("DropDown", "");
                    if (!string.IsNullOrWhiteSpace(drp.SelectedValue.ToString()) && !string.IsNullOrWhiteSpace(columnName))
                    {
                        fieldValues[columnName] = drp.SelectedValue.ToString();
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
            await ClearFields();

        }

        private async void dgvClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridView grid = (DataGridView)sender;
            TabPage tab = tabControl1.SelectedTab;
            DataGridViewRow row = grid.Rows[e.RowIndex];

            foreach (Control ctrl in tab.Controls)
            {
                if (ctrl is TextBox txt)
                {
                    string columnName = txt.Name.Replace("Box", "");
                    if (grid.Columns.Contains(columnName) && row.Cells[columnName].Value != null)
                    {
                        txt.Text = row.Cells[columnName].Value.ToString();
                    }
                }

                else if (ctrl is ComboBox cb)
                {
                    string columnName = cb.Name.Replace("DropDown", "");
                    if (grid.Columns.Contains(columnName) && row.Cells[columnName] != null)
                    {
                        cb.SelectedValue = row.Cells[columnName].Value.ToString();
                    }
                }

                else if (ctrl is DateTimePicker dtp)
                {
                    string columnName = dtp.Name.Replace("dateTime", "");
                    if (grid.Columns.Contains(columnName) && row.Cells[columnName].Value is DateTime dtValue)
                    {
                        dtp.Value = dtValue;
                    }
                }
            }
        }

        private async void updateButton_Click(object sender, EventArgs e)
        {
            TabPage tab = tabControl1.SelectedTab;
            if (tab == null) return;

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
                else if (ctrl is ComboBox cb)
                {
                    string columnName = cb.Name.Replace("DropDown", "");
                    if (cb.SelectedValue != null)
                    {
                        fieldValues[columnName] = cb.SelectedValue.ToString();
                    }
                }
                else if (ctrl is DateTimePicker dtp)
                {
                    string columnName = dtp.Name.Replace("dateTime", "");
                    fieldValues[columnName] = dtp.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            if (fieldValues.Count == 0)
            {
                MessageBox.Show("No data found to update!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await DBman.Update(table, fieldValues);
                MessageBox.Show("Row updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                loadButton_Click(this, EventArgs.Empty);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Invalid value type: " + ex.Message, "Type Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update transaction failed: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            await ClearFields();

        }

        private async void deleteButton_Click(object sender, EventArgs e)
        {
            TabPage tab = tabControl1.SelectedTab;
            if (tab == null) return;

            string PKval = "";

            string table = tab.Name;
            foreach (Control ctrl in tab.Controls)
            {
                if (ctrl is TextBox txt && await DBman.isPrimary(table, txt.Name.Replace("Box", "")))
                {
                    PKval = txt.Text;
                }
            }
            var result = MessageBox.Show(
                "Are you sure brothuh?",
                "Deletion confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try { await DBman.Delete(table, PKval); }
            catch (Exception ex)
            {
                MessageBox.Show("Error while deleting: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            loadButton_Click(this, EventArgs.Empty);

            await ClearFields();


        }

        private async Task ClearFields()
        {
            var tab = tabControl1.SelectedTab;

            foreach (Control ctrl in tab.Controls)
            {
                if (ctrl is TextBox txt)
                {
                    txt.Clear();

                }
                else if (ctrl is DateTimePicker dtp)
                {
                    dtp.Value = DateTime.Now;
                }
                else if (ctrl is ComboBox cb)
                {
                    cb.ResetText();
                }
            }
        }

        private async void clearButton_Click(object sender, EventArgs e)
        {
            await ClearFields();

        }
    }
}
    

