using LogicLayer;
using MySql.Data.MySqlClient;

namespace DataLayer
{
    public class MySQLConnection : IConnection
    {
        public MySqlConnection conn { get; set; }

        // Default local connection string (your local DB)
        private const string defaultConnectionString =
            "server=127.0.0.1;port=3306;user id=root;password=1234;database=local_dtb;SslMode=none;";

        // ✅ Constructor without parameters (uses local DB)
        public MySQLConnection()
        {
            conn = new MySqlConnection(defaultConnectionString);
        }

        // ✅ Constructor with connection string parameter (optional override)
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
