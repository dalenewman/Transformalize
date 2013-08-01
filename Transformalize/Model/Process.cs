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
using System.Linq;
using Transformalize.Data;

namespace Transformalize.Model {
    public class Process {

        public string Name;
        public Entity MasterEntity;
        public VersionOptions VersionOptions = new VersionOptions();
        public Dictionary<string, Field> Results = new Dictionary<string, Field>();
        public List<Entity> Entities = new List<Entity>();
        public Dictionary<string, SqlServerConnection> Connections = new Dictionary<string, SqlServerConnection>();
        public List<Relationship> Relationships = new List<Relationship>();
        public Dictionary<string, Dictionary<string, object>> MapEquals = new Dictionary<string, Dictionary<string, object>>();
        public Dictionary<string, Dictionary<string, object>> MapStartsWith = new Dictionary<string, Dictionary<string, object>>();
        public Dictionary<string, Dictionary<string, object>> MapEndsWith = new Dictionary<string, Dictionary<string, object>>();
        public IParameters Parameters = new Parameters();
        public IEnumerable<Field> RelatedKeys;
        public AbstractTransform[] Transforms;
        public string View;
        public bool OutputRecordsExist;
        public Dictionary<string, Script> Scripts = new Dictionary<string, Script>();

        public bool IsReady() {
            return Connections.Select(connection => connection.Value.IsReady()).All(b => b.Equals(true));
        }
    }
}
