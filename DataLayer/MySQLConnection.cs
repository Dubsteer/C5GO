using LogicLayer;
using MySql.Data.MySqlClient;

namespace DataAccessLayer
{
    public class MySQLConnection : IConnection
    {
        public MySqlConnection conn { get; set; }

        public MySQLConnection(string uri)
        {
            conn = new MySqlConnection(uri);
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