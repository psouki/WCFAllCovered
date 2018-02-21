using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows;
//As it was used the service reference, visual studio build for us
// a copy of the contract, the proxy, a copy of the ZipCodeData, the reference in the App.config
// It did all the pluming for us, so It wasn't need to add any project reference. It is self sufficient
using GeoLib.ClientWithReference.GeoService;
using GeoLib.Contracts;


namespace GeoLib.ClientWithReference
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            // The GeoServiceClient is a robuster form of the we did in the project GeoLib.Proxies
            GeoServiceClient proxy = new GeoServiceClient();

            if (string.IsNullOrEmpty(ZipCodeTxt.Text)) return;

            // when using wcf channel error handling is a good practice 
            // not use "using" because in case of exception 
            // the right method to call is Abort and not Close method. 
            // "Using" implements IDisposable and just invoke the Close method.

            try
            {
                //ZipCodeData data = proxy.GetZipInfo(ZipCodeTxt.Text);
                ZipCodeData data = proxy.GetZipInfo(ZipCodeTxt.Text);
                if (data == null) return;

                CityLbl.Content = data.City;
                StateLbl.Content = data.State;

                proxy.Close();
            }
            // In the try block the exception order has to be from the more specific to the general.
            // This will catch only if is a FaultException of ExceptionDetail
            catch (FaultException<ExceptionDetail> ex)
            {
                // This is the right method to call inside a catch block because,
                // the Close may leave behind some unreleased resources.
                proxy.Abort();
                MessageBox.Show($"FaultException<ExceptionDetail> thrown by the service {Environment.NewLine}" +
                                $"Reason:  {ex.Message} {Environment.NewLine}" +
                                $"Message:  {ex.Detail.Message} {Environment.NewLine}" +
                                $"Proxy state: {proxy.State} {Environment.NewLine}");

            }
            // This will catch only if is a FaultException of the custom error object NotFoundData.
            catch (FaultException<NotFoundData> ex)
            {
                proxy.Abort();
                MessageBox.Show($"FaultException<NotFoundData> thrown by the service {Environment.NewLine}" +
                                $"Reason:  {ex.Message} {Environment.NewLine}" +
                                $"Message:  {ex.Detail.Message} {Environment.NewLine}" +
                                $"Time:  {ex.Detail.When} {Environment.NewLine}" +
                                $"User:  {ex.Detail.User} {Environment.NewLine}" +
                                $"Proxy state: {proxy.State} {Environment.NewLine}");
            }
            // This will catch any kind of FaultException.
            // Every exception thrown by the wcf service will be a FaultException
            // but the unhandled will receive a generic message
            catch (FaultException ex)
            {
                proxy.Abort();
                MessageBox.Show($"FaultException thrown by the service {Environment.NewLine}" +
                                $"Reason:  {ex.Message} {Environment.NewLine}" +
                                $"Proxy state: {proxy.State} {Environment.NewLine}");
            }
            // As all exception thrown by the wcf service will be a FaultException 
            // this will be reached only if the service can't be found
            catch (Exception ex)
            {
                proxy.Abort();
                MessageBox.Show($"Exception thrown by the service {Environment.NewLine}" +
                                $"Reason:  {ex.Message} {Environment.NewLine}" +
                                $"Proxy state: {proxy.State} {Environment.NewLine}");
            }

        }

        private void GetZipCodesBtn_Click(object sender, RoutedEventArgs e)
        {

            GeoServiceClient proxy = new GeoServiceClient();
            if (string.IsNullOrEmpty(StateTxt.Text)) return;

            IEnumerable<ZipCodeData> data = proxy.GetZipByState(StateTxt.Text);
            ZipCodeByStateLsb.Items.Clear();
            foreach (ZipCodeData codeData in data)
            {
                ZipCodeByStateLsb.Items.Add(codeData);
            }

            proxy.Close();
        }

        private void UpdateBatchBtn_Click(object sender, RoutedEventArgs e)
        {
            GeoServiceClient proxy = new GeoServiceClient();
            IEnumerable<ZipCityData> cityBatch = new List<ZipCityData>()
            {
                new ZipCityData {ZipCode = "70112", City = "Who Data Nation"},
                new ZipCityData {ZipCode = "30313", City = "Next SuperBowl"}
            };

            try
            {
                proxy.UpdateZipCityBatch(cityBatch.ToList());
                proxy.Close();

                MessageBox.Show("Updated");
            }
            catch (FaultException ex)
            {
                proxy.Abort();
                MessageBox.Show("Error");
            }
        }

        private void RestoreDataBtn_Click(object sender, RoutedEventArgs e)
        {
            GeoServiceClient proxy = new GeoServiceClient();
            IEnumerable<ZipCityData> cityBatch = new List<ZipCityData>()
            {
                new ZipCityData {ZipCode = "70112", City = "New Orleans"},
                new ZipCityData {ZipCode = "30313", City = "Atlanta"}
            };

            try
            {
                proxy.UpdateZipCityBatch(cityBatch.ToList());
                proxy.Close();

                MessageBox.Show("Updated");
            }
            catch (FaultException)
            {
                proxy.Abort();
                MessageBox.Show("Error");
            }
        }
    }
}
