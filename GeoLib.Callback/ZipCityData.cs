using System.Runtime.Serialization;

namespace GeoLib.Callback
{
    [DataContract]
    public class ZipCityData
    {
        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string ZipCode { get; set; }
    }
}
