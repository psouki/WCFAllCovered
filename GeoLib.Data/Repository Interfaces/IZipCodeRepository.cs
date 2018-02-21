using System.Collections.Generic;
using GeoLib.Core;
using GeoLib.Data.Entities;

namespace GeoLib.Data.Repository_Interfaces
{
    public interface IZipCodeRepository : IDataRepository<ZipCode>
    {
        ZipCode GetByZip(string zip);
        IEnumerable<ZipCode> GetByState(string state);
        IEnumerable<ZipCode> GetZipsForRange(ZipCode zip, int range);
        void UpdateZipBatch(Dictionary<string, string> data);
    }
}
