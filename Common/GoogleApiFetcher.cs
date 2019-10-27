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


        readonly string API_KEY = "[REDACTED]";
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

            Console.WriteLine(responseFromServer);

            return responseFromServer;
        }

        public string FetchAtmsForRouteAsJson(Route route)
        {
            string url = $"{AtmDatabaseUrl}?route={HttpUtility.UrlEncode(route.Polyline)}"; 
            string responseFromServer = FetchWebContent(url);

            return responseFromServer;
        }

        //public string FetchBestRouteRoutes(string origin, string destination, string travelMode, DateTimeOffset departureTime)
        //{

        //    return "";
        //}


        // TODO szimulálni
        static double GetEstimatedTime(int sorbanAllokSzamaEbbenAFeloraban)
        {
            return 1.414 * sorbanAllokSzamaEbbenAFeloraban;
        }

        public IEnumerable<Atm> FetchAtms(string encodedPolyline, bool needsDeposit)
        {

            var now = DateTime.Now;
            var next = DateTime.Now.AddMinutes(30);

            int hour = now.Hour;
            int minutes = now.Minute < 30 ? 0 : 30;

            int nextHour = next.Hour;
            int nextMinutes = next.Minute < 30 ? 0 : 30;

            string timeFieldName = String.Format("{0}{1}-{2}{3}", hour.ToString("D2"), minutes.ToString("D2"), nextHour.ToString("D2"), nextMinutes.ToString("D2"));

            string depositCondition = needsDeposit ? " and deposit=TRUE ": "";
            var query = $"SELECT deposit, \"{timeFieldName}\", ST_AsText(geo_coord), zip_code, city, street_address, ST_Distance(ST_Transform(ST_LineFromEncodedPolyline('{encodedPolyline}'),23700),ST_Transform(csv.geo_coord,23700)) as distance FROM csv WHERE trans_day='SUNDAY' {depositCondition} ORDER BY distance ASC LIMIT 10;";
            //var query = "SELECT ST_Distance(ST_Transform(ST_LineFromEncodedPolyline('wow`HqbqsB}@u@QQwB_BCLBMhAv@DPFPFPBj@kFnRy@dDEd@wAbFcArDs@~BkAtDY~@m@xBa@pBaAhFCd@yCzLkDbN{EfReD`NeCtJaDrMeBzGe@tBuA|Fg@fB{@pCy@nCeApCs@fBk@zA{F`O]`AkBfF}@~AS@SE[Ec@CoEl@y@LmDvAoF~B_Bn@aAh@kAt@cDtBw@l@]RaCxG}@zCwBhGQh@QTe@TMBq@FQ@MBIBmBEcHIoDDyDIgBC_BEaBAiKOgCHm@Am@Cg@?e@?sCCMIgGYa@Cs@OICsAaA}@m@QOy@m@DOENqBuAyA{@y@e@EAGBcFmDuDiCAMSWYSGE'),23700),ST_Transform(ST_SetSRID(ST_MakePoint(19.074642,47.486211), 4326),23700));";


            var connectionString = "Server=100.98.2.250;Port=5432;Database=postgres;User Id=postgres;Password=penisz123;";


            LinkedList<Atm> list = new LinkedList<Atm>();

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
                        
                        int varakozokSzama = reader.GetInt32(1);
                        string atmGps = reader.GetString(2);
                        string streetAddress = reader.GetString(5);


                        list.AddLast(new Atm()
                        {
                            ExpectedWaitTimeInMinutes = GetEstimatedTime(varakozokSzama),
                            AtmPosition = atmGps,
                            StreetName = streetAddress,
                        });
                        //yield return new Atm
                        //{
                        //    AtmPosition = "dummy1",
                        //    StreetName = "dummy2",
                        //    ExpectedWaitTimeInMinutes = 20,
                        //};
                    }

                }

            }

            return list;
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


        static string DEMO_FORMAT_MERT_NINCS_IDO(string str)
        {
            string pre = "POINT(";
            var t = str.Remove(0, pre.Length).TrimEnd(')').Split(' ');
            return $"{t[1]},{t[0]}";
        }

        public RouteWithAtm FetchRouteIncludingAtm(string origin, string destination, string travelMode, Atm atm)
        {
            DateTimeOffset now = DateTimeOffset.Now;
            var atmFormattedPos = DEMO_FORMAT_MERT_NINCS_IDO(atm.AtmPosition);
            var routesToAtm = FetchRoutes(origin, atmFormattedPos, travelMode, now);

            DateTimeOffset expectedDepartureTimeFromAtm = now.AddMinutes(atm.ExpectedWaitTimeInMinutes);

            var routesFromAtm = FetchRoutes(atmFormattedPos, destination, travelMode, expectedDepartureTimeFromAtm);

            var first = routesToAtm.First() ?? throw new Exception("Penis a kugli pls.");
            var second = routesFromAtm.First() ?? throw new Exception("Penis a kugli pls.");


            var finalRoute = new RouteWithAtm
            {
                atm = atm,
                routeFromDepartureToAtm = first,
                routeFromAtmToDestination = second,
                totalTravelTime = atm.ExpectedWaitTimeInMinutes + first.TravelTime + second.TravelTime,
            };


            return finalRoute;
        }


        public IEnumerable<Route> FetchRoutes(string origin, string destination, string travelMode, DateTimeOffset departureTime)
        {
            string resultJsonString = FetchRoutesAsJson(origin, destination, travelMode, departureTime);


            byte[] resultJsonData = Encoding.UTF8.GetBytes(resultJsonString);
            var reader = new Utf8JsonReader(resultJsonData);

            var document = JsonDocument.Parse(resultJsonData);
            var routesArray = document.RootElement.GetProperty("routes");

            var routes = Enumerable.Select(routesArray.EnumerateArray(), jsonObject => Route.ParseFromJson(jsonObject));

            return routes;
        }

        public IEnumerable<RouteWithAtm> FetchAllRoutes(string origin, string destination, string travelMode, bool needsDeposit)
        {

            DateTimeOffset now = DateTimeOffset.Now;
            //string resultJsonString = FetchRoutesAsJson(origin, destination, travelMode, now);

            //Console.WriteLine(resultJsonString);

            //byte[] resultJsonData = Encoding.UTF8.GetBytes(resultJsonString);
            //var reader = new Utf8JsonReader(resultJsonData);

            //var document = JsonDocument.Parse(resultJsonData);
            //var routesArray = document.RootElement.GetProperty("routes");

            //var initialRoutes = Enumerable.Select(routesArray.EnumerateArray(), jsonObject => Route.ParseFromJson(jsonObject));
            var initialRoutes = FetchRoutes(origin, destination, travelMode, now);

            var allAtms = Enumerable.Empty<Atm>();
            foreach(var route in initialRoutes)
            {
                var atms = FetchAtms(route.Polyline, needsDeposit);
                allAtms = Enumerable.Concat(allAtms, atms);
            }
            // FetchRouteIncludingAtm(string origin, string destination, string travelMode, Atm atm)
            var routes = Enumerable.Select(allAtms, atm => FetchRouteIncludingAtm(origin, destination, travelMode, atm));

            return routes;

        }

        public IEnumerable<RouteWithAtm> DemoFetch()
        {

            //double geo_x = 47.483543, geo_y = 19.065962;

            // departure_time — Specifies the desired time of departure. 
            // You can specify the time as an integer in seconds since midnight, January 1, 1970 UTC.
            
            
            string origin = "Magyar Telekom Székház, Budapest, Könyves Kálmán krt. 36, 1097";
            string destination = "Westend, Budapest, Váci út 1-3, 1062";
            string travelMode = "transit";
            
            //FetchRoutes(origin, destination, travelMode, now);

            var routes = FetchAllRoutes(origin, destination, travelMode, false);

            //public IEnumerable<Atm> FetchAtms(string encodedPolyline);

            


            return routes;
        }
    }
}
