using Microsoft.EntityFrameworkCore;
using AMRVI.Models;

namespace AMRVI.Data
{
    public class ProductionDbContext : DbContext
    {
        public ProductionDbContext(DbContextOptions<ProductionDbContext> options)
            : base(options)
        {
        }

        public DbSet<ScwLog> ScwLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapping for Production SCW Logs (Table in ELWP_PRD database)
            // Note: Schema 'produksi' is specified here
            modelBuilder.Entity<ScwLog>().ToTable("tb_elwp_produksi_scw_logs", "produksi", t => t.ExcludeFromMigrations());
        }
    }
}
