using Autofac;
using Transformalize.Contracts;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Transformalize.Containers.Autofac;
using Transformalize.Logging;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Transforms.LambdaParser.Autofac;
using Transformalize.Configuration;

namespace Benchmark {

   public class Benchmarks {

      [Benchmark(Baseline = true, Description = "5000 rows")]
      public void BaselineRows() {
         var logger = new NullLogger();
         using (var outer = new ConfigurationContainer(new BogusModule(), new LambdaParserModule()).CreateScope(@"files\bogus.xml?Size=5000", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new LambdaParserModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }
      }

      [Benchmark(Baseline = false, Description = "5000 rows 1 eval")]
      public void LambdaParserRows() {
         var logger = new NullLogger();
         using (var outer = new ConfigurationContainer(new BogusModule(), new LambdaParserModule()).CreateScope(@"files\bogus-lambda-parser.xml?Size=5000", logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new LambdaParserModule()).CreateScope(process, logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }
      }

   }

   public class Program {
      private static void Main(string[] args) {
         BenchmarkRunner.Run<Benchmarks>();
      }
   }
}
