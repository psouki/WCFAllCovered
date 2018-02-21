using System.Collections.Generic;
using System.ServiceModel;

namespace GeoLib.Contracts
{
    // The contract is what the client will see
    // the concrete implementation of this interface will encapsulate 
    // the real actions that the service is doing and 
    // the client doesn't need even to bother 
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IGeoService
    {
        [OperationContract]
        // in order to the client to understand that the NotFoundData class is a 
        // custom error class it has to be defined in the contract
        // it is a question of good communication.
        [FaultContract(typeof(NotFoundData))]
        ZipCodeData GetZipInfo(string zip);

        [OperationContract]
        IEnumerable<string> GetStates(bool primaryOnly);

        [OperationContract(Name = "GetZipByState")]
        IEnumerable<ZipCodeData> GetZip(string state);

        [OperationContract(Name = "GetZipForRange")]
        IEnumerable<ZipCodeData> GetZip(string zip, int range);

        [OperationContract]
        //This is the explicit permission for the transaction flow from the client to service and back.
        //It already said, it all about communication and this is the contract. 
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateZipCity(string zip, string city);

        [OperationContract(Name = "UpdateZipCityBatch")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateZipCty(IEnumerable<ZipCityData> zipCityData);
    }
}
