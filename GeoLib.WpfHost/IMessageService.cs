using System.ServiceModel;

// To exemplify that a project can be host and client at same time
// this is a contract for a self-hosted and consumed service
namespace GeoLib.WpfHost
{
    [ServiceContract]
    public interface IMessageService
    {
        [OperationContract]
        void ShowMessage(string message);
    }
}
