using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests {
   [TestClass]
   public class TestAssemblySetup {

      [AssemblyInitialize]
      public static async Task AssemblyInit(TestContext context) {
         await Tester.InitializeContainers();
      }

      [AssemblyCleanup]
      public static async Task AssemblyCleanup() {
         await Tester.DisposeContainers();
      }
   }
}
