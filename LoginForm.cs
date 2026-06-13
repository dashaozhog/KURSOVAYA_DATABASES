using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KURSOVAYA_DATABASES
{
    public partial class LoginForm : Form
    {
        private AuthService authservice;
        public LoginForm(AuthService authservice)
        {
            InitializeComponent();
            this.authservice = authservice;
            this.authservice.LoginCompleted += OnLoginCompleted;

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private async void submitButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(loginBox.Text) ||
            string.IsNullOrWhiteSpace(passwordBox.Text))
            {
                errorLabel.Text = "Fill all fields!!!";
                return;
            }
            submitButton.Enabled = false;
            errorLabel.Text = "Checking///";

            await authservice.Login(loginBox.Text, passwordBox.Text);
        }

        private void OnLoginCompleted(object sender, LoginEventArgs e)
        {
            if (e.Success)
            {
                this.Hide();
                Form1 form1 = new Form1();
                form1.FormClosed += (s, args) => this.Close();

                form1.Show();

            }
            else
            {
                errorLabel.Text = e.Message;
                passwordBox.Clear();
                submitButton.Enabled = true;
            }
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            authservice.LoginCompleted -= OnLoginCompleted;
            base.OnFormClosed(e);
        }
    }
}
