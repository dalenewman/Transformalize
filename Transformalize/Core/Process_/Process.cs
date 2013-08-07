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
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Template_;
using Transformalize.Core.Transform_;
using Transformalize.Providers;

namespace Transformalize.Core.Process_ {

    public class Process {

        public string Name { get; set; }
        public Entity MasterEntity { get; set; }
        public Options Options { get; set; }
        public IFields Results { get; set; }
        public List<Entity> Entities { get; set; }

        public Dictionary<string, IConnection> Connections = new Dictionary<string, IConnection>();
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
        public Dictionary<string, Template> Templates = new Dictionary<string, Template>();

        public bool IsReady() {
            return Connections.Select(connection => connection.Value.IsReady()).All(b => b.Equals(true));
        }

        public Process() : this("TEST") {}

        public Process(string name)
        {
            Name = name;
            Results = new Fields();
            Entities = new List<Entity>();
            Options = new Options(name);
        }

        public IFields InputFields()
        {
            var fields = new Fields();
            foreach (var entity in Entities)
            {
                fields.AddRange(new FieldSqlWriter(entity.All).ExpandXml().Input().Context());
            }
            return fields;
        }
    }
}
