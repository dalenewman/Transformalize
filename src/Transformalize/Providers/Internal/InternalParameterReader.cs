#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Providers.Internal {

   public class InternalParameterReader : IRead {

      private readonly InputContext _input;
      private readonly IRowFactory _rowFactory;
      private readonly Field[] _fields;
      private readonly IDictionary<string, string> _parameters;

      public InternalParameterReader(InputContext input, IRowFactory rowFactory, IDictionary<string, string> parameters) : this(input, input.Entity.Fields, rowFactory, parameters) { }

      public InternalParameterReader(InputContext input, IEnumerable<Field> fields, IRowFactory rowFactory, IDictionary<string, string> parameters) {
         _input = input;
         _rowFactory = rowFactory;
         _fields = fields.Where(f => f.Input).ToArray();
         _parameters = parameters;
      }

      public IEnumerable<IRow> Read() {

         var row = _rowFactory.Create();
         var lookup = new Dictionary<string, Parameter>();
         foreach (var parameter in _input.Process.Parameters) {
            lookup[parameter.Name] = parameter;
         }
         foreach (var field in _fields) {
            var fallBack = field.DefaultValue();
            if (lookup.ContainsKey(field.Name)) {
               var p = lookup[field.Name];
               if (_parameters.ContainsKey(field.Name)) {
                  if(_parameters[field.Name] == null) {
                     row[field] = fallBack;
                  } else {
                     row[field] = Convert(p, _parameters[field.Name]);
                  }
                  _parameters.Remove(field.Name); // since parameter may be transformed and placed in value, we discard the original
               } else {
                  if(p.Value == null) {
                     row[field] = fallBack;
                  } else {
                     row[field] = Convert(p, p.Value);
                  }
               }
            } else {
               row[field] = fallBack;
            }

         }

         _input.Entity.Hits = 1;

         yield return row;
         yield break;
      }

      public object GetVersion() {
         return null;
      }

      private object Convert(Parameter p, object value) {
         if (p.Type != null && p.Type != "string" && value != null) {
            try {
               if (value is string str) {
                  return Constants.ConversionMap[p.Type](str);
               } else {
                  return Constants.ObjectConversionMap[p.Type](value);
               }
            } catch (FormatException ex) {
               _input.Error($"Could not convert '{value}' to {p.Type}. {ex.Message}");
               return Constants.TypeDefaults()[p.Type];
            }
         }
         return value;
      }
   }
}