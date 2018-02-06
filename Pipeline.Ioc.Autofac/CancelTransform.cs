using System;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Ioc.Autofac {
    public class CancelTransform : BaseTransform {
        private bool _quit;
        public CancelTransform(IContext context) : base(context, "null") {
            Console.CancelKeyPress += OnConsoleOnCancelKeyPress;
        }

        private void OnConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs eArgs) {
            _quit = true;
            eArgs.Cancel = true;
        }

        public override IRow Operate(IRow row) {
            if (!_quit) return row;
            Context.Warn("Cancelled with CTRL-C or CTRL-BREAK");
            Environment.Exit(0);
            return row;
        }

        public override void Dispose() {
            Console.CancelKeyPress -= OnConsoleOnCancelKeyPress;
            base.Dispose();
        }
    }
}