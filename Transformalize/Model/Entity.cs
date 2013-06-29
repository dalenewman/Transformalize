using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Model {

    public class Entity : WithLoggingMixin, IDisposable {

        public string Schema { get; set; }
        public string ProcessName { get; set; }
        public string Name { get; set; }
        public Connection InputConnection { get; set; }
        public Connection OutputConnection { get; set; }
        public Field Version;
        public Dictionary<string, Field> PrimaryKey { get; set; }
        public Dictionary<string, Field> Fields { get; set; }
        public Dictionary<string, Field> Xml { get; set; }
        public Dictionary<string, Field> All { get; set; }
        public Dictionary<string, Relationship> Joins { get; set; }
        public EntitySqlWriter EntitySqlWriter { get; private set; }
        public string Output { get; set; }
        public bool Processed { get; set; }
        public int RecordsAffected { get; set; }
        public object End { get; set; }
        public int TflId { get; set; }

        public Entity() {
            Name = string.Empty;
            Schema = string.Empty;
            PrimaryKey = new Dictionary<string, Field>();
            Fields = new Dictionary<string, Field>();
            All = new Dictionary<string, Field>();
            Joins = new Dictionary<string, Relationship>();
            EntitySqlWriter = new EntitySqlWriter(this);
            Processed = false;
        }

        public string FirstKey() {
            return PrimaryKey.First().Key;
        }

        public bool IsMaster() {
            return PrimaryKey.Any(kv => kv.Value.FieldType == FieldType.MasterKey);
        }

        public void Dispose() {
            foreach (var key in All.Keys) {
                var field = All[key];
                if (field.Transforms == null) continue;
                foreach (var t in field.Transforms) {
                    t.Dispose();
                }
            }
        }
    }
}