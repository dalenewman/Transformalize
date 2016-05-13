#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using Pipeline.Contracts;

namespace Pipeline.Nulls {

    public class NullWriter : IWrite {
        readonly IContext _context;
        readonly bool _log;

        public NullWriter() {
        }

        public NullWriter(IContext context, bool log = true) {
            _context = context;
            _log = log;
        }

        public void Write(IEnumerable<IRow> rows) {
            if (_log)
                _context.Info("Null writing...");
        }

    }
}