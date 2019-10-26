using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;

namespace GoogleMapsApi
{

    public class Route
    {
        public string Polyline;
        public static Route ParseFromJson(JsonElement routeJsonObject)
        {
            Route route = new Route();
            route.Polyline = routeJsonObject.GetProperty("overview_polyline").GetProperty("points").GetString();
            
            return route;
        }
    }
    public class GoogleApiFetcher
    {


        /*
         * Travel Modes

        When you calculate directions, you may specify the transportation mode to use. By default, directions are calculated as driving directions. The following travel modes are supported:

        driving (default) indicates standard driving directions using the road network.
        walking requests walking directions via pedestrian paths & sidewalks (where available).
        bicycling requests bicycling directions via bicycle paths & preferred streets (where available).
        transit requests directions via public transit routes (where available). If you set the mode to transit, you can optionally specify either a departure_time or an arrival_time. If neither time is specified, the departure_time defaults to now (that is, the departure time defaults to the current time). You can also optionally include a transit_mode and/or a transit_routing_preference.

         */


        readonly string API_KEY = "AIzaSyDf0hWOKngCVCBWZZ0OohTGIproR_VsLYc";
        readonly string GoogleApiBaseUrl = "https://maps.googleapis.com/maps/api/directions/json";
        readonly string AtmDatabaseUrl = "";


        public static string FetchWebContent(string url)
        {

            // Create a request for the URL. 		
            WebRequest request = WebRequest.Create(url);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // Display the status.
            Console.WriteLine(response.StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.

            // Cleanup the streams and the response.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

        public string FetchRouteAsJson(string origin, string destination, string travelMode, DateTimeOffset departureTime)
        {
            long departureTimeAsLong = departureTime.ToUnixTimeSeconds();
            string departureTimeAsString = departureTimeAsLong.ToString();
            string url = $"{GoogleApiBaseUrl}?origin={HttpUtility.UrlEncode(origin)}&destination={HttpUtility.UrlEncode(destination)}&mode={travelMode}&departure_time={departureTimeAsString}&key={API_KEY}";


            string responseFromServer = FetchWebContent(url);

            return responseFromServer;
        }

        public string FetchAtmsForRouteAsJson(Route route)
        {
            string url = $"{AtmDatabaseUrl}?route={HttpUtility.UrlEncode(route.Polyline)}"; 
            string responseFromServer = FetchWebContent(url);

            return responseFromServer;
        }

        public string FetchBestRouteRoutes(string origin, string destination, string travelMode, DateTimeOffset departureTime)
        {
            return "";
        }


        public IEnumerable<Route> DemoFetch()
        {

            //double geo_x = 47.483543, geo_y = 19.065962;

            // departure_time — Specifies the desired time of departure. 
            // You can specify the time as an integer in seconds since midnight, January 1, 1970 UTC.
            DateTimeOffset now = DateTimeOffset.Now;
            
            string origin = "Magyar Telekom Székház, Budapest, Könyves Kálmán krt. 36, 1097";
            string destination = "Westend, Budapest, Váci út 1-3, 1062";
            string travelMode = "transit";

            //FetchRoutes(origin, destination, travelMode, now);

            string resultJsonString = FetchRouteAsJson(origin, destination, travelMode, now);


            Console.WriteLine(resultJsonString);

            byte[] resultJsonData = Encoding.UTF8.GetBytes(resultJsonString);
            var reader = new Utf8JsonReader(resultJsonData);

            var document = System.Text.Json.JsonDocument.Parse(resultJsonData);
            var routesArray = document.RootElement.GetProperty("routes");

            var routes = System.Linq.Enumerable.Select(routesArray.EnumerateArray(), jsonObject => Route.ParseFromJson(jsonObject));
            



            return routes;
        }
    }
}
