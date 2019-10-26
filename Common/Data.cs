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

        public string UserInstructionsForRoute { get; set; }


        public static Route ParseFromJson(JsonElement routeJsonObject)
        {
            Route route = new Route();
            route.Polyline = routeJsonObject.GetProperty("overview_polyline").GetProperty("points").GetString();

            return route;
        }
    }

    public class RouteWithAtm
    {
        public Atm atm { get; set; }
        public Route routeFromDepartureToAtm { get; set; }
        public Route routeFromAtmToDestination { get; set; }
    }
}
