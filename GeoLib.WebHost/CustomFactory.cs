using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Web;
using GeoLib.Contracts;

namespace GeoLib.WebHost
{
    public class CustomFactory : ServiceHostFactory
    {
        //This intercept the service host creation and inject a endpoint programmatically
        // it can be done in any kind of service host between the creation and start
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceHost geoServiceHost = new ServiceHost(serviceType, baseAddresses);

            // We don't need to inform any address because the web project will create
            // a address with the default address that is in the property of the project
            // in this case http://localhost:28083/ and will add the relativeAddress
            // in this case GeoService.svc configured in the web.config.
            string address = string.Empty;
            Binding binding = new WSHttpBinding();
            Type contract = typeof (IGeoService);

            geoServiceHost.AddServiceEndpoint(contract, binding, address);
           
            return geoServiceHost;
        }
    }
}