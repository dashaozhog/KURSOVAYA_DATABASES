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
            dropBox = new ComboBox();
            connectButton = new Button();
            loadButton = new Button();
            dataView = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataView).BeginInit();
            SuspendLayout();
            // 
            // dropBox
            // 
            dropBox.DropDownStyle = ComboBoxStyle.DropDownList;
            dropBox.FormattingEnabled = true;
            dropBox.Location = new Point(12, 12);
            dropBox.Name = "dropBox";
            dropBox.Size = new Size(178, 23);
            dropBox.TabIndex = 0;
            // 
            // connectButton
            // 
            connectButton.Location = new Point(23, 401);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(75, 23);
            connectButton.TabIndex = 1;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // loadButton
            // 
            loadButton.Location = new Point(156, 401);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(75, 23);
            loadButton.TabIndex = 2;
            loadButton.Text = "Load";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Click += loadButton_Click;
            // 
            // dataView
            // 
            dataView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataView.Location = new Point(327, 84);
            dataView.Name = "dataView";
            dataView.Size = new Size(425, 245);
            dataView.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dataView);
            Controls.Add(loadButton);
            Controls.Add(connectButton);
            Controls.Add(dropBox);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ComboBox dropBox;
        private Button connectButton;
        private Button loadButton;
        private DataGridView dataView;
    }
}
