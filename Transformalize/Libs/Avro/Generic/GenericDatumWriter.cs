/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using Transformalize.Libs.Avro.IO;
using Transformalize.Libs.Avro.Schema;

namespace Transformalize.Libs.Avro.Generic
{
    /// <summary>
    /// PreresolvingDatumWriter for writing data from GenericRecords or primitive types.
    /// <see cref="PreresolvingDatumWriter{T}">For more information about performance considerations for choosing this implementation</see>
    /// </summary>
    public class GenericDatumWriter<T> : PreresolvingDatumWriter<T>
    {
        public GenericDatumWriter( Schema.Schema schema ) : base(schema, new GenericArrayAccess(), new DictionaryMapAccess())
        {
        }

        protected override void WriteRecordFields(object recordObj, RecordFieldWriter[] writers, Encoder encoder)
        {
            var record = (GenericRecord) recordObj;
            foreach (var writer in writers)
            {
                writer.WriteField(record[writer.Field.Name], encoder);
            }
        }

        protected override void EnsureRecordObject( RecordSchema recordSchema, object value )
        {
            if( value == null || !( value is GenericRecord ) || !( ( value as GenericRecord ).Schema.Equals( recordSchema ) ) )
            {
                throw TypeMismatch( value, "record", "GenericRecord" );
            }
        }

        protected override void WriteField(object record, string fieldName, int fieldPos, WriteItem writer, Encoder encoder)
        {
            writer(((GenericRecord)record)[fieldName], encoder);
        }

        protected override WriteItem ResolveEnum(EnumSchema es)
        {
            return (v,e) =>
                       {
                            if( v == null || !(v is GenericEnum) || !((v as GenericEnum).Schema.Equals(es)))
                                throw TypeMismatch(v, "enum", "GenericEnum");
                            e.WriteEnum(es.Ordinal((v as GenericEnum ).Value));
                       };
        }

        protected override void WriteFixed( FixedSchema es, object value, Encoder encoder )
        {
            if (value == null || !(value is GenericFixed) || !(value as GenericFixed).Schema.Equals(es))
            {
                throw TypeMismatch(value, "fixed", "GenericFixed");
            }
            GenericFixed ba = (GenericFixed)value;
            encoder.WriteFixed(ba.Value);
        }

        /*
         * FIXME: This method of determining the Union branch has problems. If the data is IDictionary<string, object>
         * if there are two branches one with record schema and the other with map, it choose the first one. Similarly if
         * the data is byte[] and there are fixed and bytes schemas as branches, it choose the first one that matches.
         * Also it does not recognize the arrays of primitive types.
         */
        protected override bool UnionBranchMatches(Schema.Schema sc, object obj)
        {
            if (obj == null && sc.Tag != Avro.Schema.Schema.Type.Null) return false;
            switch (sc.Tag)
            {
                case Avro.Schema.Schema.Type.Null:
                    return obj == null;
                case Avro.Schema.Schema.Type.Boolean:
                    return obj is bool;
                case Avro.Schema.Schema.Type.Int:
                    return obj is int;
                case Avro.Schema.Schema.Type.Long:
                    return obj is long;
                case Avro.Schema.Schema.Type.Float:
                    return obj is float;
                case Avro.Schema.Schema.Type.Double:
                    return obj is double;
                case Avro.Schema.Schema.Type.Bytes:
                    return obj is byte[];
                case Avro.Schema.Schema.Type.String:
                    return obj is string;
                case Avro.Schema.Schema.Type.Error:
                case Avro.Schema.Schema.Type.Record:
                    //return obj is GenericRecord && (obj as GenericRecord).Schema.Equals(s);
                    return obj is GenericRecord && (obj as GenericRecord).Schema.SchemaName.Equals((sc as RecordSchema).SchemaName);
                case Avro.Schema.Schema.Type.Enumeration:
                    //return obj is GenericEnum && (obj as GenericEnum).Schema.Equals(s);
                    return obj is GenericEnum && (obj as GenericEnum).Schema.SchemaName.Equals((sc as EnumSchema).SchemaName);
                case Avro.Schema.Schema.Type.Array:
                    return obj is Array && !(obj is byte[]);
                case Avro.Schema.Schema.Type.Map:
                    return obj is IDictionary<string, object>;
                case Avro.Schema.Schema.Type.Union:
                    return false;   // Union directly within another union not allowed!
                case Avro.Schema.Schema.Type.Fixed:
                    //return obj is GenericFixed && (obj as GenericFixed).Schema.Equals(s);
                    return obj is GenericFixed && (obj as GenericFixed).Schema.SchemaName.Equals((sc as FixedSchema).SchemaName);
                default:
                    throw new AvroException("Unknown schema type: " + sc.Tag);
            }
        }

        private class GenericArrayAccess : ArrayAccess
        {
            public void EnsureArrayObject( object value )
            {
                if( value == null || !( value is Array ) ) throw TypeMismatch( value, "array", "Array" );
            }

            public long GetArrayLength( object value )
            {
                return ( (Array) value ).Length;
            }

            public void WriteArrayValues(object array, WriteItem valueWriter, Encoder encoder)
            {
                var arrayInstance = (Array) array;
                for(int i = 0; i < arrayInstance.Length; i++)
                {
                    encoder.StartItem();
                    valueWriter(arrayInstance.GetValue(i), encoder);
                }
            }
        }
    }
}
