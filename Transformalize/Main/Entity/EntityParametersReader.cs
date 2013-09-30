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

namespace Transformalize.Main {
    public class EntityParametersReader : IParametersReader {
        private readonly Entity _entity;
        private readonly IParameters _parameters = new Parameters();

        public EntityParametersReader(Entity entity) {
            _entity = entity;
        }

        public IParameters Read() {
            var fields = new FieldSqlWriter(_entity.Fields).Input().ToArray();
            foreach (var field in fields) {
                _parameters.Add(field.Alias, field.Alias, null, field.Type);
            }
            return _parameters;
        }
    }
}