using LoginLibrary_CS;
using System;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class LoginForm : Form
    {
        private int Retrys = 0;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void cmdLogin_Click(object sender, EventArgs e)
        {
            if (Retrys == 2)
            {
                MessageBox.Show("Contact an admin");
                Application.ExitThread();
            }

            ApplicationLogin AppLogin = new ApplicationLogin 
                { 
                    Database = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database1.accdb"), 
                    DatabasePassword = "password", 
                    UserName = txtUserName.Text, 
                    UserPassword = txtPassword.Text 
                };

            if (AppLogin.Login())
            {
                Retrys = 0;
                this.Hide();
                Form1 f = new Form1();
                f.ShowDialog();
            }
            else
            {
                Retrys += 1;
                MessageBox.Show("Login failed");
            }
        }
    }
}