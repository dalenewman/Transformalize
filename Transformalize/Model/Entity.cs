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
        public int RecordsAffected { get; set; }
        public object Begin { get; set; }
        public object End { get; set; }
        public int TflBatchId { get; set; }
        public int InputCount { get; set; }
        public int OutputCount { get; set; }

        public IEnumerable<Relationship> RelationshipToMaster { get; set; }

        public Entity() {
            Name = string.Empty;
            Schema = string.Empty;
            PrimaryKey = new Dictionary<string, Field>();
            Fields = new Dictionary<string, Field>();
            All = new Dictionary<string, Field>();
            Joins = new Dictionary<string, Relationship>();
            EntitySqlWriter = new EntitySqlWriter(this);
        }

        public string FirstKey() {
            return PrimaryKey.First().Key;
        }

        public bool IsMaster() {
            return PrimaryKey.Any(kv => kv.Value.FieldType == FieldType.MasterKey);
        }

        public void Dispose() {
            foreach (var pair in All) {
                if (pair.Value.Transforms == null) continue;
                foreach (var t in pair.Value.Transforms) {
                    t.Dispose();
                }
            }
        }

        public string OutputName() {
            return string.Concat(ProcessName, Name);
        }
        
        public bool HasForeignKeys() {
            return Fields.Any(f => f.Value.FieldType.Equals(FieldType.ForeignKey));
        }
    }
}