using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Logging;
using Transformalize.Providers.Console;

namespace Tests {

   [TestClass]
   public class TestParameterTransforms {

      [TestMethod]
      public void TestProcessName() {

         var cfg = @"<cfg name='test'></cfg>";
         using (var c = new ConfigurationContainer().CreateScope(cfg, new DebugLogger())) {
            var process = c.Resolve<Process>();
            Assert.AreEqual(0, process.Errors().Length);
            Assert.AreEqual("test", process.Name);
         }
      }

      [TestMethod]
      public void TestParameterTransform() {

         var cfg = @"<cfg name='test'><parameters><add name='x' value='x' t='upper().append(y)' /></parameters></cfg>";
         using (var c = new ConfigurationContainer().CreateScope(cfg, new DebugLogger())) {
            var process = c.Resolve<Process>();
            Assert.AreEqual(0, process.Errors().Length);
            Assert.AreEqual("Xy", process.Parameters.First().Value);
         }
      }

      [TestMethod]
      public void TestBadAttribute() {

         var cfg = @"<cfg name='test' bad='bad'></cfg>";
         using (var c = new ConfigurationContainer().CreateScope(cfg, new DebugLogger())) {
            var process = c.Resolve<Process>();
            Assert.AreEqual(1, process.Errors().Length);
         }
      }

      [TestMethod]
      public void TestParameterTransform2() {

         var cfg = @"<cfg name='test'>
   <parameters>
      <add name='OpCo' type='int' value='4000' />
      <add name='Op' t='copy(OpCo).convert().left(1)' />
   </parameters>
</cfg>";
         using (var c = new ConfigurationContainer().CreateScope(cfg, new ConsoleLogger())) {
            var process = c.Resolve<Process>();
            Assert.AreEqual(0, process.Errors().Length);
            Assert.AreEqual("4", process.Parameters.Last().Value);
         }
      }


   }
}
