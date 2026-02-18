#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using System.Data;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Ado {

   public class AdoRowCreator {

      private readonly IConnectionContext _context;
      private readonly IRowFactory _rowFactory;
      private bool[] _errors;
      private readonly Dictionary<string, Type> _typeMap;
      private List<Func<int,object, object>> _conversions;
      private int _fieldCount;

      public AdoRowCreator(IConnectionContext context, IRowFactory rowFactory) {
         _errors = null;
         _context = context;
         _rowFactory = rowFactory;
         _typeMap = Constants.TypeSystem();
      }

      public IRow Create(IDataReader reader, Field[] fields) {

         var row = _rowFactory.Create();

         if (_fieldCount == 0) {
            _fieldCount = Math.Min(reader.FieldCount, fields.Length);
            _conversions = new List<Func<int, object, object>>(_fieldCount);
            for (var i = 0; i < _fieldCount; i++) {
               _conversions.Add(null);
            }
            _errors = new bool[fields.Length];
            for (var i = 0; i < _fieldCount; i++) {
               var inputType = reader.GetFieldType(i);
               _errors[i] = inputType != _typeMap[fields[i].Type];

               if (_errors[i]) {
                  if (fields[i].Transforms.Any() && fields[i].Transforms.First().Method == "convert") {
                     _conversions[i] = (x,o) => o;  // the user has set a conversion
                  } else {
                     if(fields[i].Format != string.Empty && _typeMap[fields[i].Type] == typeof(string)) {
                        if(inputType == typeof(DateTime)) {
                           _conversions[i] = (x,o) => ((DateTime)o).ToString(fields[x].Format);
                        } else if (inputType == typeof(decimal)) {
                           _conversions[i] = (x,o) => ((decimal)o).ToString(fields[x].Format);
                        } else {
                           _conversions[i] = (x,o) => fields[x].Convert(o);
                           _context.Warn("Type mismatch for {0}. Expected {1}, but read {2}.  Change type or add conversion.", fields[i].Name, fields[i].Type, inputType);
                        }
                     } else {
                        _conversions[i] = (x, o) => fields[x].Convert(o);
                        _context.Warn("Type mismatch for {0}. Expected {1}, but read {2}.  Change type or add conversion.", fields[i].Name, fields[i].Type, inputType);
                     }
                  }

               } else {
                  _conversions[i] = (x,o) => o;
               }

            }

            for (var i = 0; i < _fieldCount; i++) {
               if (reader.IsDBNull(i))
                  continue;
               if (_errors[i]) {
                  var value = reader.GetValue(i);
                  try {
                     row[fields[i]] = fields[i].Type == "object" ? value : _conversions[i](i, value);
                  } catch (FormatException) {
                     _context.Error($"Could not convert value {value} in field {fields[i].Alias} to {fields[i].Type}");
                  }

               } else {
                  row[fields[i]] = reader.GetValue(i);
               }
            }
         } else {
            for (var i = 0; i < _fieldCount; i++) {
               if (reader.IsDBNull(i))
                  continue;
               if (_errors[i]) {
                  row[fields[i]] = fields[i].Type == "object" ? reader.GetValue(i) : _conversions[i](i, reader.GetValue(i));
               } else {
                  row[fields[i]] = reader.GetValue(i);
               }
            }
         }

         return row;
      }

   }
}
