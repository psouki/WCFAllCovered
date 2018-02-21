using System.Diagnostics;
using System.ServiceModel;
using System.Threading;

namespace GeoLib.WpfHost
{
    // As it accessing the UI directly, this put the operation in a worker thread 
    // to free the UI thread and help in the prevention of deadlocks
    [ServiceBehavior(UseSynchronizationContext = false)]
    public class MessagerManager : IMessageService
    {
        // as this a self-hosted and consumed service
        // we'll call a method in the UI to update a label
        public void ShowMessage(string message)
        {
            int processId = Process.GetCurrentProcess().Id;
            int threadId = Thread.CurrentThread.ManagedThreadId;
            MainWindow.MainUI.ShowMessage($"{message} - Thread {threadId} | Process {processId}");
        }
    }
}
