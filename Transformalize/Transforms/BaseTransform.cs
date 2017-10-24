#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public abstract class BaseTransform : ITransform {

        private const StringComparison Sc = StringComparison.OrdinalIgnoreCase;
        private Field _singleInput;
        private string _received;
        private readonly HashSet<string> _errors = new HashSet<string>();
        private readonly HashSet<string> _warnings = new HashSet<string>();

        public IContext Context { get; }
        public bool Run { get; set; } = true;

        protected BaseTransform(IContext context, string returns) {
            Context = context;
            Returns = returns;
        }

        // this **must** be implemented
        public abstract IRow Operate(IRow row);

        // this *may* be implemented
        public virtual IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
            return Run ? rows.Select(Operate) : rows;
        }

        public string Returns {
            get => Context.Operation.Returns;
            set => Context.Operation.Returns = value;
        }

        public void Error(string error) {
            _errors.Add(error);
        }

        public void Warn(string warning) {
            _warnings.Add(warning);
        }

        public IEnumerable<string> Errors() {
            return _errors;
        }

        public IEnumerable<string> Warnings() {
            return _warnings;
        }

        public uint RowCount { get; set; }

        public virtual void Increment() {
            RowCount++;
            if (RowCount % Context.Entity.LogInterval == 0) {
                Context.Info(RowCount.ToString());
            }
        }

        /// <summary>
        /// A transformer's input can be entity fields, process fields, or the field the transform is in.
        /// </summary>
        /// <returns></returns>
        private List<Field> ParametersToFields() {
            return Context.Process.ParametersToFields(Context.Operation.Parameters, Context.Field);
        }

        public Field SingleInput() {
            return _singleInput ?? (_singleInput = ParametersToFields().First());
        }

        /// <summary>
        /// Only used with producers, see Transform.Producers()
        /// </summary>
        /// <returns></returns>
        public Field SingleInputForMultipleOutput() {
            if (Context.Operation.Parameter != string.Empty) {
                return Context.Entity == null
                    ? Context.Process.GetAllFields().First(f =>
                        f.Alias.Equals(Context.Operation.Parameter, Sc) ||
                        f.Name.Equals(Context.Operation.Parameter, Sc))
                    : Context.Entity.GetAllFields().First(f =>
                        f.Alias.Equals(Context.Operation.Parameter, Sc) ||
                        f.Name.Equals(Context.Operation.Parameter, Sc));
            }
            return Context.Field;
        }

        public Field[] MultipleInput() {
            return ParametersToFields().ToArray();
        }

        public Field[] MultipleOutput() {
            return ParametersToFields().ToArray();
        }

        public string Received() {
            if (_received != null)
                return _received;

            var index = Context.Field.Transforms.IndexOf(Context.Operation);
            if (index <= 0)
                return SingleInput().Type;

            var previous = Context.Field.Transforms[index - 1];

            _received = previous.Returns ?? _singleInput.Type;

            return _received;
        }

        public bool IsLast() {
            var count = Context.Field.Transforms.Count;
            if (count == 1)
                return true;
            var index = Context.Field.Transforms.IndexOf(Context.Operation);
            return index == count - 1;
        }

        protected bool IsNotReceivingNumber() {
            if (!Constants.IsNumericType(Received())) {
                Run = false;
                Error(
                    $"The {Context.Operation.Method} method expects a numeric input, but is receiving a {Received()} type.");
                return true;
            }

            return false;
        }

        protected bool IsNotReceivingNumbers() {
            foreach (var field in MultipleInput()) {
                if (!field.IsNumeric()) {
                    Run = false;
                    Error(
                        $"The {Context.Operation.Method} method expects a numeric input, but is receiving a {field.Type} type from {field.Alias}.");
                    return true;
                }
            }
            return false;
        }

        protected bool IsNotReceiving(string type) {
            foreach (var f in ParametersToFields()) {
                if (f.Type.StartsWith(type))
                    continue;
                Error($"The {Context.Operation.Method} method expects {type} input, but {f.Alias} is {f.Type}.");
                Run = false;
                return true;
            }
            return false;
        }

        protected bool IsMissing(string value) {
            if (value == Constants.DefaultSetting || value == string.Empty) {
                Error($"The {Context.Operation.Method} is missing a required ({nameof(value)}) parameter.");
                Run = false;
                return true;
            }

            return false;
        }

        public virtual void Dispose() {
        }

    }
}