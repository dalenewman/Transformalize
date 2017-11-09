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
using Transformalize.Impl;

namespace Transformalize.Validators {

    public abstract class BaseValidate : IValidate {

        private Field _singleInput;
        private readonly HashSet<string> _errors = new HashSet<string>();
        private readonly HashSet<string> _warnings = new HashSet<string>();
        private readonly Field _validField;
        private readonly Field _messageField;
        protected Action<IRow, string> AppendMessage;

        public IContext Context { get; }
        public bool Run { get; set; } = true;

        protected BaseValidate(IContext context) {
            Context = context;

            if (!context.Entity.TryGetField(Context.Field.ValidField, out _validField)) {
                Error($"The validator {Context.Operation.Method} can't find it's valid-field {Context.Field.ValidField}.");
                Run = false;
                return;
            }

            Context.Entity.TryGetField(Context.Field.MessageField, out _messageField);

            if (MessageField == null) {
                AppendMessage = delegate { };
            } else {
                AppendMessage = delegate (IRow row, string message) {
                    row[MessageField] = row[MessageField] + message + "|";
                };
            }
        }

        // this **must** be implemented
        public abstract IRow Operate(IRow row);

        // this *may* be implemented
        public virtual IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
            return Run && Context.Field.Validators.Any() ? rows.Select(Operate) : rows;
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
        /// A validator's input can be entity fields, process fields, or the field the validator is in.
        /// </summary>
        /// <returns></returns>
        private List<Field> ParametersToFields() {
            return Context.Process.ParametersToFields(Context.Operation.Parameters, Context.Field);
        }

        public Field SingleInput() {
            return _singleInput ?? (_singleInput = ParametersToFields().First());
        }

        public Field[] MultipleInput() {
            return ParametersToFields().ToArray();
        }

        protected bool IsNotReceivingNumber() {
            var type = SingleInput().Type;
            if (!Constants.IsNumericType(type)) {
                Run = false;
                Error($"The {Context.Operation.Method} method expects a numeric input, but is receiving a {type} type.");
                return true;
            }

            return false;
        }

        protected bool IsNotReceivingNumbers() {
            foreach (var field in MultipleInput()) {
                if (!field.IsNumeric()) {
                    Run = false;
                    Error($"The {Context.Operation.Method} method expects a numeric input, but is receiving a {field.Type} type from {field.Alias}.");
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

        public IField ValidField => _validField;

        public IField MessageField => _messageField;



        public virtual void Dispose() {
        }

        public virtual IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature();
        }
    }
}