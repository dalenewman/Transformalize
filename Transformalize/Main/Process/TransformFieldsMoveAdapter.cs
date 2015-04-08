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
using System.Linq;
using Transformalize.Configuration;

namespace Transformalize.Main {
    public class TransformFieldsMoveAdapter {

        private readonly TflProcess _process;

        public TransformFieldsMoveAdapter(TflProcess process) {
            _process = process;
        }

        public void Adapt(string transformName) {

            var fields = new Dictionary<string, Dictionary<string, List<TflField>>>();
            var calculatedFields = new Dictionary<string, Dictionary<string, List<TflField>>>();

            foreach (TflEntity entity in _process.Entities) {
                fields[entity.Alias] = GetFields(entity.Fields, transformName);
                RemoveFields(entity.Fields, transformName);

                calculatedFields[entity.Alias] = GetFields(entity.CalculatedFields, transformName);
                RemoveFields(entity.CalculatedFields, transformName);
            }

            InsertFields(fields);
            InsertFields(calculatedFields);
        }

        public Dictionary<string, List<TflField>> GetFields(List<TflField> fields, string transformName) {
            var result = new Dictionary<string, List<TflField>>();
            foreach (TflField field in fields) {
                foreach (var transform in field.Transforms) {
                    if (!transform.Method.Equals(transformName, StringComparison.OrdinalIgnoreCase))
                        continue;
                    result[field.Alias] = new List<TflField>();
                    foreach (TflField tField in transform.Fields) {
                        tField.Input = false;
                        result[field.Alias].Add(tField);
                    }
                }
            }
            return result;
        }

        public void RemoveFields(List<TflField> fields, string transformName) {
            foreach (var field in fields) {
                foreach (var transform in field.Transforms) {
                    if (!transform.Method.Equals(transformName, StringComparison.OrdinalIgnoreCase))
                        continue;
                    transform.Fields.Clear();
                }
            }
        }

        public void InsertFields(Dictionary<string, Dictionary<string, List<TflField>>> fields) {
            foreach (var entity in fields) {
                foreach (var field in entity.Value) {
                    var entityElement = _process.Entities.First(e => e.Alias == entity.Key);
                    var fieldElement = entityElement.Fields.First(f => f.Alias == field.Key);
                    var index = entityElement.Fields.IndexOf(fieldElement) + 1;
                    foreach (var element in field.Value) {
                        entityElement.Fields.Insert(index, element);
                        index++;
                    }
                }
            }
        }

    }
}