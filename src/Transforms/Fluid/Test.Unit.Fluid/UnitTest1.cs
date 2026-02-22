using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Transforms.Fluid.Autofac;

namespace Test.Integration.Core {
   [TestClass]
   public class UnitTest1 {
      [TestMethod]
      public void TestMethod1() {

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer(new FluidTransformModule()).CreateScope(@"files/bogus-with-transform.xml", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new FluidTransformModule(), new BogusModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               IRow[] output = controller.Read().ToArray();

               Assert.AreEqual(5, output.Length);
               Assert.AreEqual("<span>2190</span>", output[0][process.Entities[0].CalculatedFields[0]]);
               Assert.AreEqual("<span>65</span>", output[4][process.Entities[0].CalculatedFields[0]]);
            }
         }

      }
   }
}
