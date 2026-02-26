using Autofac;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Transforms.Json.Autofac;

namespace Test.Unit {
   [TestClass]
   public class Test {

      [TestMethod]
      public void JsonPathCombo() {
         var cfg = @"<cfg name='test'>
   <entities>
      <add name='entity'>
         <rows>
            <add key='1' json='{ ""string"": ""problematic"", ""number"": 1, ""array"": [2,3,4], ""object"" : { ""sixPointFour"": 6.4} }' />
         </rows>
         <fields>
            <add name='key' type='int' primary-key='true' />
            <add name='json' length='max' />
         </fields>
         <calculated-fields>
            <add name='string' t='copy(json).jsonpath($.string)' />
            <add name='number' type='int' t='copy(json).jsonpath($.number)' />
            <add name='array' t='copy(json).jsonpath($.array)' />
            <add name='numberInArray' type='int' t='copy(json).jsonpath($.array[2])' />
            <add name='sixPoint4' type='decimal' t='copy(json).jsonpath($.object.sixPointFour)' />
            <add name='filter' t='copy(json).jsonpath($.array[?(@!=2)])' />
         </calculated-fields>
      </add>
   </entities>
</cfg>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer(new JsonTransformModule()).CreateScope(cfg, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new JsonTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               IRow[] output = controller.Read().ToArray();

               Assert.AreEqual(1, output.Length);

               Assert.AreEqual("problematic", output[0][process.Entities[0].CalculatedFields[0]]);
               Assert.AreEqual(1, output[0][process.Entities[0].CalculatedFields[1]]);
               Assert.AreEqual(@"[
  2,
  3,
  4
]", output[0][process.Entities[0].CalculatedFields[2]]);
               Assert.AreEqual(4, output[0][process.Entities[0].CalculatedFields[3]]);
               Assert.AreEqual(6.4M, output[0][process.Entities[0].CalculatedFields[4]]);
               Assert.AreEqual("3", output[0][process.Entities[0].CalculatedFields[5]]);
            }
         }
      }

      // todo: handle malformed json

      [TestMethod]
      public void JsonPathMissingProperty() {
         var cfg = @"<cfg name='test'>
   <entities>
      <add name='entity'>
         <rows>
            <add key='1' json='{ ""color"": ""red"" }' />
            <add key='2' json='{ ""age"": 1 }' />
         </rows>
         <fields>
            <add name='key' type='int' primary-key='true' />
            <add name='json' length='max' />
         </fields>
         <calculated-fields>
            <add name='color' t='copy(json).jsonpath($.color)' />
            <add name='age' type='int' t='copy(json).jsonpath($.age)' />
         </calculated-fields>
      </add>
   </entities>
</cfg>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer(new JsonTransformModule()).CreateScope(cfg, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new JsonTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               IRow[] output = controller.Read().ToArray();

               Assert.AreEqual(2, output.Length);

               var red = output[0][process.Entities[0].CalculatedFields[0]];
               var blank = output[1][process.Entities[0].CalculatedFields[0]];
               Assert.AreEqual("red", red, "First record color should be red.");
               Assert.AreEqual("", blank, "Second record color value should be default (empty string).");

               var zero = output[0][process.Entities[0].CalculatedFields[1]];
               var one = output[1][process.Entities[0].CalculatedFields[1]];
               Assert.AreEqual(0, zero, "First record age should use default (0).");
               Assert.AreEqual(1, one, "Second record age should be 1.");
            }
         }
      }

      [TestMethod]
      public void ToJson() {
         var cfg = @"<cfg name='test'>
   <entities>
      <add name='entity'>
         <rows>
            <add num='1' company='Nvidia' ceo='Jensen Huang' />
            <add num='2' company='Microsoft' ceo='Satya Nadella' />
            <add num='3' company='Apple' ceo='Tim Cook' />
         </rows>
         <fields>
            <add name='num' type='int' primary-key='true' />
            <add name='company' />
            <add name='ceo' />
         </fields>
         <calculated-fields>
            <add name='json' t='copy(*).tojson()' />
         </calculated-fields>
      </add>
   </entities>
</cfg>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer(new JsonTransformModule()).CreateScope(cfg, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new JsonTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               IRow[] output = controller.Read().ToArray();

               Assert.AreEqual(3, output.Length);
               var field = process.Entities[0].CalculatedFields[0];
               var value = output[0][field];

               Assert.AreEqual("{\"num\":1,\"company\":\"Nvidia\",\"ceo\":\"Jensen Huang\"}", value);
            }
         }
      }


      [TestMethod]
      public void ToJsonIndented() {
         var cfg = @"<cfg name='test'>
   <entities>
      <add name='entity'>
         <rows>
            <add num='1' company='Nvidia' ceo='Jensen Huang' />
            <add num='2' company='Microsoft' ceo='Satya Nadella' />
            <add num='3' company='Apple' ceo='Tim Cook' />
         </rows>
         <fields>
            <add name='num' type='int' primary-key='true' />
            <add name='company' />
            <add name='ceo' />
         </fields>
         <calculated-fields>
            <add name='json' t='copy(num,company).tojson(indented)' />
         </calculated-fields>
      </add>
   </entities>
</cfg>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer(new JsonTransformModule()).CreateScope(cfg, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new JsonTransformModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               IRow[] output = controller.Read().ToArray();

               Assert.AreEqual(3, output.Length);
               var field = process.Entities[0].CalculatedFields[0];
               var value = output[0][field];

               Assert.AreEqual(@"{
  ""num"": 1,
  ""company"": ""Nvidia""
}", value);
            }
         }
      }
   }
}
