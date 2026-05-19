using Dapper;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Testcontainers.MsSql;

namespace Test.Unit.SqlServer {
   public static class Tester {

      // Container instance (managed by AssemblyInitialize)
      private static MsSqlContainer? _container;

      // Connection properties (populated when container starts)
      public static string Server { get; private set; } = "localhost";
      public static int Port { get; private set; } = 1433;
      public static string User { get; private set; } = "sa";
      public static string Pw { get; private set; } = "DevDev1!";

      /// <summary>
      /// Build a connection string for the specified database
      /// </summary>
      public static string GetConnectionString(string database) {
         return $"server={Server},{Port};database={database};User Id={User};Password={Pw};Trust Server Certificate=True;Encrypt=true";
      }

      /// <summary>
      /// Initialize SQL Server container and databases
      /// </summary>
      public static async Task InitializeContainer() {
         Console.WriteLine("Starting SQL Server container...");

         // Build a custom image that installs mssql-server-fts on top of SQL Server 2022.
         // Dockerfile.fts uses --platform=linux/amd64 so it works on ARM64 (Apple Silicon) via emulation.
         var dockerfileDir = Path.GetDirectoryName(typeof(Tester).Assembly.Location)!;
         const string imageName = "mssql-fts:testcontainers";

         Console.WriteLine($"Building SQL Server FTS image from {dockerfileDir}...");
         var buildProcess = new Process {
            StartInfo = new ProcessStartInfo {
               FileName = "docker",
               Arguments = $"build -f \"{Path.Combine(dockerfileDir, "Dockerfile.fts")}\" -t {imageName} \"{dockerfileDir}\"",
               RedirectStandardOutput = true,
               RedirectStandardError = true,
               UseShellExecute = false
            }
         };
         buildProcess.Start();
         var buildOutput = await buildProcess.StandardOutput.ReadToEndAsync();
         var buildError = await buildProcess.StandardError.ReadToEndAsync();
         await buildProcess.WaitForExitAsync();
         if (buildProcess.ExitCode != 0)
            throw new Exception($"docker build failed:\n{buildError}");
         Console.WriteLine($"Built image: {imageName}");

         var builder = new MsSqlBuilder(imageName)
            .WithPassword(Pw)
            .WithCleanUp(true);

         var fixedPort = Environment.GetEnvironmentVariable("MSSQL_TEST_PORT");
         if (!string.IsNullOrEmpty(fixedPort) && int.TryParse(fixedPort, out var p)) {
             builder = builder. WithPortBinding(p, MsSqlBuilder.MsSqlPort);
         }

         _container = builder.Build();

         await _container.StartAsync();

         // Update connection properties from container
         Server = _container.Hostname;
         Port = _container.GetMappedPublicPort(MsSqlBuilder.MsSqlPort);

         Console.WriteLine($"SQL Server container started: {Server}:{Port}");

         // Create test databases
         await CreateDatabases();

         // Initialize Northwind database with schema and data
         await InitializeNorthwindDatabase();

         Console.WriteLine("Database initialization complete");
      }

      /// <summary>
      /// Dispose container on test completion
      /// </summary>
      public static async Task DisposeContainer() {
         if (_container != null) {
            Console.WriteLine("Stopping SQL Server container...");
            await _container.DisposeAsync();
            Console.WriteLine("SQL Server container stopped");
         }
      }

      /// <summary>
      /// Create TflNorthWind and Junk databases
      /// </summary>
      private static async Task CreateDatabases() {
         var masterConnectionString = GetConnectionString("master");

         using var connection = new SqlConnection(masterConnectionString);
         await connection.OpenAsync();

         Console.WriteLine("Creating TflNorthWind database...");
         await connection.ExecuteAsync("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TflNorthWind') CREATE DATABASE TflNorthWind");

         Console.WriteLine("Creating Junk database...");
         await connection.ExecuteAsync("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Junk') CREATE DATABASE Junk");
      }

      /// <summary>
      /// Initialize Northwind database with schema, data, and RowVersion columns
      /// </summary>
      private static async Task InitializeNorthwindDatabase() {
         Console.WriteLine("Initializing Northwind database...");

         // Read the Northwind installation script
         var instnwndPath = Path.Combine(AppContext.BaseDirectory, "files", "instnwnd.sql");
         if (!File.Exists(instnwndPath)) {
            throw new FileNotFoundException($"Northwind installation script not found at {instnwndPath}");
         }

         var instnwndSql = await File.ReadAllTextAsync(instnwndPath);

         // Execute the Northwind installation script against master database
         var masterConnectionString = GetConnectionString("master");
         using (var connection = new SqlConnection(masterConnectionString)) {
            await connection.OpenAsync();

            // Split and execute SQL batches (GO statements)
            var batches = SplitSqlBatches(instnwndSql);
            foreach (var batch in batches) {
               if (!string.IsNullOrWhiteSpace(batch)) {
                  try {
                     await connection.ExecuteAsync(batch, commandTimeout: 120);
                  } catch (Exception ex) {
                     Console.WriteLine($"Warning executing Northwind setup batch: {ex.Message}");
                     // Continue with other batches - some may fail if objects already exist
                  }
               }
            }
         }

         Console.WriteLine("Adding RowVersion columns to Northwind tables...");

         // Read and execute the RowVersion script
         var rowversionPath = Path.Combine(AppContext.BaseDirectory, "files", "northwind-rowversion.sql");
         if (!File.Exists(rowversionPath)) {
            throw new FileNotFoundException($"Northwind RowVersion script not found at {rowversionPath}");
         }

         var rowversionSql = await File.ReadAllTextAsync(rowversionPath);

         // Execute RowVersion additions against Northwind database
         var northwindConnectionString = GetConnectionString("NorthWind");
         using (var connection = new SqlConnection(northwindConnectionString)) {
            await connection.OpenAsync();

            var batches = SplitSqlBatches(rowversionSql);
            foreach (var batch in batches) {
               if (!string.IsNullOrWhiteSpace(batch)) {
                  try {
                     await connection.ExecuteAsync(batch, commandTimeout: 30);
                  } catch (Exception ex) {
                     Console.WriteLine($"Warning adding RowVersion: {ex.Message}");
                     // Continue - column may already exist
                  }
               }
            }
         }

         Console.WriteLine("Northwind database initialization complete");
      }

      /// <summary>
      /// Split SQL script into batches on GO statements
      /// </summary>
      private static IEnumerable<string> SplitSqlBatches(string sql) {
         // Split on GO statements (case-insensitive, on its own line)
         var batches = new List<string>();
         var lines = sql.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
         var currentBatch = new List<string>();

         foreach (var line in lines) {
            var trimmedLine = line.Trim();

            // Check if this line is just "GO" (case-insensitive)
            if (string.Equals(trimmedLine, "GO", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedLine, "go", StringComparison.OrdinalIgnoreCase)) {

               // Add current batch if it has content
               if (currentBatch.Count > 0) {
                  batches.Add(string.Join(Environment.NewLine, currentBatch));
                  currentBatch.Clear();
               }
            } else {
               currentBatch.Add(line);
            }
         }

         // Add final batch if any
         if (currentBatch.Count > 0) {
            batches.Add(string.Join(Environment.NewLine, currentBatch));
         }

         return batches;
      }
   }
}
