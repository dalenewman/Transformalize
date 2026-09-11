using Transformalize.Extensions;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Logging;
using Transformalize.Transforms.Jint;
using Transformalize.Validators.Jint;

namespace Tests {
   [TestClass]
   public class TestPreparedScripts {

      [TestMethod]
      [DataRow(false)]
      [DataRow(true)]
      public void UsesCurrentRowValuesAndRepreparesWhenSourceChanges(bool validator) {
         var original = validator ? "value > 1" : "value * 2";
         var changed = validator ? "value < 1" : "value + 10";
         var context = CreateContext(original);

         IEnumerable<IRow> Input() {
            yield return CreateRow(2);
            yield return CreateRow(0);
            context.Operation.Script = changed;
            yield return CreateRow(0);
            yield return CreateRow(2);
            context.Operation.Script = original;
            yield return CreateRow(3);
         }

         var rows = Run(context, validator, Input()).ToArray();
         var field = context.Entity.GetField(validator ? "valid" : "result");
         var expected = validator
            ? new object[] { true, false, true, false, true }
            : new object[] { 4, 0, 10, 12, 6 };
         CollectionAssert.AreEqual(expected, rows.Select(r => r[field]).ToArray());
      }

      [TestMethod]
      public void PreservesScriptStateWithinAnInstanceAndKeepsInstancesIndependent() {
         const string source = "var count = (typeof count === 'undefined' ? 0 : count) + 1; value + count;";
         var first = CreateContext(source);
         var second = CreateContext(source);

         var rows = Run(first, false, new[] { CreateRow(10), CreateRow(20) }).ToArray();
         Assert.AreEqual(11, rows[0][first.Field]);
         Assert.AreEqual(22, rows[1][first.Field]);
         Assert.AreEqual(11, Run(second, false, new[] { CreateRow(10) }).Single()[second.Field]);
      }

      [TestMethod]
      [DataRow(false)]
      [DataRow(true)]
      public void ChangedSourceKeepsExistingSyntaxErrorHandling(bool validator) {
         var context = CreateContext(validator ? "value > 1" : "value * 2");

         IEnumerable<IRow> Input() {
            // Change after construction, when source validation has already completed.
            context.Operation.Script = "value +";
            yield return CreateRow(2);
         }

         Assert.AreEqual(1, Run(context, validator, Input()).Count());
         Assert.IsTrue(((MemoryLogger)context.Logger).Log.Any(l => l.Exception is global::Jint.Runtime.JavaScriptException));
      }

      [TestMethod]
      public async System.Threading.Tasks.Task StreamsWithOneEngineAndHandlesScriptChangesBetweenRows() {
         var context = CreateContext("var count = (typeof count === 'undefined' ? 0 : count) + 1; value + count;");
         using var operation = new JintTransform(context: context);
         IEnumerable<IRow> Input() {
            yield return CreateRow(10);
            yield return CreateRow(20);
            context.Operation.Script = "value + 100";
            yield return CreateRow(30);
         }
         var rows = await operation.OperateStreamAsync(Input().AsAsyncStream()).MaterializeAsync();
         CollectionAssert.AreEqual(new object[] { 11, 22, 130 }, rows.Select(r => r[context.Field]).ToArray());
         var other = CreateContext("var count = (typeof count === 'undefined' ? 0 : count) + 1; value + count;");
         using var otherOperation = new JintTransform(context: other);
         var otherRows = await otherOperation.OperateStreamAsync(new[] { CreateRow(10) }.AsAsyncStream()).MaterializeAsync();
         Assert.AreEqual(11, otherRows[0][other.Field]);
      }

      [TestMethod]
      public async System.Threading.Tasks.Task StreamingPreservesFirstRowSyntaxErrorHandling() {
         var context = CreateContext("value * 2");
         using var operation = new JintTransform(context: context);
         context.Operation.Script = "value +";
         var rows = await operation.OperateStreamAsync(new[] { CreateRow(2) }.AsAsyncStream()).MaterializeAsync();
         Assert.AreEqual(1, rows.Count);
         Assert.IsTrue(((MemoryLogger)context.Logger).Log.Any(l => l.Exception is global::Jint.Runtime.JavaScriptException));
      }

      private static IEnumerable<IRow> Run(PipelineContext context, bool validator, IEnumerable<IRow> rows) {
         if (validator) {
            var operation = new JintValidator(context: context);
            foreach (var row in rows) {
               yield return operation.Operate(row);
            }
         } else {
            var operation = new JintTransform(context: context);
            foreach (var row in operation.Operate(rows)) {
               yield return row;
            }
         }
      }

      private static PipelineContext CreateContext(string source) {
         var fields = new List<Field> {
            new Field { Name = "value", Alias = "value", Type = "int", Index = 0, MasterIndex = 0 },
            new Field { Name = "result", Alias = "result", Type = "int", Index = 1, MasterIndex = 1, ValidField = "valid", MessageField = "message" },
            new Field { Name = "valid", Alias = "valid", Type = "bool", Index = 2, MasterIndex = 2 },
            new Field { Name = "message", Alias = "message", Type = "string", Index = 3, MasterIndex = 3 }
         };
         var entity = new Entity { Name = "Data", Alias = "Data", Fields = fields };
         var process = new Process { Entities = new List<Entity> { entity } };
         var operation = new Operation {
            Method = "js", Script = source,
            Parameters = new List<Parameter> { new Parameter { Field = "value", Entity = "Data" } }
         };
         return new PipelineContext(new MemoryLogger(LogLevel.Debug), process, entity, fields[1], operation);
      }

      private static IRow CreateRow(int value) {
         return new MasterRow(4) { Storage = new object[] { value, 0, true, string.Empty } };
      }
   }
}
