using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GeoLib.Services;

namespace GeoLib.WpfHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        // it has to be out of the event handler to keep alive after the event is finish.
        private ServiceHost _serviceHost;
        private ServiceHost _serviceHostInProc;

        // This a way to communicate between threads
        // not every thread has one but the UI thread always has it
        private readonly SynchronizationContext _syncContext;

        public static MainWindow MainUI { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Threadlbl.Content = $"UI Running on Thread {Thread.CurrentThread.ManagedThreadId} || Process {Process.GetCurrentProcess().Id}";
            MainUI = this;

            // after the initialization is the right moment to get the UI thread.
            _syncContext = SynchronizationContext.Current;
        }


        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            StartBtn.IsEnabled = false;

            _serviceHost = new ServiceHost(typeof(GeoManager));
            _serviceHost.Open();

            _serviceHostInProc = new ServiceHost(typeof(MessagerManager));
            _serviceHostInProc.Open();

            StopBtn.IsEnabled = true;
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            _serviceHost.Close();
            _serviceHostInProc.Close();

            StartBtn.IsEnabled = true;
            StopBtn.IsEnabled = false;
        }

        public void ShowMessage(string message)
        {
            // As this and UI threads are different, we need a way to communicate with the UI thread.
            int threadId = Thread.CurrentThread.ManagedThreadId;
            
            // This a delegate that has the instructions we need to update the UI
            SendOrPostCallback callback = c =>
            {
                int processId = Process.GetCurrentProcess().Id;
                int marshallThreadId = Thread.CurrentThread.ManagedThreadId;
                InProcLbl.Content = $"{message} {Environment.NewLine} (marshalled from Thread {threadId} to {marshallThreadId} || Process {processId})";
            };

            // This will dispatch the callback and synchronize it with UI Thread.
            // If we need to send a additional information we may use the state property
            // as we don't, it sends null, but for example, if it was '3' in the callback above
            // the 'c' would receive this value
            _syncContext.Send(callback, null);
        }

        private async void InProcBtn_Click(object sender, RoutedEventArgs e)
        {
            string address = "net.pipe://localhost/MeesageService";
            Binding binding = new NetNamedPipeBinding();
            ChannelFactory<IMessageService> factory = new ChannelFactory<IMessageService>(binding, address);

            // This creates the proxy to consume the service.
            IMessageService proxy = factory.CreateChannel();

            // To prevent of creating deadlock this frees the UI thread.
            // To free the main thread is the async/await reason of existence
            // it put the operation in a another thread and wait it finishes.
            await Task.Run(() => proxy.ShowMessage("Who Dat !!"));

            factory.Close();
        }
    }
}
