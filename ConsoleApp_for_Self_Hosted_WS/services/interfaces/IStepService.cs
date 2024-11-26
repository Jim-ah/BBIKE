using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp_for_Self_Hosted_WS.models;

namespace ConsoleApp_for_Self_Hosted_WS.services.interfaces
{
    public interface IStepService
    {
        List<Step> GetCyclingSteps(Coordinates originCoordinates, Coordinates originStationCoordinates,
        Coordinates destinationStationCoordinates, Coordinates destinationCoordinates);
        List<Step> GetWalkingSteps(Coordinates originCoordinates, Coordinates destinationCoordinates); 
    }
}
