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

namespace Transformalize.Main.Providers.MySql {
    public class MySqlConnection : AbstractConnection {

        public override string UserProperty { get { return "Uid"; } }
        public override string PasswordProperty { get { return "Pwd"; } }
        public override string PortProperty { get { return "Port"; } }
        public override string DatabaseProperty { get { return "Database"; } }
        public override string ServerProperty { get { return "Server"; } }
        public override string TrustedProperty { get { return string.Empty; } }
        public override string PersistSecurityInfoProperty {
            get { return string.Empty; }
        }

        public MySqlConnection(Process process, ConnectionConfigurationElement element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];
        }

        public override string KeyAllQuery(Entity entity) {
            const string sql = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT {0} FROM `{1}`;
                COMMIT;
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(Provider)),
                entity.Name
            );
        }

        public override string KeyQuery(Entity entity) {
            const string sql = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT {0}
                FROM `{1}`
                WHERE `{2}` <= @End;
                COMMIT;
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(Provider)),
                entity.Name,
                entity.Version.Name
                );

        }

        public override string KeyRangeQuery(Entity entity) {

            const string sql = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT {0}
                FROM `{1}`
                WHERE `{2}` BETWEEN @Begin AND @End;
                COMMIT;
            ";

            return string.Format(
                sql,
                string.Join(", ", entity.SelectKeys(Provider)),
                entity.Name,
                entity.Version.Name
            );
        }

        public override string KeyTopQuery(Entity entity, int top) {
            const string sql = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT {0} FROM `{1}` LIMIT 0, {2};
                COMMIT;
            ";
            return string.Format(sql, string.Join(", ", entity.SelectKeys(Provider)), entity.Name, top);
        }
    }
}