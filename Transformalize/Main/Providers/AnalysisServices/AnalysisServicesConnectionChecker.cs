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
using Microsoft.AnalysisServices;

namespace Transformalize.Main.Providers.AnalysisServices {
    public class AnalysisServicesConnectionChecker : IConnectionChecker {

        public bool Check(AbstractConnection connection) {
            bool isReady;
            var server = new Server();
            try {
                server.Connect(connection.GetConnectionString());
                isReady = server.Connected;
                server.Disconnect();
            } catch (Exception e) {
                TflLogger.Warn(string.Empty, string.Empty, "Error checking {0} connection. {1}", connection.Name, e.Message);
                return false;
            }
            return isReady;
        }
    }
}