using MySql.Data.MySqlClient;
using System;

namespace netasloc.Data
{
    public class MySqlDatabase : IDisposable
    {
        public MySqlConnection Connection { get; set; }

        public MySqlDatabase(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
            Connection.Open();
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
