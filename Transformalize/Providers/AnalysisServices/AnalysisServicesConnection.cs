using System;
using System.Data;
using Transformalize.Core.Entity_;
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
        public string Provider { get; private set; }
        public int CompatibilityLevel { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public int BatchSize { get; set; }
        public string ConnectionString { get; private set; }
        public string Process { get; set; }
        public IScriptRunner ScriptRunner { get; private set; }

        public IDbConnection GetConnection()
        {
            throw new NotImplementedException();
        }

        public void LoadEndVersion(Entity entity)
        {
            throw new NotImplementedException();
        }

        public void LoadBeginVersion(Entity entity)
        {
            throw new NotImplementedException();
        }

        public AnalysisServicesConnection(string connectionString)
        {
            Provider = string.Empty;
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

        public string Name { get; set; }

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
                    _log.Warn("Could not parse Analysis Services connection string: {0}.", ConnectionString);
                    _log.Debug(e.Message);
                }
            }
        }

    }
}