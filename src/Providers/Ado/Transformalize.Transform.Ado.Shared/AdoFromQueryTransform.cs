using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Providers.Ado;

namespace Transformalize.Transforms.Ado {
    public class AdoFromQueryTransform : BaseTransform {

        private readonly Field _input;
        private readonly IDictionary<string, object> _editor;
        private readonly IDbConnection _cn;
        private readonly Field[] _output;
        private readonly ExpandoObject _parameters;

        public AdoFromQueryTransform(IContext context = null, IConnectionFactory factory = null) : base(context, null) {

            ProducesFields = true;

            if (IsMissingContext()) {
                return;
            }

            if (factory == null) {
                Context.Error("The FromAdoQueryTransform transform did not receive a connection factory.");
                Run = false;
                return;
            }

            _parameters = new ExpandoObject();
            _editor = (IDictionary<string, object>)_parameters;
            _input = SingleInputForMultipleOutput();
            _output = MultipleOutput();

            _cn = factory.GetConnection();
            _cn.Open();
        }

        public override IRow Operate(IRow row) {

            var query = Context.Operation.Query == string.Empty ? row[_input].ToString() : Context.Operation.Query;

            // map parameters to ado parameters and set values
            if (query.Contains("@")) {
                var active = Context.Process.Parameters;
                foreach (var parameter in active) {
                    if (parameter.Name.Contains(".")) {
                        parameter.Name = parameter.Name.Replace(".", "_");
                    }
                }
                foreach (var name in new AdoParameterFinder().Find(query).Distinct().ToList()) {
                    var match = active.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (match != null) {
                        Context.Debug(() => $"Set @{match.Name} to {match.Value}");
                        _editor[match.Name] = match.Convert(match.Value);
                    }
                }
            }

            // map fields to ado parameters and set values
            var fields = Context.Entity.GetFieldMatches(query).ToArray();
            foreach (var name in new AdoParameterFinder().Find(query).Distinct().ToList()) {
                var match = fields.FirstOrDefault(f => f.Alias.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (match != null) {
                    var value = match.Convert(row[match]);
                    Context.Debug(() => $"Set @{match.Alias} to {value}");
                    _editor[match.Alias] = value;
                }
            }

            try {
                using (var reader = _cn.ExecuteReader(query, _parameters)) {
                    if (reader.Read()) {
                        for (var i = 0; i < reader.FieldCount; i++) {
                            var name = reader.GetName(i);
                            var field = _output.FirstOrDefault(f => f.Alias.Equals(name, StringComparison.OrdinalIgnoreCase));
                            row[field] = reader.GetValue(i);
                        }
                    }
                }
            } catch (DbException e) {
                Context.Error(e, e.Message);
                Run = false;
            }

            return row;
        }

        public override void Dispose() {
            if (_cn != null && _cn.State != ConnectionState.Closed) {
                _cn.Close();
            }
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("fromquery") { Parameters = new List<OperationParameter>(1) { new OperationParameter("query") } };
        }
    }
}
