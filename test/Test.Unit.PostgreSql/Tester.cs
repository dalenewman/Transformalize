using Npgsql;
using Testcontainers.PostgreSql;

namespace Test {
   public static class Tester {

      private static PostgreSqlContainer? _pgContainer;

      // PostgreSQL connection properties (populated when container starts)
      public static string Server { get; private set; } = "localhost";
      public static int Port { get; private set; } = 5432;
      public static string User { get; private set; } = "postgres";
      public static string Pw { get; private set; } = "DevDev1!";

      public static string GetConnectionString(string database) {
         return $"Host={Server};Port={Port};Database={database};Username={User};Password={Pw};";
      }

      public static async Task InitializeContainer() {
         // Npgsql 6+ requires UTC DateTimes for timestamptz columns by default.
         // The Northwind data contains TIMESTAMP columns with non-UTC values, so enable
         // legacy behavior to allow writing non-UTC DateTimes to PostgreSQL.
         // TODO: Fix in Transformalize ADO Provider
         AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

         Console.WriteLine("Starting PostgreSQL container...");

         var builder = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithUsername(User)
            .WithPassword(Pw)
            .WithDatabase("postgres")
            .WithCleanUp(true);

         var fixedPort = Environment.GetEnvironmentVariable("PGSQL_TEST_PORT");
         if (!string.IsNullOrEmpty(fixedPort) && int.TryParse(fixedPort, out var p)) {
             builder = builder. WithPortBinding(p, PostgreSqlBuilder.PostgreSqlPort);
         }
              
         _pgContainer = builder.Build();

         await _pgContainer.StartAsync();

         Server = _pgContainer.Hostname;
         Port = _pgContainer.GetMappedPublicPort(PostgreSqlBuilder.PostgreSqlPort);

         Console.WriteLine($"PostgreSQL container started: {Server}:{Port}");

         await InitializeDatabases();

         Console.WriteLine("All database initialization complete");
      }

      public static async Task DisposeContainer() {
         if (_pgContainer != null) {
            Console.WriteLine("Stopping PostgreSQL container...");
            await _pgContainer.DisposeAsync();
            Console.WriteLine("PostgreSQL container stopped");
         }
      }

      private static async Task InitializeDatabases() {
         await using (var cn = new NpgsqlConnection(GetConnectionString("postgres"))) {
            await cn.OpenAsync();
            foreach (var db in new[] { "northwind_source", "northwind_destination", "junk" }) {
               await using var check = new NpgsqlCommand(
                  $"SELECT COUNT(*) FROM pg_database WHERE datname = '{db}'", cn);
               var exists = (long)(await check.ExecuteScalarAsync())! > 0;
               if (!exists) {
                  await using var create = new NpgsqlCommand($"CREATE DATABASE {db}", cn);
                  await create.ExecuteNonQueryAsync();
                  Console.WriteLine($"Created database: {db}");
               }
            }
         }

         Console.WriteLine("Loading Northwind schema and data...");
         await ExecuteSqlFile("northwind-postgres.sql", "northwind_source");

         Console.WriteLine("Northwind database ready");
      }

      private static async Task ExecuteSqlFile(string fileName, string database) {
         var path = Path.Combine(AppContext.BaseDirectory, "files", fileName);
         if (!File.Exists(path))
            throw new FileNotFoundException($"SQL script not found: {path}");

         var sql = await File.ReadAllTextAsync(path);

         await using var cn = new NpgsqlConnection(GetConnectionString(database));
         await cn.OpenAsync();

         await using var cmd = new NpgsqlCommand(sql, cn);
         cmd.CommandTimeout = 120;
         await cmd.ExecuteNonQueryAsync();

      }

   }
}
