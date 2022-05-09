using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using orderconsumer.Models;

namespace orderconsumerapi.Model
{
    public class OrderconsumerDbContext : DbContext
    {
        // configuration's like which database and what are objects to be created in the db
        private readonly IConfiguration Configuration;
        public DbSet<OrderConsumer> Orders { get; set; }
        // Database
        public OrderconsumerDbContext(DbContextOptions<OrderconsumerDbContext> options, IConfiguration configuration, ILogger<OrderconsumerDbContext> logger) : base(options)
        {
            Configuration = configuration;
            _logger = logger;
        }
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


        // database object to be created - tables 
        public DbSet<OrderConsumer> OrderConsumers { get; set; }

    }
}
