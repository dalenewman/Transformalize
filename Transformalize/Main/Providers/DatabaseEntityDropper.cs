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
using Transformalize.Libs.NLog;
using Transformalize.Libs.Dapper;

namespace Transformalize.Main.Providers
{
    public class DatabaseEntityDropper : IEntityDropper
    {
        public IEntityExists EntityExists { get; set; }
        private const string FORMAT = "DROP TABLE {0}{1};";
        private readonly Logger _log = LogManager.GetLogger(string.Empty);

        public DatabaseEntityDropper(IEntityExists entityExists)
        {
            EntityExists = entityExists;
        }

        public void Drop(AbstractConnection connection, string schema, string name)
        {
            if (!EntityExists.Exists(connection, schema, name))
                return;

            var provider = connection.Provider;
            var needSchema = NeedsSchema(schema);
            var sql = string.Format(FORMAT, needSchema ? provider.Enclose(schema) + "." : string.Empty , provider.Enclose(name));

            using (var cn = connection.GetConnection())
            {
                cn.Open();
                cn.Execute(sql);
                _log.Debug("Dropped Output {0}.{1}", schema, name);
            }
        }

        private static bool NeedsSchema(string schema)
        {
            return !(schema == string.Empty || schema.Equals("dbo", StringComparison.OrdinalIgnoreCase));
        }
    }
}