using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using GeoLib.Contracts;

// When we add a service reference, the visual studio creates this for us
// It was done by hand just to explore and explain 

// To communicate with the services we need to create a client
// This what proxies are all about, the other side of the wire
// that implements the service contract 
namespace GeoLib.Proxies
{
   // The ClientBase creates objects that can call services based in a contract
    public class GeoClient : ClientBase<IGeoService>, IGeoService
    {
        //Since we create the proxy, it will need the endpoint and the binding
        // if not provided in the configuration
        public GeoClient(string endPoint) : base(endPoint)
        {

        }

        public GeoClient(Binding binding, EndpointAddress endpoint):base(binding, endpoint)
        {
            
        }
        public ZipCodeData GetZipInfo(string zip)
        {
            // The Channel property of the ClientBase give us the connection to the service methods.
            return Channel.GetZipInfo(zip);
        }

        public IEnumerable<string> GetStates(bool primaryOnly)
        {
            return Channel.GetStates(primaryOnly);
        }

        public IEnumerable<ZipCodeData> GetZip(string state)
        {
            return Channel.GetZip(state);
        }

        public IEnumerable<ZipCodeData> GetZip(string zip, int range)
        {
            return Channel.GetZip(zip, range);
        }

        public void UpdateZipCity(string zip, string city)
        {
            Channel.UpdateZipCity(zip, city);
        }

        public void UpdateZipCty(IEnumerable<ZipCityData> zipCityData)
        {
            Channel.UpdateZipCty(zipCityData);
        }
    }
}
