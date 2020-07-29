#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using Transformalize.Contracts;
using Transformalize.Nulls;

namespace Transformalize.Impl {

   public class ParameterRowReader : IRead {

      private readonly IContext _context;
      private readonly IRead _parentReader;
      private readonly IDictionary<string, Parameter> _parameters = new Dictionary<string, Parameter>(StringComparer.OrdinalIgnoreCase);
      private readonly IRead _defaultRowReader;

      public ParameterRowReader(IContext context, IRead parentReader, IRowFactory rowFactory = null) {
         _defaultRowReader = rowFactory == null ? (IRead)new NullReader(context) : new DefaultRowReader(context, rowFactory);
         _context = context;
         _parentReader = parentReader;

         foreach (var p in context.Process.Parameters) {
            _parameters[p.Name] = p;
         }

         // attempt to disable validation if parameter can't be converted to field's type
         foreach (var field in _context.Entity.GetAllFields()) {
            Parameter p = null;
            if (_parameters.ContainsKey(field.Alias)) {
               p = _parameters[field.Alias];
            } else if (_parameters.ContainsKey(field.Name)) {
               p = _parameters[field.Name];
            }

            if (p != null && p.Value != null) {
               if (!Constants.CanConvert()[field.Type](p.Value)) {
                  field.Validators.Clear();
               }
            }

         }

      }

      public IEnumerable<IRow> Read() {

         IRow row;
         var rows = _parentReader.Read().ToArray();  // 1 or 0 records

         if (rows.Length == 0) {
            rows = _defaultRowReader.Read().ToArray();
            if (rows.Length == 0) {
               yield break;
            }
         }

         row = rows[0];

         foreach (var field in _context.Entity.GetAllFields()) {

            Parameter p = null;
            if (_parameters.ContainsKey(field.Alias)) {
               p = _parameters[field.Alias];
            } else if (_parameters.ContainsKey(field.Name)) {
               p = _parameters[field.Name];
            }

            if (p != null && p.Value != null) {

               if (Constants.CanConvert()[field.Type](p.Value)) {

                  row[field] = field.InputType == "file" && p.Value == string.Empty ? row[field] : field.Convert(p.Value);
                  var len = field.Length.Equals("max", StringComparison.OrdinalIgnoreCase) ? int.MaxValue : Convert.ToInt32(field.Length);
                  if (p.Value != null && p.Value.Length > len) {
                     if (field.ValidField != string.Empty) {
                        var validField = _context.Entity.CalculatedFields.First(f => f.Alias == field.ValidField);
                        row[validField] = false;
                     }
                     if (field.MessageField != string.Empty) {
                        var messageField = _context.Entity.CalculatedFields.First(f => f.Alias == field.MessageField);
                        row[messageField] = $"This field is limited to {len} characters.  Anything more than that is truncated.|";
                     }
                  }
               } else {
                  if (field.ValidField != string.Empty) {
                     var validField = _context.Entity.CalculatedFields.First(f => f.Alias == field.ValidField);
                     row[validField] = false;
                  }
                  if (field.MessageField != string.Empty) {
                     var messageField = _context.Entity.CalculatedFields.First(f => f.Alias == field.MessageField);
                     switch (field.Type) {
                        case "char":
                           row[messageField] = "Must be a single chracter.|";
                           break;
                        case "byte":
                           row[messageField] = "Must be a whole number (not a fraction) between 0 and 255.|";
                           break;
                        case "bool":
                        case "boolean":
                           row[messageField] = "Must be true of false.|";
                           break;
                        case "int":
                        case "int32":
                           row[messageField] = "Must be a whole number (not a fraction) between −2,147,483,648 and 2,147,483,647.|";
                           break;
                        case "short":
                        case "int16":
                           row[messageField] = "Must be a whole number (not a fraction) between -32768 and 32767.|";
                           break;
                        case "long":
                        case "int64":
                           row[messageField] = "Must be a whole number (not a fraction) between -9,223,372,036,854,775,808 and 9,223,372,036,854,775,807.|";
                           break;
                        case "double":
                           row[messageField] = "Must be a number no more than 15 digits and between between 3.4E-38 and 3.4E+38.|";
                           break;
                        case "float":
                        case "single":
                           row[messageField] = "Must be a number no more than 7 digits and between between 1.7E-308 and 1.7E+308.|";
                           break;
                        case "decimal":
                           row[messageField] = $"Must be a number no more than {field.Precision} total digits, with {field.Scale} digits to the right of the decimal point.|";
                           break;
                        default:
                           row[messageField] = $"Can not convert {p.Value} to a {field.Type}.|";
                           break;
                     }

                  }
               }

            }

         }
         yield return row;

      }
   }
}