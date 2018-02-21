using System;
using System.ServiceModel;
using GeoLib.Callback;

namespace GeoLib.ConsoleCallback
{
    class Program
    {
        static void Main(string[] args)
        {

            ServiceHost host = new ServiceHost(typeof(GeoCallbackService));
            host.Open();

            Console.WriteLine("Service started. Press [Enter] to close it.");
            Console.ReadKey();

            host.Close();
        }
    }
}
