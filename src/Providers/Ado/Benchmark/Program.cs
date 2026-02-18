using Autofac;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.MySql.Autofac;
using Transformalize.Providers.PostgreSql.Autofac;
using Transformalize.Providers.Sqlite.Autofac;
using Transformalize.Providers.SqlServer.Autofac;
using Container = Transformalize.Containers.Autofac.Container;
using Process = Transformalize.Configuration.Process;

namespace Benchmark {

   public class Benchmarks {

      public IPipelineLogger Logger = new Transformalize.Logging.NLog.NLogPipelineLogger("test");
      private const int Size = 1000;
      private const string Password = "REDACTED";

      [Benchmark(Baseline = true, Description = "baseline")]
      public void BaseLine() {

         using (var outer = new ConfigurationContainer().CreateScope($@"files\bogus.xml?Size={Size}", Logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule()).CreateScope(process, Logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }

      }

      [Benchmark(Baseline = false, Description = "sqlserver")]
      public void SqlServer() {
         using (var outer = new ConfigurationContainer().CreateScope($@"files\bogus.xml?Size={Size}&Provider=sqlserver&User=sa&Password={Password}", Logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new SqlServerModule(process)).CreateScope(process, Logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }
      }

      [Benchmark(Baseline = false, Description = "postgresql")]
      public void Postgresql() {
         using (var outer = new ConfigurationContainer().CreateScope($@"files\bogus.xml?Size={Size}&Provider=postgresql&User=postgres&Password={Password}", Logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new PostgreSqlModule()).CreateScope(process, Logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }
      }

      [Benchmark(Baseline = false, Description = "mysql")]
      public void MySql() {
         using (var outer = new ConfigurationContainer().CreateScope($@"files\bogus.xml?Size={Size}&Provider=mysql&User=root&Password={Password}", Logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new MySqlModule()).CreateScope(process, Logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }
      }

      [Benchmark(Baseline = false, Description = "sqlite")]
      public void Sqlite() {
         using (var outer = new ConfigurationContainer().CreateScope($@"files\bogus.xml?Size={Size}&Provider=sqlite&File=c:\temp\junk.sqlite", Logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new SqliteModule()).CreateScope(process, Logger)) {
               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
            }
         }
      }

   }

   public class Program {
      private static void Main(string[] args) {
         BenchmarkRunner.Run<Benchmarks>(ManualConfig.Create(DefaultConfig.Instance).WithOptions(ConfigOptions.DisableOptimizationsValidator));
      }

   }
}
