using System.Linq;
using numl.Supervised;
using numl.Supervised.DecisionTree;
using numl.Supervised.KNN;
using numl.Supervised.Perceptron;
using numl.Supervised.Regression;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Provider.Numl.Transforms {

    public class NumlTransform : BaseTransform {

        private readonly IModel _model;
        private readonly Field[] _fields;
        private readonly Field[] _output;

        public NumlTransform(IContext context) : base(context, "object") {
            _output = MultipleOutput();

            switch (context.Transform.ModelType) {
                case "balls":
                    break;
                case "knn":
                    _model = new KNNModel().Load(context.Transform.Model);
                    break;
                case "polykernelperceptron":
                case "rbfkernelperceptron":
                    _model = new KernelPerceptronModel().Load(context.Transform.Model);
                    break;
                case "linearregression":
                    _model = new LinearRegressionModel().Load(context.Transform.Model);
                    break;
                default:
                    _model = new DecisionTreeModel().Load(context.Transform.Model);
                    break;
            }

            _fields = context.GetAllEntityFields().Where(f => !f.System).ToArray();
        }

        public override IRow Transform(IRow row) {
            var result = _model.Predict(row.ToFriendlyDictionary(_fields));

            foreach (var field in _output) {
                row[field] = result[field.Alias];
            }
            Increment();
            return row;
        }

    }
}
