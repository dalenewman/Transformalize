using Autofac;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Logging;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Transforms.Fluid.Autofac;

namespace Benchmark.Core {

   public class Benchmarks {


      [Benchmark(Baseline = true, Description = "10000 test rows")]
      public void BogusRows() {
         var logger = new NullLogger();
         using (var outer = new ConfigurationContainer(new FluidTransformModule()).CreateScope(@"files\bogus.xml?Size=10000", logger)) {
            var process = outer.Resolve<Process>();
            foreach (var error in process.Errors()) {
               Console.Error.WriteLine(error);
            }
            using (var inner = new Container(new FluidTransformModule(), new BogusModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }
      }

      [Benchmark(Baseline = false, Description = "10000 rows with 1 fluid")]
      public void BogusWithFluidRows() {
         var logger = new NullLogger();
         using (var outer = new ConfigurationContainer(new FluidTransformModule()).CreateScope(@"files\bogus-with-transform.xml?Size=10000", logger)) {
            var process = outer.Resolve<Process>();
            foreach(var error in process.Errors()) {
               Console.Error.WriteLine(error);
            }
            using (var inner = new Container(new FluidTransformModule(), new BogusModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }
      }

      static void Main(string[] args) {

         var summary = BenchmarkRunner.Run<Benchmarks>();
         Console.WriteLine(summary);

      }
   }
}
