using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GeoLib.ClientCallback.GeoCallbackService;

namespace GeoLib.ClientCallback
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// [

    // it puts the callback entering call in different thread of the UI to avoid deadlock
    [CallbackBehavior(UseSynchronizationContext = false)]
    public partial class MainWindow : IGeoCallbackServiceCallback
    {
        public MainWindow()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;
        }

        readonly SynchronizationContext _syncContext = null;

        //[OperationBehavior(TransactionScopeRequired = true)]
        private async void GetInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<ZipCityData> cityBatch = new List<ZipCityData>()
            {
                new ZipCityData {ZipCode = "70112", City = "Who Data Nation"},
                new ZipCityData {ZipCode = "30313", City = "Next SuperBowl"},
                new ZipCityData {ZipCode = "02035", City = "Cheat City"},
                new ZipCityData {ZipCode = "33056", City = "Dan Marino City"}
            };

            ZipCodeLsb.Items.Clear();

            // in order to prevent deadlock it has to put tis instructions in another thread
            // to free the UI thread
            await Task.Run(() =>
            {
                try
                {
                    //All the action has to belong to a different thread otherwise after the first callback
                    // it will result in a deadlock
                    GeoCallbackServiceClient proxy = new GeoCallbackServiceClient(new InstanceContext(this));
                    proxy.Open();

                    proxy.UpdateZipCity(cityBatch.ToList());
                    proxy.Close();

                    MessageBox.Show("Updated");
                }
                catch (FaultException ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            });
        }

        private async void RestoreDataBtn_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<ZipCityData> cityBatch = new List<ZipCityData>()
            {
                new ZipCityData {ZipCode = "70112", City = "New Orleans"},
                new ZipCityData {ZipCode = "30313", City = "Atlanta"},
                new ZipCityData {ZipCode = "02035", City = "Foxborough"},
                new ZipCityData {ZipCode = "33056", City = "Miami"}
            };

            ZipCodeLsb.Items.Clear();
            
            await Task.Run(() =>
            {
                try
                {
                    GeoCallbackServiceClient proxy = new GeoCallbackServiceClient(new InstanceContext(this));
                    proxy.Open();

                    proxy.UpdateZipCity(cityBatch.ToList());
                    proxy.Close();

                    MessageBox.Show("Restored");
                }
                catch (FaultException ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            });

            MessageBox.Show("finish");
        }

        //If this callback call doesn't need to return anything it should be marked as IsOneWay = true
        //in the IUpdateCallback contract to make it 'fire and forget' communication that will help avoiding deadlock. 
        //in this case the service will not wait to receive any call from the client. 
        public void ZipUpdated(ZipCityData data)
        {
            // As it is in a different thread of the UI it has to marshal to it.
            // this means send the contents of this thread to the UI thread.
            SendOrPostCallback updateUi = arg =>
            {
                ZipCodeLsb.Items.Add(data);
            };

            _syncContext.Send(updateUi, null);

        }
    }
}
