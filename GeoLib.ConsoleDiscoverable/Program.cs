using System;
using System.ServiceModel;
using GeoLib.Services;

namespace GeoLib.ConsoleDiscoverable
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(GeoManager));
            host.Open();

            Console.WriteLine("Service started. Press [Enter] to close it.");
            Console.ReadKey();

            host.Close();
        }
    }
}
