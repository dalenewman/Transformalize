using System.Collections.Generic;
using Transformalize.Main;
using Transformalize.Main.Parameters;

namespace Transformalize.Test.Builders
{
    public class ParametersBuilder {
        private readonly IList<KeyValuePair<string, IParameter>> _params = new List<KeyValuePair<string, IParameter>>();
        private int _index;

        public ParameterBuilder Parameter(string inKey, object value) {
            var parameter = new Parameter() { Index = _index, Name = inKey, Value = value, SimpleType = "string" };
            return Consume(inKey, parameter);
        }

        public ParameterBuilder Parameter(string inKey) {
            var parameter = new Parameter() { Index = _index, Name = inKey, SimpleType = "string" };
            return Consume(inKey, parameter);
        }

        private ParameterBuilder Consume(string inKey, Parameter parameter) {
            _index++;
            _params.Add(new KeyValuePair<string, IParameter>(inKey, parameter));
            var parametersBuilder = this;
            return new ParameterBuilder(ref parametersBuilder, ref parameter);
        }

        public Parameters ToParameters() {
            var parameters = new Parameters();
            foreach (var p in _params) {
                parameters.Add(p.Key, p.Value);
            }
            return parameters;
        }

        public ParameterBuilder Parameters(params string[] inKeys) {
            ParameterBuilder builder = null;
            foreach (var inKey in inKeys) {
                builder = Parameter(inKey);
            }
            return builder;
        }
    }
}