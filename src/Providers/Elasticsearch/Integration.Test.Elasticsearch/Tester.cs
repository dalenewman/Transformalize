using Testcontainers.Elasticsearch;

namespace Test.Integration.Core {
   public static class Tester {

      private static ElasticsearchContainer? _elasticContainer;

      // Elasticsearch connection properties
      public static string ElasticServer { get; private set; } = "localhost";
      public static int ElasticPort { get; private set; } = 9200;
      public static string ElasticUser { get; private set; } = "elastic";
      public static string ElasticPassword { get; private set; } = "ElasticDev1";
      public static string ElasticVersion { get; private set; } = "8.12.0";
      public static string ElasticUrl { get; private set; } = "https://localhost:9200";

      public static async Task InitializeContainers() {
         await StartElasticContainer();
      }

      public static async Task DisposeContainers() {
         if (_elasticContainer != null) {
            await _elasticContainer.DisposeAsync().AsTask();
         }
      }

      private static async Task StartElasticContainer() {
         Console.WriteLine("Starting Elasticsearch container...");

         _elasticContainer = new ElasticsearchBuilder()
             .WithImage("docker.elastic.co/elasticsearch/elasticsearch:8.12.0")
             .WithPassword(ElasticPassword)
             .WithCleanUp(true)
             .Build();

         await _elasticContainer.StartAsync();

         ElasticServer = _elasticContainer.Hostname;
         ElasticPort = _elasticContainer.GetMappedPublicPort(9200);
         ElasticUrl = $"https://{ElasticServer}:{ElasticPort}";
         Console.WriteLine($"Elasticsearch container started: {ElasticUrl}");
      }
   }
}
