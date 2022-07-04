using Microsoft.EntityFrameworkCore;

namespace A_Vick.Telegram.DataAccess
{
    public class DataContext : DbContext
    {
        public const string SchemaName = "av";

        public DataContext() { }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        }
    }
}