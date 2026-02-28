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
using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Console {

   public class ConsoleWriter : IWrite {
      private readonly ISerialize _serializer;
      private readonly OutputContext _context;

      public ConsoleWriter(OutputContext context, ISerialize serializer) {
         _serializer = serializer;
         _context = context;
      }

      public void Write(IEnumerable<IRow> rows) {
         if (!string.IsNullOrEmpty(_serializer.Header)) {
            System.Console.Out.WriteLine(_serializer.Header);
         }

         using (var enumerator = rows.GetEnumerator()) {
            var last = !enumerator.MoveNext();

            while (!last) {
               var current = enumerator.Current;
               last = !enumerator.MoveNext();
               System.Console.Out.Write(_serializer.RowPrefix);
               System.Console.Out.Write(_serializer.Serialize(current));
               System.Console.Out.WriteLine(last ? string.Empty : _serializer.RowSuffix);
               ++_context.Entity.Inserts;
            }
         }

         if (!string.IsNullOrEmpty(_serializer.Footer)) {
            System.Console.Out.WriteLine(_serializer.Footer);
         }
      }
   }
}