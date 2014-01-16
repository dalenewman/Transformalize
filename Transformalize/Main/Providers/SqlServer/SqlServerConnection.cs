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

using Transformalize.Configuration;

namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerConnection : AbstractConnection {
        public override string UserProperty { get { return "User Id"; } }
        public override string PasswordProperty { get { return "Password"; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return "Database"; } }
        public override string ServerProperty { get { return "Server"; } }
        public override string TrustedProperty { get { return "Trusted_Connection"; } }

        public SqlServerConnection(
            Process process,
            ConnectionConfigurationElement element,
            AbstractProvider provider,
            IConnectionChecker connectionChecker,
            IScriptRunner scriptRunner,
            IProviderSupportsModifier providerScriptModifer,
            IEntityRecordsExist recordsExist,
            IEntityDropper dropper,
            ITflWriter tflWriter,
            IViewWriter viewWriter)
            : base(
                element,
                provider,
                connectionChecker,
                scriptRunner,
                providerScriptModifer,
                recordsExist,
                dropper,
                tflWriter,
                viewWriter) {

            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];

            EntityKeysQueryWriter = process.Options.Top > 0 ? (IEntityQueryWriter)new SqlServerEntityKeysTopQueryWriter(process.Options.Top) : new SqlServerEntityKeysQueryWriter();
            EntityKeysRangeQueryWriter = new SqlServerEntityKeysRangeQueryWriter();
            EntityKeysAllQueryWriter = new SqlServerEntityKeysAllQueryWriter();
            TableQueryWriter = new SqlServerTableQueryWriter();
        }
    }
}