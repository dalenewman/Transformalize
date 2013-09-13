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
using System.Collections.Generic;
using System.Data;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers
{
    public class DefaultConnectionChecker : AbstractConnectionChecker
    {
        private static readonly Dictionary<string, bool> CachedResults = new Dictionary<string, bool>();
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly int _timeOut;

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

            bool result = false;
            try
            {
                using (IDbConnection cs = connection.GetConnection())
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