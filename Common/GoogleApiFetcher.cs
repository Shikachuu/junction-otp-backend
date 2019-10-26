using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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

    public class RouteWithAtm
    {
        Atm atm;
        Route routeFromDepartureToAtm;
        Route routeFromAtmToDestination;
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

        public string FetchRoutesAsJson(string origin, string destination, string travelMode, DateTimeOffset departureTime)
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


        public IEnumerable<Atm> FetchAtms(string polyline)
        {

            var query = "SELECT ST_Distance(ST_Transform(ST_LineFromEncodedPolyline('wow`HqbqsB}@u@QQwB_BCLBMhAv@DPFPFPBj@kFnRy@dDEd@wAbFcArDs@~BkAtDY~@m@xBa@pBaAhFCd@yCzLkDbN{EfReD`NeCtJaDrMeBzGe@tBuA|Fg@fB{@pCy@nCeApCs@fBk@zA{F`O]`AkBfF}@~AS@SE[Ec@CoEl@y@LmDvAoF~B_Bn@aAh@kAt@cDtBw@l@]RaCxG}@zCwBhGQh@QTe@TMBq@FQ@MBIBmBEcHIoDDyDIgBC_BEaBAiKOgCHm@Am@Cg@?e@?sCCMIgGYa@Cs@OICsAaA}@m@QOy@m@DOENqBuAyA{@y@e@EAGBcFmDuDiCAMSWYSGE'),23700),ST_Transform(ST_SetSRID(ST_MakePoint(19.074642,47.486211), 4326),23700));";


            var connectionString = "Server=100.98.2.250;Port=5432;Database=postgres;User Id=postgres;Password=penisz123;";


            using (var postgresConn = new NpgsqlConnection(connectionString))
            {

                postgresConn.Open();

                NpgsqlCommand command = new NpgsqlCommand(query, postgresConn);
                //var result = cmd.ExecuteScalar().ToString();
                //Console.WriteLine(result);
                command.ExecuteNonQuery();
                using (NpgsqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read())
                    {
                        yield return new Atm
                        {
                            AtmPosition = "dummy1",
                            StreetName = "dummy2",
                            UserFriendlyRoute = "dummy3",
                        };
                    }

                }

            }
            //return "";


            ////using (SqlConnection connection = new SqlConnection("postgresql://postgres:penisz123@100.98.2.250:5432/postgres"))
            //using (SqlConnection connection = new SqlConnection(connectionString))
            ////using (SqlConnection connection = new SqlConnection())
            //{

            //    using (SqlCommand command = new SqlCommand(query, connection))
            //    {
            //        connection.Open();
            //        string result = (string)command.ExecuteScalar();
            //        Console.WriteLine(result);
            //        return result;
            //    }
            //}

            //return null;
        }

        public RouteWithAtm FetchRouteIncludingAtm(string origin, string destination, string travelMode, Atm atm)
        {
            return null;
        }


        public IEnumerable<RouteWithAtm> FindAllRoutes(string origin, string destination, string travelMode)
        {

            DateTimeOffset now = DateTimeOffset.Now;
            string resultJsonString = FetchRoutesAsJson(origin, destination, travelMode, now);

            Console.WriteLine(resultJsonString);

            byte[] resultJsonData = Encoding.UTF8.GetBytes(resultJsonString);
            var reader = new Utf8JsonReader(resultJsonData);

            var document = JsonDocument.Parse(resultJsonData);
            var routesArray = document.RootElement.GetProperty("routes");

            var initialRoutes = Enumerable.Select(routesArray.EnumerateArray(), jsonObject => Route.ParseFromJson(jsonObject));

            var allAtms = Enumerable.Empty<Atm>();
            foreach(var route in initialRoutes)
            {
                var atms = FetchAtms(route.Polyline);
                allAtms = Enumerable.Concat(allAtms, atms);
            }
            // FetchRouteIncludingAtm(string origin, string destination, string travelMode, Atm atm)
            var routes = Enumerable.Select(allAtms, atm => FetchRouteIncludingAtm(origin, destination, travelMode, atm));

            return routes;

        }

        public IEnumerable<Route> DemoFetch()
        {

            //double geo_x = 47.483543, geo_y = 19.065962;

            // departure_time — Specifies the desired time of departure. 
            // You can specify the time as an integer in seconds since midnight, January 1, 1970 UTC.
            
            
            string origin = "Magyar Telekom Székház, Budapest, Könyves Kálmán krt. 36, 1097";
            string destination = "Westend, Budapest, Váci út 1-3, 1062";
            string travelMode = "transit";

            //FetchRoutes(origin, destination, travelMode, now);

            FindAllRoutes(origin, destination, travelMode);





            return routes;
        }
    }
}
