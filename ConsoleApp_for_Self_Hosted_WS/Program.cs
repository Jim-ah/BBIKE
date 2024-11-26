using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// add the WCF ServiceModel namespace 
using System.ServiceModel;
using System.ServiceModel.Description;
using ConsoleApp_for_Self_Hosted_WS.services.impl;
using ConsoleApp_for_Self_Hosted_WS.services.interfaces;

namespace ConsoleApp_for_Self_Hosted_WS
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a URI to serve as the base address
            //Be careful to run Visual Studio as Admistrator or to allow VS to open new port netsh command. 
            // Example : netsh http add urlacl url=http://+:80/MyUri user=DOMAIN\user
            Uri httpUrl = new Uri("http://localhost:5432/");

            // Create ServiceHost
            ServiceHost host = new ServiceHost(typeof(ItineraryService), httpUrl);

            // Add RESTful endpoint
            var endpoint = host.AddServiceEndpoint(typeof(IItineraryService), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());

            // Enable metadata exchange
            var smb = new ServiceMetadataBehavior { HttpGetEnabled = true };
            host.Description.Behaviors.Add(smb);

            // Open host
            host.Open();
            Console.WriteLine("Service is running at " + httpUrl);
            Console.ReadLine();

        }
    }
}
