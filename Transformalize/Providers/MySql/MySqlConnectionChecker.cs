using System;
using System.Data;
using Transformalize.Libs.NLog;

namespace Transformalize.Providers.MySql
{
    public class MySqlConnectionChecker : IConnectionChecker
    {
        private readonly IConnection _connection;
        private readonly int _timeOut;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public MySqlConnectionChecker(IConnection connection, int timeOut = 3)
        {
            _connection = connection;
            _timeOut = timeOut;
        }

        public bool Check(string connectionString)
        {
            var result = false;

            var type = Type.GetType(_connection.Provider, false, true);
            if (type == null)
            {
                _log.Error("The type name '" + _connection.Provider + "' could not be found for " + _connection.Name + ".  Make sure the assembly or dll is available.");
                Environment.Exit(1);
            }
            using (var connection = (IDbConnection) Activator.CreateInstance(type))
            {
                connection.ConnectionString = _connection.ConnectionString.TrimEnd(";".ToCharArray()) + string.Format(";Connection Timeout={0};", _timeOut);
                try
                {
                    connection.Open();
                    result = connection.State == ConnectionState.Open;
                    if (result)
                    {
                        _log.Debug("Connection {0} is ready.", _connection.Name);
                    }
                    else
                    {
                        _log.Warn("Connection {0} is not responding.", _connection.Name);
                    }

                }
                catch (Exception e)
                {
                    _log.Error("Connection {0} caused error message: {1}", _connection.Name, e.Message);
                }

            }

            return result;

        }
    }
}