using System.Runtime.Serialization;

namespace ConsoleApp_for_Self_Hosted_WS.contracts
{
    [DataContract]
    public class ItineraryRequest
    {
        [DataMember]
        public RequestCoordinates Origin { get; set; }

        [DataMember]
        public RequestCoordinates Destination { get; set; }
    }

    [DataContract]
    public class RequestCoordinates
    {
        [DataMember]
        public float Latitude { get; set; }

        [DataMember]
        public float Longitude { get; set; }
    }
}
