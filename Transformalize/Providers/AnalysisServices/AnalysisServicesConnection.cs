using System;
using Transformalize.Libs.NLog;

namespace Transformalize.Providers.AnalysisServices
{
    public class AnalysisServicesConnection : IConnection
    {
        private const StringSplitOptions RE = StringSplitOptions.RemoveEmptyEntries;
        private readonly IConnectionChecker _connectionChecker;
        private readonly char[] _semiColen = new[] { ';' };
        private readonly char[] _equal = new[] { '=' };
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public string Database { get; private set; }
        public string Server { get; private set; }
        public int CompatibilityLevel { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public int BatchSize { get; set; }
        public string ConnectionString { get; private set; }
        public string Process { get; set; }
        public IScriptRunner ScriptRunner { get; private set; }
        
        public AnalysisServicesConnection(string connectionString)
        {
            _connectionChecker = new AnalysisServicesConnectionChecker();
            ConnectionString = connectionString;
            CompatibilityLevel = 100;
            ConnectionType = ConnectionType.AnalysisServices;
            ParseConnectionString();
            ScriptRunner = new AnalysisServicesScriptRunner(this);
        }

        public bool IsReady()
        {
            return _connectionChecker.Check(ConnectionString);
        }

        private void ParseConnectionString()
        {
            foreach (var pair in ConnectionString.Split(_semiColen, RE))
            {
                try
                {
                    var attribute = pair.Split(_equal, RE)[0].Trim().ToLower();
                    var value = pair.Split(_equal, RE)[1].Trim();

                    if (attribute == "data source")
                    {
                        Server = value;
                    }

                    if (attribute == "catalog")
                    {
                        Database = value;
                    }

                }
                catch (Exception e)
                {
                    Server = string.Empty;
                    Database = string.Empty;
                    _log.Warn("{0} | Could not parse Analysis Services connection string: {1}.", Process, ConnectionString);
                    _log.Debug(e.Message);
                }
            }
        }

    }
}