using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace OrganizerDB
{
    public partial class OrganizerContext : DbContext
    {
        public OrganizerContext()
        {
        }

        public DbSet<PDF> PDFs { get; set; }

        public DbSet<Folder> Folders { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Tell Code First to ignore PluralizingTableName convention
            // If you keep this convention then the generated tables will have pluralized names.
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}