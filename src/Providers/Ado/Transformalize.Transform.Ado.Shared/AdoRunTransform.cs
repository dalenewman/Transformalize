using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Providers.Ado;

namespace Transformalize.Transforms.Ado {
    public class AdoRunTransform : BaseTransform {

        private readonly Field _input;
        private readonly IDictionary<string, object> _editor;
        private readonly IDbConnection _cn;
        private readonly ExpandoObject _parameters;

        public AdoRunTransform(IContext context = null, IConnectionFactory factory = null) : base(context, null) {

            ProducesFields = true;

            if (IsMissingContext()) {
                return;
            }

            if (factory == null) {
                Context.Error("The AdoRunTransform transform did not receive a connection factory.");
                Run = false;
                return;
            }

            _parameters = new ExpandoObject();
            _editor = (IDictionary<string, object>)_parameters;
            _input = SingleInput();

            _cn = factory.GetConnection();
        }

        public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {

            using (_cn) {
                _cn.Open();
                foreach (var row in rows) {
                    var cmd = Context.Operation.Command == string.Empty ? row[_input].ToString() : Context.Operation.Command;

                    // map parameters to ado parameters and set values
                    if (cmd.Contains("@")) {
                        var active = Context.Process.Parameters;
                        foreach (var parameter in active) {
                            if (parameter.Name.Contains(".")) {
                                parameter.Name = parameter.Name.Replace(".", "_");
                            }
                        }
                        foreach (var name in new AdoParameterFinder().Find(cmd).Distinct().ToList()) {
                            var match = active.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                            if (match != null) {
                                Context.Debug(() => $"Set @{match.Name} to {match.Value}");
                                _editor[match.Name] = match.Convert(match.Value);
                            }
                        }
                    }

                    // map fields to ado parameters and set values
                    var fields = Context.Entity.GetFieldMatches(cmd).ToArray();
                    foreach (var name in new AdoParameterFinder().Find(cmd).Distinct().ToList()) {
                        var match = fields.FirstOrDefault(f => f.Alias.Equals(name, StringComparison.OrdinalIgnoreCase));
                        if (match != null) {
                            var value = match.Convert(row[match]);
                            Context.Debug(() => $"Set @{match.Alias} to {value}");
                            _editor[match.Alias] = value;
                        }
                    }

                    try {
                        var rowCount = _cn.Execute(cmd, _parameters);
                        Context.Debug(() => $"{cmd} affected {rowCount} row(s).");
                    } catch (Exception e) {
                        Context.Error(e, e.Message);
                        Run = false;
                        yield break;
                    }

                    yield return row;
                }
            }
        }

        public override IRow Operate(IRow row) {
            throw new NotImplementedException("This should never be called! See Operate on rows.");
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("run") { Parameters = new List<OperationParameter>(1) { new OperationParameter("command") } };
        }
    }
}
