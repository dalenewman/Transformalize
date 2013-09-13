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

namespace Transformalize.Main.Providers.SqlServer
{
    public class SqlServerEntityKeysQueryWriter : IEntityQueryWriter
    {
        private const string SQL_PATTERN = @"
                SELECT {0}
                FROM [{1}].[{2}] WITH (NOLOCK)
                WHERE [{3}] <= @End
            ";

        public string Write(Entity entity)

        {
            AbstractConnection connection = entity.InputConnection;
            return string.Format(
                SQL_PATTERN,
                string.Join(", ", entity.SelectKeys(connection.Provider)),
                entity.Schema,
                entity.Name,
                entity.Version.Name
                );
        }
    }
}