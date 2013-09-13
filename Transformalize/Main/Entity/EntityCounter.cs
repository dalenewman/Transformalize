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

using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main
{
    public class EntityCounter : WithLoggingMixin
    {
        private readonly IEntityCounter _entityCounter;
        private readonly Process _process;

        public EntityCounter(Process process, IEntityCounter entityCounter = null)
        {
            _process = process;
            _entityCounter = entityCounter ?? new SqlServerEntityCounter(new DefaultConnectionChecker());
        }

        public void Count()
        {
            foreach (var entity in _process.Entities)
            {
                entity.InputCount = _entityCounter.CountInput(entity);
                Info("Entity {0} input has {1} records.", entity.Alias, entity.InputCount);
                entity.OutputCount = _entityCounter.CountOutput(entity);
                Info("Entity {0} output has {1} records.", entity.Alias, entity.OutputCount);
            }
        }
    }
}