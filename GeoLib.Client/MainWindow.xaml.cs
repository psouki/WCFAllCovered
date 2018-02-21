using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using GeoLib.Contracts;
using GeoLib.Proxies;

namespace GeoLib.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = $"Client running on {Thread.CurrentThread.ManagedThreadId} | Process {Process.GetCurrentProcess().Id}";
        }

        private void GetInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ZipCodeTxt.Text)) return;

            // As we have two endpoints for the same client
            // we need to specify a name for each one.
            // Here we are creating the communication to the service
            using (GeoClient proxy = new GeoClient("WsEp"))
            {
                ZipCodeData data = proxy.GetZipInfo(ZipCodeTxt.Text);
                if (data == null) return;

                CityLbl.Content = data.City;
                StateLbl.Content = data.State;
            }
        }

        private void GetZipCodesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(StateTxt.Text)) return;

            // We could use this, but the generic binding doesn't have access to lot of properties
            //Binding binding = new BasicHttpBinding();

            //To access the MaxReceivedMessageSize we need this instance.
            BasicHttpBinding binding = new BasicHttpBinding {MaxReceivedMessageSize = int.MaxValue};

            EndpointAddress address = new EndpointAddress("http://localhost/GeoService");



            //This way we can override the configuration
            //using (GeoClient proxy = new GeoClient(binding, address))
            //{
            //    IEnumerable<ZipCodeData> data = proxy.GetZip(StateTxt.Text);

            //    if (data == null) return;

            //    foreach (ZipCodeData zipCodeData in data)
            //    {
            //        ZipCodeByStateLsb.Items.Add(zipCodeData);
            //    }
            //}

            // Or even create a proxy on the fly 
            // the only things we need to communicate with the service are
            // contract, binding and endpoint.
            ChannelFactory<IGeoService> proxyFactory = new ChannelFactory<IGeoService>(binding, address);
            IGeoService proxy = proxyFactory.CreateChannel();
            IEnumerable<ZipCodeData> data = proxy.GetZip(StateTxt.Text);

            if (data == null) return;

            ZipCodeByStateLsb.Items.Clear();
            foreach (ZipCodeData zipCodeData in data)
            {
                ZipCodeByStateLsb.Items.Add(zipCodeData);
            }

            proxyFactory.Close();
        }
    }
}
