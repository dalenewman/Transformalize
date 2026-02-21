namespace Test {
   [TestClass]
   public class TestAssemblySetup {

      [AssemblyInitialize]
      public static async Task AssemblyInit(TestContext context) {
         await Tester.InitializeContainer();
      }

      [AssemblyCleanup]
      public static async Task AssemblyCleanup() {
         await Tester.DisposeContainer();
      }
   }
}
