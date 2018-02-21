using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using GeoLib.Core;
using GeoLib.Data.Entities;
using GeoLib.Data.Repository_Interfaces;

namespace GeoLib.Data.Repositories
{
    public class ZipCodeRepository : DataRepositoryBase<ZipCode, GeoLibDbContext>, IZipCodeRepository
    {
        protected override DbSet<ZipCode> DbSet(GeoLibDbContext entityContext)
        {
            return entityContext.ZipCodeSet;
        }

        protected override Expression<Func<ZipCode, bool>> IdentifierPredicate(GeoLibDbContext entityContext, int id)
        {
            return (e => e.ZipCodeId == id);
        }

        public override IEnumerable<ZipCode> Get()
        {
            using (GeoLibDbContext entityContext = new GeoLibDbContext())
            {
                return entityContext.ZipCodeSet
                    .Include(e => e.State).ToFullyLoaded();
            }
        }

        public ZipCode GetByZip(string zip)
        {
            using (GeoLibDbContext entityContext = new GeoLibDbContext())
            {
                return entityContext.ZipCodeSet
                    .Include(e => e.State)
                    .FirstOrDefault(e => e.Zip == zip);
            }
        }

        public IEnumerable<ZipCode> GetByState(string state)
        {
            using (GeoLibDbContext entityContext = new GeoLibDbContext())
            {
                return entityContext.ZipCodeSet
                    .Include(e => e.State)
                    .Where(e => e.State.Abbreviation == state).ToFullyLoaded();
            }
        }

        public IEnumerable<ZipCode> GetZipsForRange(ZipCode zip, int range)
        {
            using (GeoLibDbContext entityContext = new GeoLibDbContext())
            {
                double degrees = range / 69.047;

                return entityContext.ZipCodeSet
                    .Include(e => e.State)
                    .Where(e => (e.Latitude <= zip.Latitude + degrees && e.Latitude >= zip.Latitude - degrees) &&
                                (e.Longitude <= zip.Longitude + degrees && e.Longitude >= zip.Longitude - degrees))
                    .ToFullyLoaded();
            }
        }

        public void UpdateZipBatch(Dictionary<string, string> data)
        {
            using (GeoLibDbContext entityContext = new GeoLibDbContext())
            {
                ICollection<string> cityBatch = data.Select(x => x.Key).ToList();
                var zips = entityContext.ZipCodeSet.Where(e => cityBatch.Contains(e.Zip)).ToList();
                zips.ForEach( c=> c.City = data[c.Zip]);

                entityContext.SaveChanges();
            }
        }
    }
}
