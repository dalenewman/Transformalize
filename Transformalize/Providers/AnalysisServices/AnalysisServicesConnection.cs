using System;
using Transformalize.Configuration;
using Transformalize.Core.Process_;
using Transformalize.Libs.NLog;

namespace Transformalize.Providers.AnalysisServices
{
    public class AnalysisServicesConnection : AbstractConnection
    {
        private const StringSplitOptions RE = StringSplitOptions.RemoveEmptyEntries;
        private readonly char[] _semiColen = new[] { ';' };
        private readonly char[] _equal = new[] { '=' };
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public AnalysisServicesConnection(ConnectionConfigurationElement element, AbstractProvider provider, AbstractConnectionChecker connectionChecker, IScriptRunner scriptRunner, IProviderSupportsModifier providerScriptModifer)
            : base(element, provider, connectionChecker, scriptRunner, providerScriptModifer)
        {
            ParseConnectionString();
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
                    _log.Warn("Could not parse Analysis Services connection string: {0}.", ConnectionString);
                    _log.Debug(e.Message);
                }
            }
        }

    }
}