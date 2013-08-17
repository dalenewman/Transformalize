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
using System.Linq;
using System.Text;
using Transformalize.Core.Parameter_;
using Transformalize.Core.Transform_;
using Transformalize.Providers.SqlServer;

namespace Transformalize.Core.Field_
{

    public class Field
    {

        private FieldType _fieldType;
        private string _name;
        private string _sqlDataType;
        private FieldSqlWriter _sqlWriter;
        private bool _input;

        public StringBuilder StringBuilder;

        public string Type { get; private set; }
        public string SimpleType { get; private set; }
        public string Quote { get; private set; }
        public string Alias { get; set; }
        public string Schema { get; set; }
        public string Entity { get; set; }
        public string Process { get; set; }
        public string Parent { get; set; }
        public string Length { get; private set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public bool Clustered { get; set; }
        public bool NotNull { get; set; }
        public bool Identity { get; set; }
        public KeyValuePair<string, string> References { get; set; }
        public Transforms Transforms { get; set; }
        public bool Output { get; set; }
        public bool UseStringBuilder { get; private set; }
        public Type SystemType { get; private set; }
        public Dictionary<string, Field> InnerXml { get; set; }
        public string XPath { get; set; }
        public int Index { get; set; }
        public bool Unicode { get; set; }
        public bool VariableLength { get; set; }
        public object Default;
        public bool Auto { get; set; }
        public string Aggregate { get; set; }
        public Parameter AsParameter { get; set; }
        public bool HasTransforms { get; set; }

        public string SqlDataType
        {
            get { return _sqlDataType ?? (_sqlDataType = new SqlServerDataTypeService().GetDataType(this)); }
        }

        public bool Input
        {
            get { return _input; }
            set
            {
                _input = value;
                if (!_input && Output)
                {
                    Output = false;
                }
            }
        }

        /// <summary>
        /// FieldType can affect Output
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
        /// Alias follows name if no alias provided
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

        public bool MustBeOutput()
        {
            return FieldType.HasFlag(FieldType.MasterKey) || FieldType.HasFlag(FieldType.ForeignKey) || FieldType.HasFlag(FieldType.PrimaryKey);
        }

        public Field(FieldType fieldType) : this("System.String", "64", fieldType, true, null) { }

        public Field(string typeName, string length, FieldType fieldType, bool output, object @default)
        {
            Initialize(typeName, length, fieldType, output, @default);
        }

        private void Initialize(string typeName, string length, FieldType fieldType, bool output, object @default)
        {
            Input = true;
            Unicode = true;
            VariableLength = true;
            Type = typeName;
            Length = length;
            SimpleType = Type.ToLower().Replace("system.", string.Empty);
            Quote = (new[] { "string", "char", "datetime", "guid" }).Any(t => t.Equals(SimpleType)) ? "'" : string.Empty;
            UseStringBuilder = (new[] { "string", "char" }).Any(t => t.Equals(SimpleType));
            FieldType = fieldType;
            Output = output || MustBeOutput();
            SystemType = System.Type.GetType(typeName);
            StringBuilder = UseStringBuilder ? new StringBuilder() : null;
            InnerXml = new Dictionary<string, Field>();
            Default = new ConversionFactory().Convert(@default ?? string.Empty, SimpleType);

            if (SimpleType.Equals("rowversion"))
            {
                Output = false;
            }

        }

        public FieldSqlWriter SqlWriter
        {
            get { return _sqlWriter ?? (_sqlWriter = new FieldSqlWriter(this)); }
            set { _sqlWriter = value; }
        }

        public string AsJoin(string left, string right)
        {
            return string.Format("{0}.[{1}] = {2}.[{1}]", left, Name, right);
        }

        public void Transform()
        {
            foreach (AbstractTransform t in Transforms)
            {
                t.Transform(ref StringBuilder);
            }
        }

        public void Transform(ref object value)
        {
            foreach (AbstractTransform t in Transforms)
            {
                t.Transform(ref value);
            }
        }

        public override string ToString()
        {
            return string.Format("({0}} {1}.{2}", Type, Entity, Alias);
        }

    }
}