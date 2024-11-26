using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// add assembly System.ServiceModel  and using for the corresponding model
using System.ServiceModel;
using ConsoleApp_for_Self_Hosted_WS.models;
using ConsoleApp_for_Self_Hosted_WS.contracts;
using System.ServiceModel.Web;

namespace ConsoleApp_for_Self_Hosted_WS
{

    [ServiceContract()]
    public interface IItineraryService
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
                   UriTemplate = "GetItinerary",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json)]
        List<Step> GetItinerary(ItineraryRequest request);
    }

}



