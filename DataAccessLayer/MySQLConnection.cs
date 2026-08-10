using LogicLayer;
using MySql.Data.MySqlClient;

namespace DataLayer
{
    public class MySQLConnection : IConnection
    {
        public MySqlConnection conn { get; set; }

        public MySQLConnection(string connectionString)
        {
            conn = new MySqlConnection(connectionString);
        }

        public void Open()
        {
            conn.Open();
        }

        public void Close()
        {
            conn.Close();
        }

        public dynamic GetInnerConn()
        {
            return conn;
        }
    }
}
