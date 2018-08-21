using System;
using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class LogTransform : BaseTransform {

        private readonly Action<object> _logger;

        public LogTransform(IContext context = null) : base(context, null) {
            if (IsMissingContext()) {
                return;
            }

            switch (Context.Operation.Level) {
                case "debug":
                    _logger = o => Context.Debug(() => $"{LastMethod()} => {o}");
                    break;
                case "error":
                    _logger = o => Context.Error($"{LastMethod()} => {o}");
                    break;
                case "warn":
                    _logger = o => Context.Warn($"{LastMethod()} => {o}");
                    break;
                default:
                    _logger = o => Context.Info($"{LastMethod()} => {o}");
                    break;
            }

        }

        public override IRow Operate(IRow row) {
            _logger(row[Context.Field]);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("log") { Parameters = new List<OperationParameter>(1) { new OperationParameter("level", "info") } };
        }
    }
}