namespace KURSOVAYA_DATABASES
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            connectButton = new Button();
            loadButton = new Button();
            statusLabel = new Label();
            disconnectButton = new Button();
            tabControl1 = new TabControl();
            addButton = new Button();
            updateButton = new Button();
            deleteButton = new Button();
            clearButton = new Button();
            SuspendLayout();
            // 
            // connectButton
            // 
            connectButton.Location = new Point(1189, 56);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(75, 23);
            connectButton.TabIndex = 1;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // loadButton
            // 
            loadButton.Enabled = false;
            loadButton.Location = new Point(1189, 85);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(75, 23);
            loadButton.TabIndex = 2;
            loadButton.Text = "Load";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Click += loadButton_Click;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(1207, 38);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(39, 15);
            statusLabel.TabIndex = 5;
            statusLabel.Text = "Status";
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // disconnectButton
            // 
            disconnectButton.Enabled = false;
            disconnectButton.Location = new Point(1189, 114);
            disconnectButton.Name = "disconnectButton";
            disconnectButton.Size = new Size(75, 23);
            disconnectButton.TabIndex = 6;
            disconnectButton.Text = "Disconnect";
            disconnectButton.UseVisualStyleBackColor = true;
            disconnectButton.Click += disconnectButton_Click;
            // 
            // tabControl1
            // 
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1159, 623);
            tabControl1.TabIndex = 4;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            tabControl1.ControlAdded += tabControl1_ControlAdded;
            // 
            // addButton
            // 
            addButton.BackColor = SystemColors.HotTrack;
            addButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            addButton.ForeColor = SystemColors.HighlightText;
            addButton.Location = new Point(1181, 249);
            addButton.Name = "addButton";
            addButton.Size = new Size(105, 37);
            addButton.TabIndex = 7;
            addButton.Text = "Add";
            addButton.UseVisualStyleBackColor = false;
            addButton.Click += addButton_Click;
            // 
            // updateButton
            // 
            updateButton.BackColor = Color.LimeGreen;
            updateButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            updateButton.ForeColor = SystemColors.HighlightText;
            updateButton.Location = new Point(1181, 301);
            updateButton.Name = "updateButton";
            updateButton.Size = new Size(105, 37);
            updateButton.TabIndex = 8;
            updateButton.Text = "Update";
            updateButton.UseVisualStyleBackColor = false;
            // 
            // deleteButton
            // 
            deleteButton.BackColor = Color.Crimson;
            deleteButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            deleteButton.ForeColor = SystemColors.HighlightText;
            deleteButton.Location = new Point(1181, 356);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(105, 37);
            deleteButton.TabIndex = 9;
            deleteButton.Text = "Delete";
            deleteButton.UseVisualStyleBackColor = false;
            // 
            // clearButton
            // 
            clearButton.BackColor = Color.Black;
            clearButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            clearButton.ForeColor = SystemColors.HighlightText;
            clearButton.Location = new Point(1181, 409);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(105, 37);
            clearButton.TabIndex = 10;
            clearButton.Text = "Clear";
            clearButton.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1298, 647);
            Controls.Add(clearButton);
            Controls.Add(deleteButton);
            Controls.Add(updateButton);
            Controls.Add(addButton);
            Controls.Add(disconnectButton);
            Controls.Add(statusLabel);
            Controls.Add(connectButton);
            Controls.Add(loadButton);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button connectButton;
        private Button loadButton;
        private DataGridView dataView;
        private Label statusLabel;
        private Button disconnectButton;
        private TabPage tabPage1;
        private TabControl tabControl1;
        private TextBox idTextBox;
        private Label idLabel;
        private Button addButton;
        private Button updateButton;
        private Button deleteButton;
        private Button clearButton;
    }
}
