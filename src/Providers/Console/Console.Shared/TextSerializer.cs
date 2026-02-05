#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
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
using System.Linq;
using System.Text;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Console {
   public class TextSerializer : ISerialize {

      private readonly int _length;
      private readonly OutputContext _context;
      private readonly Field[] _fields;

      public TextSerializer(OutputContext context) {
         _fields = context.OutputFields.Where(f => !f.System).ToArray();
         _length = _fields.Length;
         _context = context;
      }
      public string Serialize(IRow row) {
         var builder = new StringBuilder();
         for (var index = 0; index < _length; index++) {
            var field = _fields[index];
            var value = row[field];
            switch (field.Type) {
               case "byte[]":
                  builder.Append(value == null ? string.Empty : value is byte[]? Convert.ToBase64String((byte[])value) : value);
                  break;
               case "date":
               case "datetime":
                  if (field.Format != string.Empty) {
                     var date = (DateTime)value;
                     var formatted = date.ToString(field.Format);
                     builder.Append(formatted);
                  } else {
                     builder.Append(value.ToString());
                  }
                  break;
               default:
                  builder.Append(value.ToString());
                  break;
            }
            if (_context.Connection.Delimiter != string.Empty && index < _length - 1) {
               builder.Append(_context.Connection.Delimiter);
            }
         }
         return builder.ToString();
      }

      string ISerialize.Header {
         get {
            return _context.Connection.Header == Constants.DefaultSetting ? string.Empty : _context.Connection.Header;
         }
      }

      string ISerialize.Footer {
         get {
            return _context.Connection.Footer;
         }
      }
      public string RowSuffix { get; } = string.Empty;
      public string RowPrefix { get; } = string.Empty;

   }
}