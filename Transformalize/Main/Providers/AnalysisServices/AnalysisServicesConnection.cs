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
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.AnalysisServices {

    public class AnalysisServicesConnection : AbstractConnection {

        public override string UserProperty { get { return string.Empty; } }
        public override string PasswordProperty { get { return string.Empty; } }
        public override string PortProperty { get { return string.Empty; } }
        public override string DatabaseProperty { get { return "Catalog"; } }
        public override string ServerProperty { get { return "Data Source"; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty { get { return string.Empty; } }
        public override int NextBatchId(string processName) {
            return 1;
        }

        public override void WriteEndVersion(AbstractConnection input, Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation EntityOutputKeysExtract(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation EntityOutputKeysExtractAll(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation EntityBulkLoad(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override IOperation EntityBatchUpdate(Entity entity) {
            throw new System.NotImplementedException();
        }

        public AnalysisServicesConnection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
            Type = ProviderType.AnalysisServices;
        }
    }
}