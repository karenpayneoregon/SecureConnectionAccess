using System;
using System.Data.OleDb;
using LoginLibrary.Models;

namespace LoginLibrary.Classes
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

                    cmd.Parameters.Add("@UserName", OleDbType.LongVarChar).Value = userName;
                    cmd.Parameters.Add("@UserPassword", OleDbType.LongVarChar).Value = userPassword;

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

                    cmd.Parameters.Add("@UserPassword", OleDbType.LongVarChar).Value = userPassword;
                    cmd.Parameters.Add("@UserName", OleDbType.LongVarChar).Value = userName;

                    cn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Try to login a user based on proper user name and password. IsAdmin
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// * You should (as done here) keep it a mystery why a login failed as a hacker may be attempting to hack your app
        /// * If by chance more that a true/false is needed use a named value tuple and alter the SELECT statement as needed
        /// </remarks>
        public bool Login()
        {

            if (!(string.IsNullOrWhiteSpace(UserName)) && !(string.IsNullOrWhiteSpace(UserPassword)))
            {
                using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
                {
                    using (var cmd = new OleDbCommand { Connection = cn })
                    {

                        cmd.CommandText = 
                            "SELECT UserName,UserPassword FROM Users WHERE UserName = @UserName AND UserPassword = @UserPassword";

                        cmd.Parameters.Add("@UserName", OleDbType.LongVarChar).Value = UserName;
                        cmd.Parameters.Add("@UserPassword", OleDbType.LongVarChar).Value = UserPassword;

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
                                throw;
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

        /// <summary>
        /// Same as above but returns two extra pieces of information into a named value tuple
        /// </summary>
        /// <returns>success and if valid user information</returns>
        public (bool success, User user) Login1()
        {

            if (!(string.IsNullOrWhiteSpace(UserName)) && !(string.IsNullOrWhiteSpace(UserPassword)))
            {
                using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
                {
                    using (var cmd = new OleDbCommand { Connection = cn })
                    {

                        cmd.CommandText =
                            "SELECT Identifer, UserName, UserPassword, IsAdmin FROM Users WHERE UserName = @UserName AND UserPassword = @UserPassword";

                        cmd.Parameters.Add("@UserName", OleDbType.LongVarChar).Value = UserName;
                        cmd.Parameters.Add("@UserPassword", OleDbType.LongVarChar).Value = UserPassword;

                        try
                        {
                            cn.Open();
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.ToLower().Contains("not a valid password"))
                            {
                                return (false, null);
                            }
                            else
                            {
                                throw;
                            }
                        }

                        var reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            User user = new User
                            {
                                Identifer = reader.GetInt32(0),
                                IsAdmin = reader.GetBoolean(1)
                            };
                            Retries = 0;
                            return (true, user);
                        }
                        else
                        {
                            Retries += 1;
                            return (false, null);
                        }
                    }
                }
            }
            else
            {
                return (false, null);
            }
        }
    }
}