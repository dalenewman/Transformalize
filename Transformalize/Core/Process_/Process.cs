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

        public static Dictionary<string, Map> MapEquals = new Dictionary<string, Map>();
        public static Dictionary<string, Map> MapStartsWith = new Dictionary<string, Map>();
        public static Dictionary<string, Map> MapEndsWith = new Dictionary<string, Map>();
        public static Dictionary<string, Script> Scripts = new Dictionary<string, Script>();
        public static Dictionary<string, Template> Templates = new Dictionary<string, Template>();
        public static string Name { get; set; }
        public static Dictionary<string, IConnection> Connections = new Dictionary<string, IConnection>();
        public static bool OutputRecordsExist;
        public static List<Entity> Entities { get; set; }
        public static IFields CalculatedFields { get; set; }

        public Entity MasterEntity { get; set; }
        public static Options Options { get; set; }
        public List<Relationship> Relationships = new List<Relationship>();
        public IEnumerable<Field> RelatedKeys;
        public string View;
        
        public bool IsReady() {
            return Connections.Select(connection => connection.Value.IsReady()).All(b => b.Equals(true));
        }

        public Process() : this("TEST") {}

        public Process(string name)
        {
            Name = name;
            Entities = new List<Entity>();
            Options = new Options();
            CalculatedFields = new Fields();
        }

        public static IFields OutputFields()
        {
            var fields = new Fields();
            foreach (var entity in Entities)
            {
                fields.AddRange(new FieldSqlWriter(entity.All, entity.CalculatedFields).ExpandXml().Output().ToArray());
            }
            return fields;
        }

        public IParameters Parameters()
        {
            var parameters = new Parameters();

            foreach (var calculatedField in CalculatedFields)
            {
                if (calculatedField.Value.HasTransforms)
                {
                    foreach (AbstractTransform transform in calculatedField.Value.Transforms)
                    {
                        if (transform.HasParameters)
                        {
                            foreach (var parameter in transform.Parameters)
                            {
                                parameters[parameter.Key] = parameter.Value;
                            }
                        }
                    }
                }
            }
            return parameters;
        }
    }
}
