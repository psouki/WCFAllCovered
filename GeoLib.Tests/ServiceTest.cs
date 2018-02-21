using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using GeoLib.Contracts;
using GeoLib.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeoLib.Tests
{
    // Integration test
    // The real thing, no use of mocks so it needs the database connection
    [TestClass]
    public class ServiceTest
    {
        [TestMethod]
        public void ZipCodeRetrival()
        {
            Type contract = typeof(IGeoService);
            Binding binding = new BasicHttpBinding();
            string addressTxt = "http://localhost/GeoService";
            EndpointAddress address = new EndpointAddress(addressTxt);

            // Creates the service
            ServiceHost host = new ServiceHost(typeof(GeoManager));
            host.AddServiceEndpoint(contract, binding, addressTxt);
            host.Open();

            // Creates the proxy
            ChannelFactory<IGeoService> proxyFactory = new ChannelFactory<IGeoService>(binding, address);
            IGeoService proxy = proxyFactory.CreateChannel();

            // Test it
            ZipCodeData data = proxy.GetZipInfo("70112");

            Assert.IsTrue(data.City == "New Orleans");
            Assert.IsTrue(data.State == "LA");
        }
    }
}
