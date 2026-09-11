#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Transforms {

   public abstract class BaseTransform : ITransform, IOperateStream {

      private string _returns;
      private const StringComparison Sc = StringComparison.OrdinalIgnoreCase;
      private Field _singleInput;
      private string _received;
      private readonly HashSet<string> _errors = new HashSet<string>();
      private readonly HashSet<string> _warnings = new HashSet<string>();

      public IContext Context { get; }
      public bool Run { get; set; } = true;
      public bool ProducesFields { get; set; } = false;
      public bool ProducesRows { get; set; } = false;

      protected BaseTransform(IContext context, string returns) {
         Context = context;
         Returns = returns;
      }

      // this **must** be implemented
      public abstract IRow Operate(IRow row);

      /// <summary>
      /// One-time setup that must happen before the first row, for work that is not available at
      /// construction (loading a map, compiling a template, resolving a connection). Runs once per
      /// execution on both the synchronous and streaming paths, before any row is operated on, and
      /// may set <see cref="Run"/>. Prefer this over overriding <c>Operate(IEnumerable&lt;IRow&gt;)</c>:
      /// a sequence override forces the whole input to be buffered on the streaming path.
      /// </summary>
      protected virtual void Initialize() { }

      public virtual IAsyncEnumerable<IRow> OperateStreamAsync(IAsyncEnumerable<IRow> rows,
         CancellationToken token = default) {
         token.ThrowIfCancellationRequested();
         // A derived sequence overload may expand, aggregate, initialize or finalize rows.
         // Only the unchanged base Select contract is safe to translate to per-row calls.
         var sequenceMethod = GetType().GetMethod(nameof(Operate), new[] { typeof(IEnumerable<IRow>) });
         if (sequenceMethod.DeclaringType != typeof(BaseTransform)) {
            Context?.Warn($"Streaming fallback: materializing the whole input for {GetType().Name}. It overrides Operate(IEnumerable<IRow>) without a native OperateStreamAsync, so memory is not bounded.");
            return MaterializeFallbackAsync(rows, token);
         }
         // Setup runs eagerly here, at composition time, so ordering matches the synchronous path.
         // A disabled transform is a pass-through and skips setup, which may rely on fields the
         // constructor left unset when it turned Run off.
         if (!(Run && Context != null)) return rows;
         Initialize();
         return StreamAsync(rows, token);
      }

      private async IAsyncEnumerable<IRow> MaterializeFallbackAsync(IAsyncEnumerable<IRow> rows,
         [EnumeratorCancellation] CancellationToken token) {
         var buffered = await rows.MaterializeAsync(token).ConfigureAwait(false);
         foreach (var row in Operate(buffered)) {
            token.ThrowIfCancellationRequested();
            yield return row;
         }
      }

      private async IAsyncEnumerable<IRow> StreamAsync(IAsyncEnumerable<IRow> rows,
         [EnumeratorCancellation] CancellationToken token) {
         // Initialize may have turned Run off (an invalid format, for example).
         var run = Run;
         await foreach (var row in rows.WithCancellation(token).ConfigureAwait(false)) {
            token.ThrowIfCancellationRequested();
            yield return run ? Operate(row) : row;
         }
      }


      // this *may* be implemented
      public virtual IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
         if (!(Run && Context != null)) return rows;
         Initialize();
         return Run ? rows.Select(Operate) : rows;
      }

      public string Returns {
         get => Context == null ? _returns : Context.Operation.Returns;
         set {
            _returns = value;
            if (Context != null) {
               Context.Operation.Returns = value;
            }
         }
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

         var name = string.Empty;
         if (Context.Operation.Parameters.Where(p => p.Input).Any()) {
            name = Context.Operation.Parameters.First(p => p.Input).Field;
         }

         if (name != string.Empty) {
            return Context.Entity == null
                ? Context.Process.GetAllFields().First(f =>
                    f.Alias.Equals(name, Sc) ||
                    f.Name.Equals(name, Sc))
                : Context.Entity.GetAllFields().First(f =>
                    f.Alias.Equals(name, Sc) ||
                    f.Name.Equals(name, Sc));
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
         if (index <= 0) {
            if (Context.Field.IsCalculated && Context.Operation.Parameters.Any()) {
               _received = Context.Operation.Parameters.First().AsField(Context.Process).Type;
               return _received;
            }

            _received = SingleInput().Type;
            return _received;
         }

         var previous = Context.Field.Transforms[index - 1];

         _received = previous.Returns ?? SingleInput().Type;

         return _received;
      }

      public bool IsLast() {
         var count = Context.Field.Transforms.Count;
         if (count == 1)
            return true;
         var index = Context.Field.Transforms.IndexOf(Context.Operation);
         return index == count - 1;
      }

      public bool IsFirst() {
         var count = Context.Field.Transforms.Count;
         if (count == 1)
            return true;
         var index = Context.Field.Transforms.IndexOf(Context.Operation);
         return index == 0;
      }

      public Operation LastOperation() {
         var index = Context.Field.Transforms.IndexOf(Context.Operation);
         return index <= 0 ? null : Context.Field.Transforms[index - 1];
      }

      public Operation NextOperation() {
         var index = Context.Field.Transforms.IndexOf(Context.Operation);
         if (index + 1 < Context.Field.Transforms.Count) {
            return Context.Field.Transforms[index + 1];
         } else {
            return null;
         }
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

      protected bool IsMissingContext() {
         if (Context == null) {
            Run = false;
            return true;
         }
         return false;
      }

      protected bool IsNotReceivingNumbers() {
         foreach (var field in MultipleInput()) {
            if (!field.IsNumericType()) {
               Run = false;
               Error(
                   $"The {Context.Operation.Method} method expects a numeric input, but is receiving a {field.Type} type from {field.Alias}.");
               return true;
            }
         }
         return false;
      }

      protected bool IsNotReceiving(string type) {
         var received = Received();

         if (received == null) {
            foreach (var f in ParametersToFields()) {
               if (f.Type.StartsWith(type))
                  continue;
               Error($"The {Context.Operation.Method} method expects {type} input, but {f.Alias} is {f.Type}.");
               Run = false;
               return true;
            }
            return false;
         } else {
            if (received.StartsWith(type)) {
               return false;
            }

            Error($"The {Context.Operation.Method} method expects {type} input, but is receiving {received}.");
            Run = false;
            return true;
         }

      }

      protected bool IsMissing(string value) {
         if (value == Constants.DefaultSetting || string.IsNullOrEmpty(value)) {
            Error($"The {Context.Operation.Method} is missing a required ({nameof(value)}) parameter.");
            Run = false;
            return true;
         }

         return false;
      }

      public virtual IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature();
      }

      public virtual void Dispose() {
      }

   }
}