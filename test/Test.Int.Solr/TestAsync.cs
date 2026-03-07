using Autofac;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.Solr.Autofac;

namespace IntegrationTests {

   [TestClass]
   [DoNotParallelize]
   public class TestAsync {

      [TestMethod]
      public async Task Write773Async() {
         var core = NewCore("bogusasync");
         PrepareCoreFolder(core);
         var xml = BuildBogusWriteXml(core, includeIdentity: false, mdop: 2, insertSize: 255);
         var result = await ExecuteAsync(xml, new BogusModule(), new SolrModule());
         Assert.AreEqual((uint)1000, result.Inserts);
      }

      [TestMethod]
      public async Task Read773Async() {
         var core = NewCore("bogusasync");
         PrepareCoreFolder(core);
         await ExecuteAsync(BuildBogusWriteXml(core, includeIdentity: false, mdop: 1, insertSize: 255), new BogusModule(), new SolrModule());
         var result = await ExecuteAsync(BuildReadXml(core, Tester.SolrVersion), new BogusModule(), new SolrModule());
         Assert.AreEqual(1000, result.Rows);
      }

      [TestMethod]
      public async Task Write773PrimaryKeyAsync() {
         var core = NewCore("boguspk");
         PrepareCoreFolder(core);
         var xml = BuildBogusWriteXml(core, includeIdentity: true, mdop: 1, insertSize: 1000);
         var result = await ExecuteAsync(xml, new BogusModule(), new SolrModule());
         Assert.AreEqual((uint)1000, result.Inserts);
      }

      [TestMethod]
      public async Task WriteDates773Async() {
         var core = NewCore("datesasync");
         PrepareCoreFolder(core);
         string xml = $@"<add name='TestProcess' mode='init'>
  <connections>
    <add name='input' provider='internal' seed='1' />
    <add name='output' provider='solr' core='{core}' folder='{Tester.SolrDataDir}' server='{Tester.SolrServer}' port='{Tester.SolrPort}' path='{Tester.SolrPath}' version='{Tester.SolrVersion}' />
  </connections>
  <entities>
    <add name='dates'>
      <rows>
         <add string='2019-05-02 13:00:00' date='2019-05-02 13:00:00' dateoffset='2019-05-02 13:00:00-04:00' datez='2019-05-02 13:00:00Z' />
         <add string='2019-05-03 13:00:00' date='2019-05-03 13:00:00' dateoffset='2019-05-03 13:00:00-04:00' datez='2019-05-03 13:00:00Z' />
      </rows>
      <fields>
        <add name='string' type='string' primary-key='true' />
        <add name='date' type='datetime' />
        <add name='dateoffset' type='datetime' />
        <add name='datez' type='datetime' />
      </fields>
    </add>
  </entities>
</add>";
         var result = await ExecuteAsync(xml, new SolrModule());
         Assert.AreEqual((uint)2, result.Inserts);
      }

      [TestMethod]
      public async Task Read773FastPagingAsync() {
         var core = NewCore("bogusfast");
         PrepareCoreFolder(core);
         await ExecuteAsync(BuildBogusWriteXml(core, includeIdentity: true, mdop: 1, insertSize: 1000), new BogusModule(), new SolrModule());
         var result = await ExecuteAsync(BuildReadFastPagingXml(core, Tester.SolrVersion), new BogusModule(), new SolrModule());
         Assert.AreEqual(1000, result.Rows);
      }

      [TestMethod]
      public async Task Read773SlowPagingAsync() {
         var core = NewCore("bogusslow");
         PrepareCoreFolder(core);
         await ExecuteAsync(BuildBogusWriteXml(core, includeIdentity: false, mdop: 1, insertSize: 255), new BogusModule(), new SolrModule());
         var result = await ExecuteAsync(BuildReadXml(core, "4.6"), new BogusModule(), new SolrModule());
         Assert.AreEqual(1000, result.Rows);
      }

      [TestMethod]
      public async Task ReadWithExpression773Async() {
         var core = NewCore("bogusexp");
         PrepareCoreFolder(core);
         await ExecuteAsync(BuildBogusWriteXml(core, includeIdentity: false, mdop: 1, insertSize: 255), new BogusModule(), new SolrModule());
         var result = await ExecuteAsync(BuildReadWithExpressionXml(core), new BogusModule(), new SolrModule());
         Assert.AreEqual(1, result.Rows);
      }

      private static string NewCore(string prefix) {
         return $"{prefix}{Guid.NewGuid():N}".Substring(0, 14);
      }

      private static void PrepareCoreFolder(string core) {
         var coreFolder = Path.Combine(Tester.SolrDataDir, core);
         Directory.CreateDirectory(coreFolder);
         if (!OperatingSystem.IsWindows()) {
            try {
               using var process = global::System.Diagnostics.Process.Start("chmod", $"-R 777 {coreFolder}");
               process?.WaitForExit();
            } catch {
               // best effort
            }
         }
      }

      private static async Task<(uint Inserts, int Rows)> ExecuteAsync(string xml, params Autofac.Module[] modules) {
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(modules).CreateScope(process, logger)) {
               await inner.Resolve<IProcessController>().ExecuteAsync();
               var entity = process.Entities.FirstOrDefault();
               return (entity?.Inserts ?? 0, entity?.Rows?.Count ?? 0);
            }
         }
      }

      private static string BuildBogusWriteXml(string core, bool includeIdentity, int mdop, int insertSize) {
         var identityField = includeIdentity ? "\n        <add name='Identity' type='int' primary-key='true' />" : string.Empty;
         return $@"<add name='TestProcess' mode='init'>
  <parameters>
    <add name='Size' type='int' value='1000' />
    <add name='MDOP' type='int' value='{mdop}' />
  </parameters>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='solr' core='{core}' server='{Tester.SolrServer}' folder='{Tester.SolrDataDir}' version='{Tester.SolrVersion}' path='{Tester.SolrPath}' port='{Tester.SolrPort}' max-degree-of-parallelism='@[MDOP]' request-timeout='100' />
  </connections>
  <entities>
    <add name='Contact' size='@[Size]' insert-size='{insertSize}'>
      <fields>{identityField}
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' min='1' max='5' />
        <add name='Reviewers' type='int' min='0' max='500' />
      </fields>
    </add>
  </entities>
</add>";
      }

      private static string BuildReadXml(string core, string version) {
         return $@"<add name='TestProcess' read-only='true'>
  <connections>
    <add name='input' provider='solr' core='{core}' folder='{Tester.SolrDataDir}' server='{Tester.SolrServer}' port='{Tester.SolrPort}' path='{Tester.SolrPath}' version='{version}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Contact'>
      <fields>
        <add name='firstname' />
        <add name='lastname' />
        <add name='stars' type='byte' />
        <add name='reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>";
      }

      private static string BuildReadFastPagingXml(string core, string version) {
         return $@"<add name='TestProcess' read-only='true'>
  <connections>
    <add name='input' provider='solr' core='{core}' folder='{Tester.SolrDataDir}' server='{Tester.SolrServer}' port='{Tester.SolrPort}' path='{Tester.SolrPath}' version='{version}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Contact'>
      <fields>
        <add name='identity' type='int' primary-key='true' output='false' />
        <add name='firstname' />
        <add name='lastname' />
        <add name='stars' type='byte' />
        <add name='reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>";
      }

      private static string BuildReadWithExpressionXml(string core) {
         return $@"<add name='TestProcess'>
  <connections>
    <add name='input' provider='solr' core='{core}' server='{Tester.SolrServer}' folder='{Tester.SolrDataDir}' path='{Tester.SolrPath}' port='{Tester.SolrPort}' version='{Tester.SolrVersion}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Contact'>
      <filter>
         <add expression='firstname:Justin AND lastname:K*' type='search' />
      </filter>
      <fields>
        <add name='firstname' />
        <add name='lastname' />
        <add name='stars' type='byte' />
        <add name='reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>";
      }
   }
}
