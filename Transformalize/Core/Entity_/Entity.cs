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

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Core.Transform_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Providers;
using Transformalize.Providers.SqlServer;

namespace Transformalize.Core.Entity_
{

    public class Entity
    {

        public string Schema { get; set; }
        public string ProcessName { get; set; }
        public string Alias { get; set; }
        public IConnection InputConnection { get; set; }
        public IConnection OutputConnection { get; set; }
        public Field Version { get; set; }
        public IFields PrimaryKey { get; set; }
        public IFields Fields { get; set; }
        public IFields Xml { get; set; }
        public IFields All { get; set; }
        public Dictionary<string, Relationship> Joins { get; set; }
        public long RecordsAffected { get; set; }
        public object Begin { get; set; }
        public object End { get; set; }
        public int TflBatchId { get; set; }
        public int InputCount { get; set; }
        public int OutputCount { get; set; }
        public IEnumerable<Relationship> RelationshipToMaster { get; set; }
        public List<Row> InputKeys { get; set; }
        public IDbCommand InputKeysCommand { get; set; }
        public string Prefix { get; set; }
        public bool Group { get; set; }
        public bool Auto { get; set; }
        public string Name { get; set; }
        public IFields CalculatedFields { get; set; }
        public bool HasRows { get; set; }
        public bool HasRange { get; set; }

        public Entity()
        {
            Name = string.Empty;
            Alias = string.Empty;
            Schema = string.Empty;
            PrimaryKey = new Fields();
            Fields = new Fields();
            All = new Fields();
            Joins = new Dictionary<string, Relationship>();
            InputKeys = new List<Row>();
            Prefix = string.Empty;
            CalculatedFields = new Fields();
        }

        public string FirstKey()
        {
            return PrimaryKey.First().Key;
        }

        public bool IsMaster()
        {
            return PrimaryKey.Any(kv => kv.Value.FieldType.HasFlag(FieldType.MasterKey));
        }

        public string OutputName()
        {
            return string.Concat(ProcessName, Alias).Replace(" ", string.Empty);
        }

        public bool HasForeignKeys()
        {
            return Fields.Any(f => f.Value.FieldType.HasFlag(FieldType.ForeignKey));
        }

        public bool NeedsUpdate()
        {
            if (!HasRows)
                return false;

            return (!HasRange || !BeginAndEndAreEqual());
        }

        public List<string> SelectKeys()
        {
            var selectKeys = new List<string>();
            foreach (var field in PrimaryKey.ToEnumerable())
            {
                selectKeys.Add(field.Alias.Equals(field.Name) ? string.Concat("[", field.Name, "]") : string.Format("{0} = [{1}]", field.Alias, field.Name));
            }
            return selectKeys;
        }

        public List<string> OrderByKeys()
        {
            var orderByKeys = new List<string>();
            foreach (var field in PrimaryKey.ToEnumerable())
            {
                orderByKeys.Add(string.Concat("[", field.Name, "]"));
            }
            return orderByKeys;
        }

        public IFields InputFields()
        {
            return new FieldSqlWriter(All, CalculatedFields).ExpandXml().Input().Context();
        }

        public override string ToString()
        {
            return Alias;
        }

        public bool BeginAndEndAreEqual()
        {
            if (HasRange)
            {
                var bytes = new[] { "byte[]", "rowversion" };
                if (bytes.Any(t => t == Version.SimpleType))
                {
                    var beginBytes = Common.ObjectToByteArray(Begin);
                    var endBytes = Common.ObjectToByteArray(End);
                    return beginBytes.SequenceEqual(endBytes);
                }
                return Begin.Equals(End);
            }
            return false;
        }

        public void CheckDelta()
        {
            OutputConnection.LoadBeginVersion(this);
            InputConnection.LoadEndVersion(this);
        }
    }
}