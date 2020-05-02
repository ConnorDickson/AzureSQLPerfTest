using AzureSQLPerfTest.Models;
using System.Data.Entity;

namespace AzureSQLPerfTest.DAL
{
    public class TestContext : DbContext
    {
        public TestContext(string connectionString) : base(connectionString)
        {
        }

        public DbSet<Test> Tests { get; set; }
    }
}
