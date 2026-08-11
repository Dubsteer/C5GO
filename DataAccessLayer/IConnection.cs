using MySql.Data.MySqlClient;

namespace DataLayer
{
    public interface IConnection : IDisposable
    {
        MySqlConnection Connection { get; }

        void Open();
    }
}
