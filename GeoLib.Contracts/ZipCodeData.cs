using System.Runtime.Serialization;

namespace GeoLib.Contracts
{
    // Even if the real entity has more quantity and complex properties
    // the data contract has to focus in what the client need
    // the data contract is what the client will receive.
    [DataContract]
    public class ZipCodeData : IExtensibleDataObject
    {
        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string City { get; set; }

        //This is use to make the services a little more version tolerant 
        // in case that the object received has more properties that expected 
        // the service will put it in this object to not lose it.
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
