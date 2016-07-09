#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using Pipeline.Contracts;

namespace Pipeline.DotNetFiddle.Impl {
    public class ConsoleWriter : IWrite {
        private readonly ISerialize _serializer;

        public ConsoleWriter(ISerialize serializer) {
            _serializer = serializer;
        }

        public void Write(IEnumerable<IRow> rows) {
            if (!string.IsNullOrEmpty(_serializer.Header)) {
                Console.WriteLine(_serializer.Header);
            }

            using (var enumerator = rows.GetEnumerator()) {
                var last = !enumerator.MoveNext();

                while (!last) {
                    var current = enumerator.Current;
                    last = !enumerator.MoveNext();
                    Console.Write(_serializer.RowPrefix);
                    Console.Write(_serializer.Serialize(current));
                    Console.WriteLine(last ? string.Empty : _serializer.RowSuffix);
                }
            }

            if (!string.IsNullOrEmpty(_serializer.Footer)) {
                Console.WriteLine(_serializer.Footer);
            }
        }
    }
}