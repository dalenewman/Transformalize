using RethinkDb;
using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;
using System;

namespace Transformalize.Provider.RethinkDB {
    public class RethinkDbOutputProvider : IOutputProvider {

        readonly InputContext _input;
        readonly OutputContext _output;
        readonly IConnectionFactory _factory;

        public RethinkDbOutputProvider(InputContext input, OutputContext output, IConnectionFactory connectionFactory) {
            _input = input;
            _output = output;
            _factory = connectionFactory;
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
            var conn = _factory.Get();

            var t = Query.Db(database).Table<Dictionary<string, object>>(table);

            var result = _output.Entity.Delete ?
                conn.Run(t.Filter(x=>!(bool)x[deletedName]).Max(x => x[versionName])) :
                conn.Run(t.Max(x => x[versionName]));

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

        public int GetMaxTflBatchId() {
            throw new NotImplementedException();
        }

        public int GetMaxTflKey() {
            throw new NotImplementedException();
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
    }
}
