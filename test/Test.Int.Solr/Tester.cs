using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IntegrationTests {
   public static class Tester {

      private static IContainer? _solrContainer;
      private static string? _hostDataDir;

      // Solr connection properties
      public static string SolrServer { get; private set; } = "localhost";
      public static int SolrPort { get; private set; } = 8983;
      public static string SolrVersion { get; private set; } = "7.7.3";
      public static string SolrPath { get; private set; } = "solr";
      public static string SolrDataDir => _hostDataDir ?? throw new InvalidOperationException("Container not initialized");

      public static async Task InitializeContainers() {
         // Using the system temp path for cross-platform compatibility
         _hostDataDir = Path.Combine(Path.GetTempPath(), "tfl-solr-" + Guid.NewGuid().ToString("N"));
         Directory.CreateDirectory(_hostDataDir);
         
         // Write a basic solr.xml
         File.WriteAllText(Path.Combine(_hostDataDir, "solr.xml"), @"<solr>
  <int name='coreLoadThreads'>${solr.coreLoadThreads:3}</int>
  <str name='coreRootDirectory'>${coreRootDirectory:.}</str>
  <shardHandlerFactory name='shardHandlerFactory' class='HttpShardHandlerFactory'>
    <int name='socketTimeout'>${socketTimeout:600000}</int>
    <int name='connTimeout'>${connTimeout:600000}</int>
  </shardHandlerFactory>
</solr>");

         // On Linux/CI, we must ensure the container's solr user (UID 8983) can read/write the mounted directory.
         if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            try {
               var process = Process.Start("chmod", $"-R 777 {_hostDataDir}");
               if (process != null) {
                  await process.WaitForExitAsync();
               }
            } catch (Exception ex) {
               Console.WriteLine($"Warning: Failed to set permissions on {_hostDataDir}: {ex.Message}");
            }
         }
         
         await StartSolrContainer();
      }

      public static async Task DisposeContainers() {
         if (_solrContainer != null) {
            await _solrContainer.DisposeAsync().AsTask();
         }
         if (_hostDataDir != null && Directory.Exists(_hostDataDir)) {
            try {
               Directory.Delete(_hostDataDir, true);
            } catch {
               // Ignore cleanup errors
            }
         }
      }

      private static async Task StartSolrContainer() {
         Console.WriteLine($"Starting Solr container with data dir: {_hostDataDir}");

         _solrContainer = new ContainerBuilder()
             .WithImage($"solr:{SolrVersion}")
             .WithPortBinding(0, 8983) // Random host port to container 8983
             .WithBindMount(_hostDataDir!, "/var/solr/data") // Standard Solr data location
             .WithEnvironment("SOLR_HOME", "/var/solr/data")
             .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8983))
             .WithCleanUp(true)
             .Build();

         await _solrContainer.StartAsync();

         SolrServer = _solrContainer.Hostname;
         SolrPort = _solrContainer.GetMappedPublicPort(8983);
         Console.WriteLine($"Solr container started on {SolrServer}:{SolrPort}");
      }
   }
}
