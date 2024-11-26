using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleApp_for_Self_Hosted_WS.models;
using ConsoleApp_for_Self_Hosted_WS.services.interfaces;
using Microsoft.Extensions.Logging;

namespace ConsoleApp_for_Self_Hosted_WS.services.impl
{
    public class StationService: IStationService
    {
        private readonly ILogger<StationService> _logger;
        private string apiKey = Environment.GetEnvironmentVariable("JCDECEAUX_API_KEY");

        public StationService()
        {
            // Manually create a logger factory and logger
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("ConsoleApp_for_Self_Hosted_WS", LogLevel.Debug)
                    .AddConsole();
            });
            _logger = loggerFactory.CreateLogger<StationService>();

            _logger.LogInformation("Logger initialized for StationService.");
        }

        public (Station originStation, Station destinationStation) GetClosestStations(MLocation origin,
            MLocation destination)
        {
            _logger.LogInformation("Converting coordinates to city names");
            string originCity = GetCity(origin.Coordinates).Result;
            string destinationCity = GetCity(destination.Coordinates).Result;
            _logger.LogInformation("Conversion successful");
            _logger.LogInformation("Getting contract names");
            string originContractName = GetContractName(originCity).Result;
            string destinationContractName = GetContractName(destinationCity).Result;
            _logger.LogInformation("Contracts retrieved successfully " + originContractName + " " +
                                   destinationContractName);
            _logger.LogInformation("Getting stations");
            List<Station> originStations = GetStations(originContractName).Result;
            List<Station> destinationStations = GetStations(destinationContractName).Result;
            _logger.LogInformation("Finding closest stations with available bikes");
            Station originStation = GetClosestStationWithAvailableBikes(origin.Coordinates, originStations);
            _logger.LogInformation("Origin station coordinates: " + originStation.Coordinates.Latitude + ", " +
                                   originStation.Coordinates.Longitude);
            Station destinationStation =
                GetClosestStationWithAvailableBikes(destination.Coordinates, destinationStations);
            _logger.LogInformation("Destination station coordinates: " + destinationStation.Coordinates.Latitude + ", " +
                                   destinationStation.Coordinates.Longitude);
            _logger.LogInformation("Closest stations found successfully");
            return (originStation, destinationStation);
        }

        private async Task<string> GetCity(Coordinates coordinates)
        {
            string url =
                $"https://nominatim.openstreetmap.org/reverse?lat={coordinates.Latitude}&lon={coordinates.Longitude}&format=json";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("BBike/1.0");

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                JsonObject json = JsonNode.Parse(content).AsObject();

                if (json["address"] != null && json["address"]["city"] != null)
                {
                    string city = json["address"]["city"].ToString();
                    return city;
                }
                else
                {
                    _logger.LogWarning("City not found in the response.");
                    return "Unknown City";
                }
            }

            _logger.LogError("Failed to get city from coordinates.");
            throw new Exception("Failed to get city");
        }

        private async Task<string> GetContractName(string cityName)
        {
            string url = $"https://api.jcdecaux.com/vls/v1/contracts?apiKey={apiKey}";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("BBike/1.0");
            var response = await client.GetAsync(url);
            var contracts =
                JsonSerializer.Deserialize<List<JCDecauxContract>>(await response.Content.ReadAsStringAsync());
            if (contracts == null)
            {
                _logger.LogError("Failed to get contracts from JCDecaux API");
                throw new Exception("Failed to get contracts");
            }

            foreach (var contract in contracts)
            {
                if (contract.Cities != null && contract.Cities.Contains(cityName))
                {
                    if (contract.Name == null)
                    {
                        _logger.LogError("Contract name is null");
                        throw new Exception("Contract name is null");
                    }

                    return contract.Name;
                }
            }

            throw new Exception("Failed to get contract name");
        }

        private async Task<List<Station>> GetStations(string contractName)
        {
            string url = $"https://api.jcdecaux.com/vls/v1/stations?contract={contractName}&apiKey={apiKey}";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("BBike/1.0");

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var stations = JsonSerializer.Deserialize<List<Station>>(content);
                if (stations == null)
                {
                    _logger.LogError("Failed to get stations from JCDecaux API");
                    throw new Exception("Failed to get stations");
                }

                return stations;
            }

            throw new Exception("Failed to get stations");
        }

        public Station GetClosestStationWithAvailableBikes(Coordinates coordinates, List<Station> stations)
        {
            Station closestStation = null;
            double closestDistance = double.MaxValue;

            foreach (var station in stations)
            {
                if (station.AvailableBikes > 0)
                {
                    double distance = coordinates.GetDistanceTo(station.Coordinates);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestStation = station;
                    }
                }
            }

            if (closestStation == null)
            {
                _logger.LogWarning("No station with available bikes found.");
            }

            return closestStation;
        }
    }
}
