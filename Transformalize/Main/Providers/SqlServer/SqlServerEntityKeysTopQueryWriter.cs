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
    public class SqlServerEntityKeysTopQueryWriter : IEntityQueryWriter
    {
        private const string SQL_PATTERN = @"
            SELECT TOP {0} {1} FROM [{2}] WITH (NOLOCK);
        ";
        private readonly int _top;

        public SqlServerEntityKeysTopQueryWriter(int top)
        {
            _top = top;
        }

        public string Write(Entity entity)
        {
            var connection = entity.InputConnection;
            return string.Format(SQL_PATTERN, _top, string.Join(", ", entity.SelectKeys(connection.Provider)), entity.Name);
        }
    }
}