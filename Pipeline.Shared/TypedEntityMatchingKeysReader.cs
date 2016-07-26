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
using System.Collections.Generic;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms.System;

namespace Pipeline {
    public class TypedEntityMatchingKeysReader : ITakeAndReturnRows {
        private readonly ITakeAndReturnRows _reader;
        private readonly TypeTransform _typeTransform;

        public TypedEntityMatchingKeysReader(ITakeAndReturnRows reader, IContext context) {
            _reader = reader;
            _typeTransform = new TypeTransform(context, CombineFields(context.Entity.GetPrimaryKey(), context.Entity.TflHashCode()));
        }

        public IEnumerable<IRow> Read(IEnumerable<IRow> input) {
            return _reader.Read(input).Select(r => _typeTransform.Transform(r));
        }

        static IEnumerable<Field> CombineFields(IEnumerable<Field> keys, Field hashCode) {
            var fields = new List<Field>(keys) { hashCode };
            return fields.ToArray();
        }
    }
}