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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main
{
    public class Field
    {
        public object Default;
        public StringBuilder StringBuilder;
        private FieldType _fieldType;
        private string _name;
        private string _sqlDataType;
        private FieldSqlWriter _sqlWriter;

        public Field(FieldType fieldType) : this("System.String", "64", fieldType, true, null)
        {
        }

        public Field(string typeName, string length, FieldType fieldType, bool output, string @default)
        {
            Initialize(typeName, length, fieldType, output, @default);
        }

        public string Type { get; private set; }
        public string SimpleType { get; set; }
        public string Quote { get; private set; }
        public string Alias { get; set; }
        public string Schema { get; set; }
        public string Entity { get; set; }
        public string Process { get; set; }
        public string Parent { get; set; }
        public string Length { get; private set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public bool NotNull { get; set; }
        public bool Identity { get; set; }
        public KeyValuePair<string, string> References { get; set; }
        public Transforms Transforms { get; set; }
        public bool Input { get; set; }
        public bool Output { get; set; }
        public bool UseStringBuilder { get; private set; }
        public Type SystemType { get; set; }
        public int Index { get; set; }
        public bool Unicode { get; set; }
        public bool VariableLength { get; set; }
        public bool Auto { get; set; }
        public string Aggregate { get; set; }

        public bool HasTransforms
        {
            get { return Transforms != null && Transforms.Count > 0; }
        }

        public IParameters Parameters { get; set; }
        public bool HasParameters { get; set; }
        public List<SearchType> SearchTypes { get; set; }

        public string SqlDataType
        {
            get { return _sqlDataType ?? (_sqlDataType = new SqlServerDataTypeService().GetDataType(this)); }
        }

        /// <summary>
        ///     FieldType can affect Output
        /// </summary>
        public FieldType FieldType
        {
            get { return _fieldType; }
            set
            {
                _fieldType = value;
                if (MustBeOutput())
                {
                    Output = true;
                }
            }
        }

        /// <summary>
        ///     Alias follows name if no alias provided
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (string.IsNullOrEmpty(Alias))
                {
                    Alias = Name;
                }
            }
        }

        public FieldSqlWriter SqlWriter
        {
            get { return _sqlWriter ?? (_sqlWriter = new FieldSqlWriter(this)); }
            set { _sqlWriter = value; }
        }

        public bool MustBeOutput()
        {
            return FieldType.HasFlag(FieldType.MasterKey) || FieldType.HasFlag(FieldType.ForeignKey) ||
                   FieldType.HasFlag(FieldType.PrimaryKey);
        }

        private void Initialize(string typeName, string length, FieldType fieldType, bool output, string @default)
        {
            Name = "field";
            Input = true;
            Unicode = true;
            VariableLength = true;
            Type = typeName;
            Length = length;
            SimpleType = Common.ToSimpleType(typeName);
            Quote = (new[] {"string", "char", "datetime", "guid", "xml"}).Any(t => t.Equals(SimpleType)) ? "'" : string.Empty;
            UseStringBuilder = (new[] {"string", "char"}).Any(t => t.Equals(SimpleType));
            FieldType = fieldType;
            Output = output || MustBeOutput();
            SystemType = Common.ToSystemType(SimpleType);
            StringBuilder = UseStringBuilder ? new StringBuilder() : null;
            Transforms = new Transforms();
            Default = new ConversionFactory().Convert(@default, SimpleType);
            SearchTypes = new List<SearchType>();
        }

        public string AsJoin(string left, string right)
        {
            return string.Format("{0}.[{1}] = {2}.[{1}]", left, Name, right);
        }

        public void Transform(Row row)
        {
            foreach (AbstractTransform t in Transforms)
            {
                if (t.RequiresRow || t.HasParameters)
                {
                    t.Transform(ref row, Alias);
                }
                else
                {
                    if (UseStringBuilder)
                    {
                        StringBuilder.Clear();
                        StringBuilder.Append(row[Alias]);
                        t.Transform(ref StringBuilder);
                        row[Alias] = StringBuilder.ToString();
                    }
                    else
                    {
                        row[Alias] = t.Transform(row[Alias], SimpleType);
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("({0}) {1}", Type, Alias);
        }
    }
}