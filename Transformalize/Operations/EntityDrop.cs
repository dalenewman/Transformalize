/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using Transformalize.Data;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Model;

namespace Transformalize.Operations {
    public class EntityDrop : AbstractOperation {
        private readonly IEntityDropper _entityDropper;
        private readonly Entity _entity;

        public EntityDrop(Entity entity, IEntityDropper entityDropper = null) {
            _entity = entity;
            _entityDropper = entityDropper ?? new SqlServerEntityDropper();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            _entityDropper.DropOutput(_entity);
            return rows;
        }

    }
}