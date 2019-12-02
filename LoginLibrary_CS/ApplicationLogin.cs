using System;

using System.Data.OleDb;

namespace LoginLibrary_CS
{
    public class ApplicationLogin
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Database { get; set; }
        public string DatabasePassword { get; set; }
        public int Retries { get; set; }

        /// <summary>
        /// Setup connection, this could be hardwired
        /// in app.config and encrypted.
        /// </summary>
        public ApplicationLogin()
        {
            Database = "database1.accdb";
            DatabasePassword = "password";
            _builder.Add("Jet OLEDB:Database Password", DatabasePassword);
            _builder.DataSource = Database;
        }

        private OleDbConnectionStringBuilder _builder = new OleDbConnectionStringBuilder
            { Provider = "Microsoft.ACE.OLEDB.12.0" };

        /// <summary>
        /// FORUM RESPONSE ONLY
        /// Add a user to the database
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        public void AddUser(string userName, string userPassword)
        {
            using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
            {
                using (var cmd = new OleDbCommand { Connection = cn })
                {
                    cmd.CommandText = "INSERT INTO Users (UserName, UserPassword) VALUES (@UserName,@UserPassword) ";
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@UserPassword", userPassword);

                    cn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// FORUM RESPONSE ONLY
        /// Update user password by user name.
        /// Generally speaking when a user log in we would possible remember their
        /// primary key in the database table so we can find their record better then
        /// by user name.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        public void UpdatePassword(string userName, string userPassword)
        {
            using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
            {
                using (var cmd = new OleDbCommand { Connection = cn })
                {
                    cmd.CommandText = "UPDATE Users SET UserPassword = @UserPassword WHERE UserName = @UserPassword";
                    cmd.Parameters.AddWithValue("@UserPassword", userPassword);
                    cmd.Parameters.AddWithValue("@UserName", userName);

                    cn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Try to login a user based on proper user name and password.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// You should (as done here) keep it a mystery why a login failed as a hacker
        /// may be attempting to hack your app
        /// </remarks>
        public bool Login()
        {

            if (!(string.IsNullOrWhiteSpace(UserName)) && !(string.IsNullOrWhiteSpace(UserPassword)))
            {
                using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
                {
                    using (var cmd = new OleDbCommand { Connection = cn, CommandText = "SELECT UserName,UserPassword FROM Users WHERE UserName = @UserName AND UserPassword = @UserPassword" })
                    {
                        cmd.Parameters.AddWithValue("@UserName", UserName);
                        cmd.Parameters.AddWithValue("@UserPassword", UserPassword);

                        try
                        {
                            cn.Open();
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.ToLower().Contains("not a valid password"))
                            {
                                return false;
                            }
                            else
                            {
                                throw ex;
                            }
                        }

                        var reader = cmd.ExecuteScalar();
                        if (reader != null)
                        {
                            Retries = 0;
                            return true;
                        }
                        else
                        {
                            Retries += 1;
                            return false;
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}