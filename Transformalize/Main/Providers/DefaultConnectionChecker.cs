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

namespace Transformalize.Main.Providers {
    public class DefaultConnectionChecker : IConnectionChecker {

        protected static readonly Dictionary<string, bool> CachedResults = new Dictionary<string, bool>();
        protected readonly Logger Log = LogManager.GetLogger(string.Empty);
        private readonly int _timeOut;

        public DefaultConnectionChecker(int timeOut = 3) {
            _timeOut = timeOut;
        }

        public bool Check(AbstractConnection connection) {
            if (CachedResults.ContainsKey(connection.Name)) {
                return CachedResults[connection.Name];
            }

            return CheckConnection(connection);
        }

        protected bool CheckConnection(AbstractConnection connection) {
            var result = false;
            try {
                using (var cn = connection.GetConnection()) {

                    if (connection.Provider.Supports.ConnectionTimeout) {
                        cn.ConnectionString = connection.GetConnectionString().TrimEnd(";".ToCharArray()) + string.Format(";Connection Timeout={0};", _timeOut);
                    } else {
                        cn.ConnectionString = connection.GetConnectionString();
                    }

                    try {
                        cn.Open();
                        result = cn.State == ConnectionState.Open;
                        if (result) {
                            Log.Debug("{0} connection is ready.", connection.Name);
                        } else {
                            Log.Warn("{0} connection is not responding.", connection.Name);
                        }
                    } catch (Exception e) {
                        Log.Error("{0} connection caused error message: {1}", connection.Name, e.Message);
                    }
                }
            } catch (Exception ex) {
                Log.Error("{0} connection type '{1}' is unavailable.  Make sure the assembly (*.dll) is in the same folder as your executable. Error Message: {2}", connection.Name, connection.Provider, ex.Message);
                Environment.Exit(1);
            }

            CachedResults[connection.Name] = result;
            return result;
        }
    }
}