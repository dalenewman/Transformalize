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
using RethinkDb;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.RethinkDB {
    public class RethinkDbOutputProvider : IOutputProvider {

        readonly InputContext _input;
        readonly OutputContext _output;
        readonly IConnection _cn;

        public RethinkDbOutputProvider(InputContext input, OutputContext output, IConnectionFactory connectionFactory) {
            _input = input;
            _output = output;
            _cn = connectionFactory.Get();
        }

        public void Delete() {
            throw new NotImplementedException();
        }

        public object GetMaxVersion() {

            if (string.IsNullOrEmpty(_output.Entity.Version)) {
                return null;
            }

            var version = _output.Entity.GetVersionField();
            var versionName = version.Alias;
            var deletedName = _output.Entity.TflDeleted().Alias;

            var database = _output.Connection.Database;
            var table = _output.Entity.Alias;

            var t = Query.Db(database).Table<Dictionary<string, object>>(table);

            var result = _output.Entity.Delete ?
                _cn.Run(t.Filter(x => !(bool)x[deletedName]).Max(x => x[versionName])) :
                _cn.Run(t.Max(x => x[versionName]));

            var value = result[versionName];
            if (value != null && value.GetType() != Constants.TypeSystem()[version.Type]) {
                value = version.Convert(value);
            }

            _output.Info($"Found Version {value ?? "null"}");
            return value;
        }

        public void End() {
            throw new NotImplementedException();
        }

        public int GetNextTflBatchId() {
            var database = _output.Connection.Database;
            var table = _output.Entity.Alias;
            var t = Query.Db(database).Table<Dictionary<string, object>>(table);

            if (_output.Process.Mode != "init") {
                // query and set Context.Entity.BatchId (max of TflBatchId)
                var batchName = _output.Entity.TflBatchId().Alias;
                var batchId = _cn.Run(t.Max(x => x[batchName]))[batchName];
                return batchId != null ? Convert.ToInt32(batchId) + 1 : 0;
            }
            return 0;
        }

        public int GetMaxTflKey() {
            var database = _output.Connection.Database;
            var table = _output.Entity.Alias;
            var t = Query.Db(database).Table<Dictionary<string, object>>(table);

            if (_output.Process.Mode != "init") {

                // query and set Context.Entity.Identity (max of Identity)
                var identityName = _output.Entity.TflKey().Alias;

                var identity = _cn.Run(t.Max(x => x[identityName]))[identityName];
                return identity != null ? Convert.ToInt32(identity) : 0;
            }
            return 0;
        }

        public void Initialize() {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> Match(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> ReadKeys() {
            throw new NotImplementedException();
        }

        public void Start() {
            throw new NotImplementedException();
        }

        public void Write(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            if (_cn != null) {
                _cn.Dispose();
            }
        }
    }
}
