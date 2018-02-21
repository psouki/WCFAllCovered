using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace GeoLib.Callback
{
    // In order to link the callback to the main service
    // it has to explicitly name the callback contract
    [ServiceContract(CallbackContract = typeof(IUpdateCallback))]
    public interface IGeoCallbackService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateZipCity(IEnumerable<ZipCityData> data);

        //[OperationContract]
        //Task UpdateZipCityAsync(IEnumerable<ZipCityData> data);
    }

    //Callback contract that the client has to implement in order to execute the callback
    [ServiceContract]
    public interface IUpdateCallback
    {
        //A operation could be One Way =  true, this means fire and forget call
        //the client make a call and don't wait for any response, for that reason it has to be void.
        //In this case, it is set to the default for demonstration purpose
        //what makes the communication Request and Response instead of Fire an Forget.
        [OperationContract(IsOneWay = false)]
        void ZipUpdated(ZipCityData data);
    }
}
