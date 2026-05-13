namespace TronBeTongV3.CSDL
{
    public class DbConfig
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Db { get; set; }
        public string User { get; set; }
        public string Pw { get; set; }

        public DbConfig(string srv, int port, string db, string user, string pw)
        {
            Server = srv;
            Port = port;
            Db = db;
            User = user;
            Pw = pw;
        }
    }
}
