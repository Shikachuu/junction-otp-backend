using System;
using System.Text.Json;

namespace GoogleMapsApi
{
    public class Atm
    {
        
        public string AtmPosition { get; set; }

        public string StreetName { get; set; }

        
        public double ExpectedWaitTimeInMinutes { get; set; }
    }


    public class Route
    {
        public string Polyline { get; set; }
        public double TravelTime { get; set; }

        public string UserInstructionsForRoute { get; set; }


        public static Route ParseFromJson(JsonElement routeJsonObject)
        {
            Route route = new Route();
            route.Polyline = routeJsonObject.GetProperty("overview_polyline").GetProperty("points").GetString();
            route.TravelTime = routeJsonObject.GetProperty("legs")[0].GetProperty("duration").GetProperty("value").GetDouble() / 60.0;
            return route;
        }
    }

    public class RouteWithAtm
    {
        public Atm atm { get; set; }
        public Route routeFromDepartureToAtm { get; set; }
        public Route routeFromAtmToDestination { get; set; }
        public double totalTravelTime { get; set; }
    }
}
