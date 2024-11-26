using System.Collections.Generic;
using ConsoleApp_for_Self_Hosted_WS.contracts;
using ConsoleApp_for_Self_Hosted_WS.models;
using ConsoleApp_for_Self_Hosted_WS.services.impl;
using ConsoleApp_for_Self_Hosted_WS.services.interfaces;

namespace ConsoleApp_for_Self_Hosted_WS
{
    class ItineraryService : IItineraryService
    {
        private readonly IStepService _stepService = new StepService();
        private readonly IStationService _stationService= new StationService();
        public ItineraryService() { }

        public List<Step> GetItinerary(ItineraryRequest request)
        {
            var origin = new MLocation(new Coordinates(request.Origin.Latitude, request.Origin.Longitude));
            var destination = new MLocation(new Coordinates(request.Destination.Latitude, request.Destination.Longitude));
            (Station originStation, Station destinationStation) = _stationService.GetClosestStations(origin, destination);
            List<Step> cycle_steps = _stepService.GetCyclingSteps(origin.Coordinates, originStation.Coordinates, destinationStation.Coordinates, destination.Coordinates);
            List<Step> walking_steps = _stepService.GetWalkingSteps(origin.Coordinates, destination.Coordinates);
            return cycle_steps[0].Distance > walking_steps[0].Distance ? cycle_steps : walking_steps;
        }
    }

}
