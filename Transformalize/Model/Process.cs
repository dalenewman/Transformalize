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
using Transformalize.Transforms;

namespace Transformalize.Model {

    public class Process {
        private Dictionary<string, Field> _fields;

        public string Name;
        public Entity MasterEntity;
        public Dictionary<string, Connection> Connections = new Dictionary<string, Connection>();
        public Dictionary<string, Entity> Entities = new Dictionary<string, Entity>();
        public List<Relationship> Relationships = new List<Relationship>();
        public Dictionary<string, Dictionary<string, object>> MapEquals = new Dictionary<string, Dictionary<string, object>>();
        public Dictionary<string, Dictionary<string, object>> MapStartsWith = new Dictionary<string, Dictionary<string, object>>();
        public Dictionary<string, Dictionary<string, object>> MapEndsWith = new Dictionary<string, Dictionary<string, object>>();
        public ITransform[] Transforms { get; set; }
        public Dictionary<string, Field> Parameters = new Dictionary<string, Field>();
        public Dictionary<string, Field> Results = new Dictionary<string, Field>();
        public IEnumerable<Field> RelatedKeys;
        public string View { get; set; }
        public bool OutputRecordsExist { get; set; }

        public bool IsReady() {
            return Connections.Select(connection => connection.Value.IsReady()).All(b => b.Equals(true));
        }
    }
}
