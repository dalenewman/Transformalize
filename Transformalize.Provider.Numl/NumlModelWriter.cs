using System;
using System.Collections.Generic;
using System.Linq;
using numl.Math.Kernels;
using numl.Model;
using numl.Supervised;
using numl.Supervised.DecisionTree;
using numl.Supervised.KNN;
using numl.Supervised.Perceptron;
using numl.Supervised.Regression;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Provider.Numl {

    public class NumlModelWriter : IWrite {

        private readonly IConnectionContext _context;
        private readonly Descriptor _descriptor;
        private readonly IGenerator _generator;
        private readonly Field[] _fields;
        private static readonly Dictionary<string, Type> Types = Constants.TypeSystem();

        public NumlModelWriter(IConnectionContext context) {

            _context = context;
            _descriptor = Descriptor.New();

            switch (context.Connection.ModelType) {
                case "knn":
                    _generator = new KNNGenerator();
                    break;
                case "rbfkernelperceptron":
                    _generator = new KernelPerceptronGenerator(new RBFKernel(6));
                    break;
                case "polykernelperceptron":
                    _generator = new KernelPerceptronGenerator(new PolyKernel(6));
                    break;
                case "linearregression":
                    _generator = new LinearRegressionGenerator();
                    break;
                default:
                    _generator = new DecisionTreeGenerator();  // placeholder
                    break;
            }

            _fields = context.GetAllEntityOutputFields().Where(f => !f.System).ToArray();

            foreach (var field in _fields.Where(f => !f.Learn)) {
                _descriptor.With(field.Alias).As(Types[field.Type]);
            }

            foreach (var field in _fields.Where(f => f.Learn)) {
                _descriptor.Learn(field.Alias).As(Types[field.Type]);
            }

        }

        public void Write(IEnumerable<IRow> rows) {
            var model = _generator.Generate(_descriptor, rows.Select(r => r.ToFriendlyDictionary(_fields)));
            _context.Info(model.ToString());
            model.Save(_context.Connection.File);
        }
    }
}
