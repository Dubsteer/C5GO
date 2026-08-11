using MySql.Data.MySqlClient;

namespace DataLayer
{
    public sealed class MySQLConnection : IConnection
    {
        public MySQLConnection(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
        }

        public MySqlConnection Connection { get; }

        public void Open()
        {
            Connection.Open();
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
