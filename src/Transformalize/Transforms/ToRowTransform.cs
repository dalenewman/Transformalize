using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
   public class ToRowTransform : BaseTransform {

      private readonly IRowFactory _rowFactory;
      private readonly Field _input;
      private readonly Field[] _fields;
      private readonly Field _hashCode;
      private Field[] _fieldsToHash;

      public ToRowTransform(IContext context = null, IRowFactory rowFactory = null) : base(context, null) {

         if (IsMissingContext()) {
            return;
         }

         var lastOperation = LastOperation();
         if (lastOperation == null) {
            Error($"The toRow operation should receive an array. You may want proceed it with a split operation.");
            Run = false;
            return;
         }

         if (!lastOperation.ProducesArray) {
            Error($"The toRow operation should receive an array. The {lastOperation.Method} method is not producing an array.");
            Run = false;
            return;
         }

         if (rowFactory == null) {
            Run = false;
            Context.Error("The toRow() method did not receive a row factory.");
            return;
         }

         ProducesRows = true;

         _rowFactory = rowFactory;
         _fields = Context.Entity.GetAllFields().ToArray();
         _input = SingleInput();

         _hashCode = Context.Entity.TflHashCode();
         _fieldsToHash = _fields.Where(f => !f.System).ToArray();
      }

      public override IRow Operate(IRow row) {
         throw new NotImplementedException();
      }

      public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
         foreach (var outer in rows) {
            var values = (string[])outer[_input];
            if (values.Length == 0) {
               yield return outer;
            } else {
               foreach (var value in values) {
                  var inner = _rowFactory.Clone(outer, _fields);
                  inner[Context.Field] = value;

                  if (!Context.Process.ReadOnly) {
                     inner[_hashCode] = HashcodeTransform.GetDeterministicHashCode(_fieldsToHash.Select(f => inner[f]));
                  }

                  yield return inner;
               }
            }

         }
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("torow");
         yield return new OperationSignature("torows");
      }
   }
}