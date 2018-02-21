using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using GeoLib.Contracts;
using GeoLib.Services;

namespace GeoLib.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            // There are lost of forms to host a WCF service. Console, WPF, Web, IIS
            // what is needed is to instantiate aserviceHost form the System.ServiceModel
            // with the desired service
            ServiceHost geoManagerHost = new ServiceHost(typeof(GeoManager));
            
            // --
            // With this we don't need any configuration reference in App.config
            string address = "net.tcp://localhost:8086/GeoService";
            Binding binding = new NetTcpBinding();
            Type contract = typeof(IGeoService);

            geoManagerHost.AddServiceEndpoint(contract, binding, address);
            //--

            geoManagerHost.Open();

            // if it gets here, the service is ready to be consumed.
            Console.WriteLine("Service started. Press [Enter] to close it.");
            Console.ReadLine();

            geoManagerHost.Close();
        }
    }
}
