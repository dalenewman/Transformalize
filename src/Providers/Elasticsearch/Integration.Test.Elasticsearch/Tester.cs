using Dapper;
using Microsoft.Data.SqlClient;
using Testcontainers.Elasticsearch;
using Testcontainers.MsSql;

namespace Test.Integration.Core {
   public static class Tester {

      private static MsSqlContainer? _sqlContainer;
      private static ElasticsearchContainer? _elasticContainer;

      // SQL Server connection properties
      public static string SqlServer { get; private set; } = "localhost";
      public static string SqlUser { get; private set; } = "sa";
      public static string SqlPw { get; private set; } = "DevDev1!";

      // Elasticsearch connection properties
      public static string ElasticServer { get; private set; } = "localhost";
      public static int ElasticPort { get; private set; } = 9200;
      public static string ElasticUser { get; private set; } = "elastic";
      public static string ElasticPassword { get; private set; } = "ElasticDev1";
      public static string ElasticVersion { get; private set; } = "8.12.0";
      public static string ElasticUrl { get; private set; } = "https://localhost:9200";

      public static string GetSqlConnectionString(string database) {
         return $"server={SqlServer};database={database};User Id={SqlUser};Password={SqlPw};Trust Server Certificate=True;Encrypt=true";
      }

      public static async Task InitializeContainers() {
         // Start both containers in parallel
         await Task.WhenAll(StartSqlContainer(), StartElasticContainer());

         // Initialize SQL Server databases after container is ready
         await CreateDatabases();
         await InitializeNorthwindDatabase();

         Console.WriteLine("Container initialization complete");
      }

      public static async Task DisposeContainers() {
         var tasks = new List<Task>();
         if (_sqlContainer != null) tasks.Add(_sqlContainer.DisposeAsync().AsTask());
         if (_elasticContainer != null) tasks.Add(_elasticContainer.DisposeAsync().AsTask());
         await Task.WhenAll(tasks);
      }

      private static async Task StartSqlContainer() {
         Console.WriteLine("Starting SQL Server container...");

         _sqlContainer = new MsSqlBuilder()
             .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
             .WithPassword(SqlPw)
             .WithCleanUp(true)
             .Build();

         await _sqlContainer.StartAsync();

         // Use "host,port" format understood by SQL Server connection strings
         SqlServer = $"{_sqlContainer.Hostname},{_sqlContainer.GetMappedPublicPort(MsSqlBuilder.MsSqlPort)}";
         Console.WriteLine($"SQL Server container started: {SqlServer}");
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

      private static async Task CreateDatabases() {
         using var connection = new SqlConnection(GetSqlConnectionString("master"));
         await connection.OpenAsync();
         Console.WriteLine("Creating TflNorthWind database...");
         await connection.ExecuteAsync("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TflNorthWind') CREATE DATABASE TflNorthWind");
      }

      private static async Task InitializeNorthwindDatabase() {
         Console.WriteLine("Initializing Northwind database...");

         var instnwndPath = Path.Combine(AppContext.BaseDirectory, "files", "instnwnd.sql");
         if (!File.Exists(instnwndPath)) {
            throw new FileNotFoundException($"Northwind installation script not found at {instnwndPath}");
         }

         var instnwndSql = await File.ReadAllTextAsync(instnwndPath);

         using (var connection = new SqlConnection(GetSqlConnectionString("master"))) {
            await connection.OpenAsync();
            foreach (var batch in SplitSqlBatches(instnwndSql)) {
               if (!string.IsNullOrWhiteSpace(batch)) {
                  try {
                     await connection.ExecuteAsync(batch, commandTimeout: 120);
                  } catch (Exception ex) {
                     Console.WriteLine($"Warning executing Northwind setup batch: {ex.Message}");
                  }
               }
            }
         }

         Console.WriteLine("Adding RowVersion columns to Northwind tables...");

         var rowversionPath = Path.Combine(AppContext.BaseDirectory, "files", "northwind-rowversion.sql");
         if (!File.Exists(rowversionPath)) {
            throw new FileNotFoundException($"Northwind RowVersion script not found at {rowversionPath}");
         }

         var rowversionSql = await File.ReadAllTextAsync(rowversionPath);

         using (var connection = new SqlConnection(GetSqlConnectionString("NorthWind"))) {
            await connection.OpenAsync();
            foreach (var batch in SplitSqlBatches(rowversionSql)) {
               if (!string.IsNullOrWhiteSpace(batch)) {
                  try {
                     await connection.ExecuteAsync(batch, commandTimeout: 30);
                  } catch (Exception ex) {
                     Console.WriteLine($"Warning adding RowVersion: {ex.Message}");
                  }
               }
            }
         }

         Console.WriteLine("Northwind database initialization complete");
      }

      private static IEnumerable<string> SplitSqlBatches(string sql) {
         var batches = new List<string>();
         var lines = sql.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
         var currentBatch = new List<string>();

         foreach (var line in lines) {
            if (string.Equals(line.Trim(), "GO", StringComparison.OrdinalIgnoreCase)) {
               if (currentBatch.Count > 0) {
                  batches.Add(string.Join(Environment.NewLine, currentBatch));
                  currentBatch.Clear();
               }
            } else {
               currentBatch.Add(line);
            }
         }

         if (currentBatch.Count > 0) {
            batches.Add(string.Join(Environment.NewLine, currentBatch));
         }

         return batches;
      }
   }
}
