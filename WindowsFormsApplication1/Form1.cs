using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.ExitThread();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder { Provider = "Microsoft.ACE.OLEDB.12.0", DataSource = System.IO.Path.Combine(Application.StartupPath, "Database1.accdb") };

            //
            // Our highly secure password :-)
            //
            Builder.Add("Jet OLEDB:Database Password", "password");

            using (OleDbConnection cn = new OleDbConnection { ConnectionString = Builder.ConnectionString })
            {
                using (OleDbCommand cmd = new OleDbCommand { Connection = cn, CommandText = "SELECT TOP 6 CompanyName FROM Customers;" })
                {
                    cn.Open();

                    dt.Load(cmd.ExecuteReader());
                    ListBox1.DisplayMember = "CompanyName";
                    ListBox1.DataSource = dt;
                }
            }
        }
    }
}