using MySqlConnector;
using Testcontainers.MySql;

namespace Test {
   public static class Tester {

      private static MySqlContainer? _mySqlContainer;

      // MySQL connection properties (populated when container starts)
      public static string Server { get; private set; } = "localhost";
      public static int Port { get; private set; } = 3306;
      public static string User { get; private set; } = "root";
      public static string Pw { get; private set; } = "DevDev1!";

      public static string GetConnectionString(string? database = null) {
         var db = database != null ? $"Database={database};" : "";
         return $"Server={Server};Port={Port};{db}Uid={User};Pwd={Pw};AllowUserVariables=True;";
      }

      public static async Task InitializeContainer() {
         Console.WriteLine("Starting MySQL container...");

         var builder = new MySqlBuilder()
            .WithImage("mysql:8.0")
            .WithPassword(Pw)
            .WithCleanUp(true);

         var fixedPort = Environment.GetEnvironmentVariable("MYSQL_TEST_PORT");
         if (!string.IsNullOrEmpty(fixedPort) && int.TryParse(fixedPort, out var p)) {
            builder = builder.WithPortBinding(p, MySqlBuilder.MySqlPort);
         }

         _mySqlContainer = builder.Build();

         await _mySqlContainer.StartAsync();

         Server = _mySqlContainer.Hostname;
         Port = _mySqlContainer.GetMappedPublicPort(MySqlBuilder.MySqlPort);

         Console.WriteLine($"MySQL container started: {Server}:{Port}");

         await InitializeDatabases();

         Console.WriteLine("All database initialization complete");
      }

      public static async Task DisposeContainer() {
         if (_mySqlContainer != null) {
            Console.WriteLine("Stopping MySQL container...");
            await _mySqlContainer.DisposeAsync();
            Console.WriteLine("MySQL container stopped");
         }
      }

      private static async Task InitializeDatabases() {
         // Execute the northwind-mysql.sql script (creates northwind database)
         Console.WriteLine("Loading Northwind schema and data...");
         await ExecuteSqlFile("northwind-mysql.sql");

         // Add modified_at timestamp column to all entity tables for version tracking
         Console.WriteLine("Adding modified_at columns...");
         await AddModifiedAtColumns();

         // Create additional databases
         await using var cn = new MySqlConnection(GetConnectionString());
         await cn.OpenAsync();
         foreach (var db in new[] { "northwindstar", "junk" }) {
            await using var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{db}`", cn);
            await cmd.ExecuteNonQueryAsync();
            Console.WriteLine($"Created database: {db}");
         }

         Console.WriteLine("Northwind database ready");
      }

      private static async Task ExecuteSqlFile(string fileName) {
         var path = Path.Combine(AppContext.BaseDirectory, "files", fileName);
         if (!File.Exists(path))
            throw new FileNotFoundException($"SQL script not found: {path}");

         var sql = await File.ReadAllTextAsync(path);

         await using var cn = new MySqlConnection(GetConnectionString());
         await cn.OpenAsync();

         await using var cmd = new MySqlCommand(sql, cn);
         cmd.CommandTimeout = 120;
         await cmd.ExecuteNonQueryAsync();
      }

      private static async Task AddModifiedAtColumns() {
         var tables = new[] { "OrderDetail", "SalesOrder", "Customer", "Employee", "Product", "Supplier", "Category", "Shipper" };

         await using var cn = new MySqlConnection(GetConnectionString("northwind"));
         await cn.OpenAsync();

         foreach (var table in tables) {
            var sql = $"ALTER TABLE `{table}` ADD COLUMN `modified_at` TIMESTAMP(6) DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);";
            await using var cmd = new MySqlCommand(sql, cn);
            await cmd.ExecuteNonQueryAsync();
         }
      }

   }
}
