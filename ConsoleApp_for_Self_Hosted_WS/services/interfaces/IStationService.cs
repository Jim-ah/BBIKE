using ConsoleApp_for_Self_Hosted_WS.models;

namespace ConsoleApp_for_Self_Hosted_WS.services.interfaces
{
    public interface IStationService
    {
        (Station originStation, Station destinationStation) GetClosestStations(MLocation origin, MLocation destination);
    }
}
