using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleApp_for_Self_Hosted_WS.models;
using Microsoft.Extensions.Logging;
using ConsoleApp_for_Self_Hosted_WS.services.interfaces;

namespace ConsoleApp_for_Self_Hosted_WS.services.impl
{
    public class StepService : IStepService
    {
        private readonly ILogger<StepService> _logger;
        public StepService()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("ConsoleApp_for_Self_Hosted_WS", LogLevel.Debug)
                    .AddConsole();
            });
            _logger = loggerFactory.CreateLogger<StepService>();

            _logger.LogInformation("Logger initialized for StationService.");
        }

        public List<Step> GetCyclingSteps(Coordinates originCoordinates, Coordinates originStationCoordinates,
            Coordinates destinationStationCoordinates, Coordinates destinationCoordinates)
        {
            _logger.LogInformation("Getting steps");
            List<Step> walkingToFirstStation =
                GetStepsBetweenTwoCoordinates(originCoordinates, originStationCoordinates, "foot-walking").Result;
            List<Step> bikingBetweenStations =
                GetStepsBetweenTwoCoordinates(originStationCoordinates, destinationStationCoordinates, "cycling-regular")
                    .Result;
            List<Step> walkingToDestination =
                GetStepsBetweenTwoCoordinates(destinationStationCoordinates, destinationCoordinates, "foot-walking").Result;

            List<Step> allSteps = new List<Step>();
            allSteps.AddRange(walkingToFirstStation);
            allSteps.AddRange(bikingBetweenStations);
            allSteps.AddRange(walkingToDestination);
            _logger.LogInformation("Steps retrieved successfully");
            return allSteps;
        }

        public List<Step> GetWalkingSteps(Coordinates originCoordinates, Coordinates destinationCoordinates)
        {
            List<Step> walkingSteps = GetStepsBetweenTwoCoordinates(originCoordinates, destinationCoordinates, "foot-walking").Result;
            return walkingSteps;
        }

        private async Task<List<Step>> GetStepsBetweenTwoCoordinates(Coordinates originCoordinates,
            Coordinates destinationCoordinates, string commuteType)
        {
            string url = "https://api.openrouteservice.org/v2/directions/" + commuteType;
            string api_key = Environment.GetEnvironmentVariable("OPENROUTESERVICE_API_KEY");
            if (api_key == null)
            {
                throw new Exception("OPENROUTESERVICE_API_KEY is not set");
            }

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("BBike/1.0");
            client.DefaultRequestHeaders.Add("Authorization", api_key);
            JsonObject requestBody = new JsonObject
            {
                ["coordinates"] = new JsonArray
                {
                    new JsonArray { originCoordinates.Longitude, originCoordinates.Latitude },
                    new JsonArray { destinationCoordinates.Longitude, destinationCoordinates.Latitude }
                }
            };
            var requestContent = new StringContent(requestBody.ToString(), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, requestContent);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var OSR = JsonSerializer.Deserialize<OSR>(content);
                if (OSR == null)
                {
                    throw new Exception("Failed to get routes");
                }

                if (OSR.Routes[0].Segments == null || OSR.Routes[0].Segments.Count == 0)
                {
                    throw new Exception("No segments found");
                }

                if (OSR.Routes[0].Segments[0].Steps == null || OSR.Routes[0].Segments[0].Steps.Count == 0)
                {
                    throw new Exception("No steps found");
                }

                List<Step> steps = new List<Step>();
                foreach (var segment in OSR.Routes[0].Segments)
                {
                    steps.AddRange(segment.Steps);
                }

                return steps;
            }
            else
            {
                throw new Exception("Failed to get steps");
            }
        }
    }
}
