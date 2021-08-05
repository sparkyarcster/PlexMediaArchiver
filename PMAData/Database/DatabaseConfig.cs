using System.Data.SQLite;

namespace PMAData.Database
{
    public class DatabaseConfig
    {
        private SQLiteConnection _connection;
        public string Name { get; set; }

        public SQLiteConnection Connection
        {
            get
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    if (_connection == null)
                    {
                        _connection = NewConnection();
                    }

                    return _connection;
                }

                return null;
            }
            set
            {
                _connection = value;
            }
        }

        public SQLiteConnection NewConnection()
        {
            return new SQLiteConnection($"Data Source={Name}.db");
        }

        public bool HasConnection
        {
            get
            {
                return Connection != null;
            }
        }

        public void Dispose()
        {
            if (HasConnection)
            {
                try
                {
                    Connection.Dispose();
                }
                catch
                {
                    //do nothing
                }

                Connection = null;
            }
        }
    }
}
