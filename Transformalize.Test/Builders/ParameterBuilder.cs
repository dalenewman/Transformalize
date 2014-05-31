using Transformalize.Main;
using Transformalize.Main.Parameters;

namespace Transformalize.Test.Builders
{
    public class ParameterBuilder {
        private readonly ParametersBuilder _parametersBuilder;
        private readonly Parameter _parameter;

        public ParameterBuilder(ref ParametersBuilder parametersBuilder, ref Parameter parameter) {
            _parametersBuilder = parametersBuilder;
            _parameter = parameter;
        }

        public ParameterBuilder Value(object value) {
            _parameter.Value = value;
            return this;
        }

        public ParameterBuilder Type(string type) {
            _parameter.SimpleType = type;
            return this;
        }

        public ParameterBuilder Name(string name) {
            _parameter.Name = name;
            return this;
        }

        public ParameterBuilder Parameter(string inKey) {
            return _parametersBuilder.Parameter(inKey);
        }

        public ParameterBuilder Parameter(string inKey, object value) {
            return _parametersBuilder.Parameter(inKey, value);
        }

        public Parameters ToParameters() {
            return _parametersBuilder.ToParameters();
        }
    }
}