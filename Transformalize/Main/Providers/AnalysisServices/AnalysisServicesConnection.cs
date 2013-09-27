#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.AnalysisServices
{
    public class AnalysisServicesConnection : AbstractConnection
    {
        private const StringSplitOptions RE = StringSplitOptions.RemoveEmptyEntries;
        private readonly char[] _equal = new[] {'='};
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly char[] _semiColen = new[] {';'};

        public AnalysisServicesConnection(ConnectionConfigurationElement element, AbstractProvider provider, IConnectionChecker connectionChecker, IScriptRunner scriptRunner, IProviderSupportsModifier providerScriptModifer, IEntityRecordsExist recordsExist, IEntityDropper dropper)
            : base(element, provider, connectionChecker, scriptRunner, providerScriptModifer, recordsExist, dropper)
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