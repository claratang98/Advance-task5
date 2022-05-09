using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using orderconsumerapi.Models;

namespace orderconsumerapi.Data
{
    public class OrderconsumerDbContext : DbContext
    {
        // configuration's like which database and what are objects to be created in the db

        // Database
        public OrderconsumerDbContext(DbContextOptions<OrderconsumerDbContext> options, IConfiguration configuration, ILogger<OrderconsumerDbContext> logger) : base(options)
        {
            Configuration = configuration;
            _logger = logger;
        }
        public IConfiguration Configuration { get; }
        public ILogger _logger { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var server = Configuration["DbServer"] ?? "localhost";
            var server = Configuration["DbServer"] ?? "db-mssql";
            var port = Configuration["DbPort"] ?? "1433"; // Default SQL Server port
            var user = Configuration["DbUser"] ?? "guest"; // Warning do not use the SA account
            var password = Configuration["Password"] ?? "guest";
            var database = Configuration["Database"] ?? "OrdersDb";

            _logger.LogInformation("Connection string:" + $"Server={server},{port};Database={database};User={user};Password={password}"
               );
            optionsBuilder.UseSqlServer(
                //Configuration.GetConnectionString("DefaultURL")
                //Configuration.GetSection("cs:DefaultURL").Value);
                $"Server={server},{port};Database={database};User={user};Password={password}"
                );
            base.OnConfiguring(optionsBuilder);
        }

        // DML - 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.SeedData();
        }
        // database object to be created - tables 
        public DbSet<Orderconsumer> Orderconsumers { get; set; }

    }
}
