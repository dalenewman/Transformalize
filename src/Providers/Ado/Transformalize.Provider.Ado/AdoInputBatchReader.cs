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

using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.Ado {

    /// <summary>
    /// Writes one query to read all the keys, 
    /// then writes many queries to read the data based on the keys.
    /// Use this if reading the data with SqlInputReader causes blocking 
    /// for other applications.  It is slower, and requires the creation of
    /// temporary tables, but it reads according to the batch-size set on the 
    /// connection and is less likely to block (the smaller the batch size).  Of course, 
    /// you can also set no-lock on the entity to reduce blocking, but at the risk of 
    /// reading un-commited data.
    /// </summary>
    public class AdoInputBatchReader : IRead {
        private readonly InputContext _input;
        private readonly IRead _reader;
        private readonly IConnectionFactory _cf;
        readonly AdoEntityMatchingFieldsReader _fieldsReader;
        int _rowCount;

        public AdoInputBatchReader(InputContext input, IRead reader, IConnectionFactory cf, IRowFactory rowFactory) {
            _input = input;
            _reader = reader;
            _cf = cf;
            _fieldsReader = new AdoEntityMatchingFieldsReader(input, cf, rowFactory);
        }

        public IEnumerable<IRow> Read() {
            using (var cn = _cf.GetConnection()) {
                cn.Open();
                foreach (var batch in _reader.Read().Partition(_input.Entity.ReadSize)) {
                    foreach(var row in _fieldsReader.Read(batch)) {
                        _rowCount++;
                        yield return row;
                    }
                }
            }
            _input.Info("{0} from {1}", _rowCount, _input.Connection.Name);
        }

    }
}