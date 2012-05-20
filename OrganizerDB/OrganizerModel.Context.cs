using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace OrganizerDB
{
    public partial class OrganizerContext : DbContext
    {
        public OrganizerContext()
        {
        }

        public DbSet<Part> Parts { get; set; }

        public DbSet<Datasheet> Datasheets { get; set; }

        public DbSet<Manufacturer> Manufacturers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Tell Code First to ignore PluralizingTableName convention
            // If you keep this convention then the generated tables will have pluralized names.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}