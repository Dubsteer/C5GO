namespace LogicLayer
{
    public interface IConnection
    {
        public void Open();

        public void Close();

        public dynamic GetInnerConn();
    }
}