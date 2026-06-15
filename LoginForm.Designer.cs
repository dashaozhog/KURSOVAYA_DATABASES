namespace KURSOVAYA_DATABASES
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            errorLabel = new Label();
            passLabel = new Label();
            loginLabel = new Label();
            passwordBox = new TextBox();
            loginBox = new TextBox();
            submitButton = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Highlight;
            panel1.Controls.Add(errorLabel);
            panel1.Controls.Add(passLabel);
            panel1.Controls.Add(loginLabel);
            panel1.Controls.Add(passwordBox);
            panel1.Controls.Add(loginBox);
            panel1.Controls.Add(submitButton);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 450);
            panel1.TabIndex = 0;
            // 
            // errorLabel
            // 
            errorLabel.AutoSize = true;
            errorLabel.Font = new Font("Consolas", 18F, FontStyle.Bold, GraphicsUnit.Point, 204);
            errorLabel.ForeColor = Color.IndianRed;
            errorLabel.Location = new Point(334, 331);
            errorLabel.Name = "errorLabel";
            errorLabel.Size = new Size(25, 28);
            errorLabel.TabIndex = 5;
            errorLabel.Text = " ";
            errorLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // passLabel
            // 
            passLabel.AutoSize = true;
            passLabel.Font = new Font("Yang Bagus", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            passLabel.ForeColor = SystemColors.ButtonHighlight;
            passLabel.Location = new Point(317, 188);
            passLabel.Name = "passLabel";
            passLabel.Size = new Size(158, 32);
            passLabel.TabIndex = 4;
            passLabel.Text = "Password";
            // 
            // loginLabel
            // 
            loginLabel.AutoSize = true;
            loginLabel.Font = new Font("Yang Bagus", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            loginLabel.ForeColor = SystemColors.ButtonHighlight;
            loginLabel.Location = new Point(344, 117);
            loginLabel.Name = "loginLabel";
            loginLabel.Size = new Size(93, 32);
            loginLabel.TabIndex = 3;
            loginLabel.Text = "Login";
            // 
            // passwordBox
            // 
            passwordBox.Location = new Point(344, 223);
            passwordBox.Name = "passwordBox";
            passwordBox.PasswordChar = '*';
            passwordBox.Size = new Size(100, 23);
            passwordBox.TabIndex = 2;
            // 
            // loginBox
            // 
            loginBox.Location = new Point(344, 152);
            loginBox.Name = "loginBox";
            loginBox.Size = new Size(100, 23);
            loginBox.TabIndex = 1;
            // 
            // submitButton
            // 
            submitButton.Font = new Font("Super Black", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            submitButton.ForeColor = SystemColors.HotTrack;
            submitButton.Location = new Point(334, 266);
            submitButton.Name = "submitButton";
            submitButton.Size = new Size(120, 32);
            submitButton.TabIndex = 0;
            submitButton.Text = "Submit";
            submitButton.UseVisualStyleBackColor = true;
            submitButton.Click += submitButton_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel1);
            Name = "LoginForm";
            Text = "LoginForm";
            Load += LoginForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private TextBox loginBox;
        private Button submitButton;
        private Label loginLabel;
        private TextBox passwordBox;
        private Label passLabel;
        private Label errorLabel;
    }
}