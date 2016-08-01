#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {

    public abstract class BaseTransform : ITransform {
        public IContext Context { get; private set; }

        public abstract IRow Transform(IRow row);

        public virtual IEnumerable<IRow> Transform(IEnumerable<IRow> rows) {
            return rows.Select(Transform);
        }

        protected BaseTransform(IContext context) {
            Context = context;
        }

        public long RowCount { get; set; }

        protected virtual void Increment() {
            RowCount++;
            if (RowCount % Context.Entity.LogInterval == 0) {
                Context.Info(RowCount.ToString());
            }
        }

        /// <summary>
        /// A transformer's input can be entity fields, process fields, or the field the transform is in.
        /// </summary>
        /// <returns></returns>
        List<Field> ParametersToFields() {
            return Context.Process.ParametersToFields(Context.Transform.Parameters, Context.Field);
        }

        public Field SingleInput() {
            return ParametersToFields().First();
        }

        /// <summary>
        /// Only used with producers, see Transform.Producers()
        /// </summary>
        /// <returns></returns>
        public Field SingleInputForMultipleOutput() {
            if (Context.Transform.Parameter != string.Empty) {
                return Context.Entity == null
                    ? Context.Process.GetAllFields().First(f => f.Alias.Equals(Context.Transform.Parameter, StringComparison.OrdinalIgnoreCase) || f.Name.Equals(Context.Transform.Parameter, StringComparison.OrdinalIgnoreCase))
                    : Context.Entity.GetAllFields().First(f => f.Alias.Equals(Context.Transform.Parameter, StringComparison.OrdinalIgnoreCase) || f.Name.Equals(Context.Transform.Parameter, StringComparison.OrdinalIgnoreCase));
            }
            return Context.Field;
        }

        public Field[] MultipleInput() {
            return ParametersToFields().ToArray();
        }

        public Field[] MultipleOutput() {
            return ParametersToFields().ToArray();
        }

        public virtual void Dispose() {
            Context = null;
        }
    }
}