using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using GeoLib.Core;
using GeoLib.Data.Entities;

namespace GeoLib.Data
{
    public class GeoLibDbContext : DbContext
    {
        public GeoLibDbContext() : base("name=main")
        {
            Database.SetInitializer<GeoLibDbContext>(null);
        }

        public DbSet<ZipCode> ZipCodeSet { get; set; }
        public DbSet<State> StateSet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Ignore<IIdentifiableEntity>();

            modelBuilder.Entity<ZipCode>().HasKey(e => e.ZipCodeId).Ignore(e => e.EntityId)
                .HasRequired(e => e.State).WithMany().HasForeignKey(e => e.StateId);

            modelBuilder.Entity<State>().HasKey(e => e.StateId).Ignore(e => e.EntityId);
        }
    }
}
