using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Unit.SqlServer {
   /// <summary>
   /// Assembly-level test setup and teardown for TestContainers
   /// </summary>
   [TestClass]
   public class TestAssemblySetup {

      /// <summary>
      /// Initialize SQL Server container before any tests run
      /// </summary>
      [AssemblyInitialize]
      public static async Task AssemblyInit(TestContext context) {
         await Tester.InitializeContainer();
      }

      /// <summary>
      /// Clean up SQL Server container after all tests complete
      /// </summary>
      [AssemblyCleanup]
      public static async Task AssemblyCleanup() {
         await Tester.DisposeContainer();
      }
   }
}
