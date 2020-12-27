#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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