using GoogleMapsApi;
using System;

namespace GoogleMapsApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            GoogleApiFetcher googleApiFetcher = new GoogleApiFetcher();
            googleApiFetcher.DemoFetch();

            //var result = googleApiFetcher.DemoFetch();

        }
    }
}
