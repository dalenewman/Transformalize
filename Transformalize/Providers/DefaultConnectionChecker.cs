using System;
using System.Collections.Generic;
using System.Data;
using Transformalize.Libs.NLog;

namespace Transformalize.Providers
{
    
    public class DefaultConnectionChecker : AbstractConnectionChecker
    {
        private static readonly Dictionary<string, bool> CachedResults = new Dictionary<string, bool>();
        private readonly int _timeOut;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public DefaultConnectionChecker(int timeOut = 3)
        {
            _timeOut = timeOut;
        }

        public bool Check(AbstractConnection connection)
        {
            if (CachedResults.ContainsKey(connection.ConnectionString))
            {
                return CachedResults[connection.ConnectionString];
            }

            var result = false;
            try
            {
                using (var cs = connection.GetConnection())
                {
                    cs.ConnectionString = connection.ConnectionString.TrimEnd(";".ToCharArray()) + string.Format(";Connection Timeout={0};", _timeOut);
                    try
                    {
                        cs.Open();
                        result = cs.State == ConnectionState.Open;
                        if (result)
                        {
                            _log.Debug("{0} connection is ready.", connection.Name);
                        }
                        else
                        {
                            _log.Warn("{0} connection is not responding.", connection.Name);
                        }

                    }
                    catch (Exception e)
                    {
                        _log.Error("{0} connection caused error message: {1}", connection.Name, e.Message);
                    }

                }
            }
            catch (Exception)
            {
                _log.Error("{0} connection type '{1}' is unavailable.  Make sure the assembly (*.dll) is in the same folder as your executable.", connection.Name, connection.Provider);
                Environment.Exit(1);
            }

            CachedResults[connection.ConnectionString] = result;
            return result;

        }
    }
}