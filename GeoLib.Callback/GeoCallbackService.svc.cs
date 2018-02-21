using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeoLib.Data.Entities;
using GeoLib.Data.Repositories;
using GeoLib.Data.Repository_Interfaces;

namespace GeoLib.Callback
{
    //The concurrency mode used here is the Reentrant. This a variation of the Single to cope with callbacks.
    //As the service can process only one call at a time, in the Single mode, it creates a lock to prevent other calls,
    //in case of a call of the same process that initiated the call, with this variation, the call is allowed 
    //to reenter the service. The only catch is that the service has to guarantee a instance mode of PerSession
    // even though is the default state is a good practice to ensure manually.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class GeoCallbackService : IGeoCallbackService
    {
        public void UpdateZipCity(IEnumerable<ZipCityData> data)
        {
            IZipCodeRepository rep = new ZipCodeRepository();

            foreach (var cityData in data)
            {
                ZipCode zip = rep.GetByZip(cityData.ZipCode);
                zip.City = cityData.City;
                ZipCode updatedZipCode = rep.Update(zip);

                //To create a proxy programmatically we use the ChannelFactory<T>,
                //to correlated method to create a callback proxy is the GetCallbackChannel from
                //the operation context. Here weren't creating anything, its already exists, 
                //we're just recovering it.
                IUpdateCallback callback = OperationContext.Current.GetCallbackChannel<IUpdateCallback>();
                if (callback == null) continue;

                // it calls back the client sending the result
                // here weren't receiving anything back from the client because is a void method
                //but it could has returning values.
                callback.ZipUpdated(cityData);

                //just signaling the returning at server.
                MessageBox.Show($"updated {cityData.City}, called the client, back at service.");
            }
        }
    }
}
