using Elasticsearch.Net;
using System.IO;
using System.Text;
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
         await EnsureColorsIndexAsync();
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

      private static ElasticLowLevelClient CreateClient() {
         var pool = new SingleNodeConnectionPool(new Uri(ElasticUrl));
         var settings = new ConnectionConfiguration(pool)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .BasicAuthentication(ElasticUser, ElasticPassword);
         return new ElasticLowLevelClient(settings);
      }

      private static async Task EnsureColorsIndexAsync() {
         const string indexName = "colors";
         var client = CreateClient();

         var exists = await client.Indices.ExistsAsync<DynamicResponse>(indexName);
         if (exists.Success && exists.HttpStatusCode == 200) {
            await client.Indices.DeleteAsync<DynamicResponse>(indexName);
         }

         const string mapping = @"{
  ""mappings"": {
    ""properties"": {
      ""code"": { ""type"": ""keyword"" },
      ""name"": { ""type"": ""text"" },
      ""hex"": { ""type"": ""keyword"" },
      ""rgb"": { ""type"": ""keyword"" },
      ""r"": { ""type"": ""integer"" },
      ""g"": { ""type"": ""integer"" },
      ""b"": { ""type"": ""integer"" },
      ""h"": { ""type"": ""float"" },
      ""s"": { ""type"": ""float"" },
      ""l"": { ""type"": ""float"" },
      ""total"": { ""type"": ""integer"" }
    }
  }
}";

         var create = await client.Indices.CreateAsync<DynamicResponse>(indexName, PostData.String(mapping));
         if (!create.Success) {
            throw new InvalidOperationException($"Failed to create index '{indexName}': {create}");
         }

         var colorsPath = Path.Combine(AppContext.BaseDirectory, "files", "colors.csv");
         if (!File.Exists(colorsPath)) {
            throw new FileNotFoundException($"colors.csv not found at {colorsPath}");
         }

         var lines = await File.ReadAllLinesAsync(colorsPath);
         const int batchSize = 200;
         var totalDocs = lines.Length;

         for (var batchStart = 0; batchStart < totalDocs; batchStart += batchSize) {
            var batchEnd = Math.Min(totalDocs, batchStart + batchSize);
            var sb = new StringBuilder();

            for (var i = batchStart; i < batchEnd; i++) {
               var parts = lines[i].Split(',');
               if (parts.Length < 6) {
                  continue;
               }

               var code = EscapeJson(parts[0].Trim());
               var name = EscapeJson(parts[1].Trim());
               var hex = EscapeJson(parts[2].Trim());
               var r = int.Parse(parts[3]);
               var g = int.Parse(parts[4]);
               var b = int.Parse(parts[5]);
               var (h, s, l) = RgbToHsl(r, g, b);
               var rgb = $"{r},{g},{b}";
               var total = i + 1;

               sb.AppendLine($@"{{""index"":{{""_index"":""{indexName}"",""_id"":""{total}""}}}}");
               sb.AppendLine(FormattableString.Invariant(
                  $@"{{""code"":""{code}"",""name"":""{name}"",""hex"":""{hex}"",""rgb"":""{rgb}"",""r"":{r},""g"":{g},""b"":{b},""h"":{h:F2},""s"":{s:F2},""l"":{l:F2},""total"":{total}}}"));
            }

            var bulk = await client.BulkAsync<DynamicResponse>(PostData.String(sb.ToString()));
            if (!bulk.Success) {
               throw new InvalidOperationException($"Failed to seed '{indexName}' batch {batchStart + 1}-{batchEnd}: {bulk}");
            }
         }

         await client.Indices.RefreshAsync<DynamicResponse>(indexName);
      }

      private static string EscapeJson(string value) {
         return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);
      }

      private static (double h, double s, double l) RgbToHsl(int r, int g, int b) {
         var rf = r / 255.0;
         var gf = g / 255.0;
         var bf = b / 255.0;

         var max = Math.Max(rf, Math.Max(gf, bf));
         var min = Math.Min(rf, Math.Min(gf, bf));
         var delta = max - min;

         double h = 0.0;
         if (delta > 0) {
            if (max == rf) {
               h = ((gf - bf) / delta) % 6.0;
            } else if (max == gf) {
               h = ((bf - rf) / delta) + 2.0;
            } else {
               h = ((rf - gf) / delta) + 4.0;
            }
            h *= 60.0;
            if (h < 0) {
               h += 360.0;
            }
         }

         var l = (max + min) / 2.0;
         var s = delta == 0 ? 0.0 : delta / (1.0 - Math.Abs(2.0 * l - 1.0));

         return (h, s, l);
      }
   }
}
