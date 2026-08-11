namespace LogicLayer
{
    public interface IConnection : IDisposable
    {
        public void Open();

        public void Close();

        public dynamic GetInnerConn();
    }
}
