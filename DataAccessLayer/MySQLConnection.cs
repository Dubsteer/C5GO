using LogicLayer;
using MySql.Data.MySqlClient;

namespace DataLayer
{
    public class MySQLConnection : IConnection
    {
        public MySqlConnection conn { get; set; }

        private const string defaultConnectionString =
            "server=127.0.0.1;port=3306;user id=root;password=1234;database=local_dtb;SslMode=None;AllowPublicKeyRetrieval=True;";

        public MySQLConnection()
        {
            conn = new MySqlConnection(defaultConnectionString);
        }

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
