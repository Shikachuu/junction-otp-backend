using System;

namespace GoogleMapsApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            GoogleApiFetcher googleApiFetcher = new GoogleApiFetcher();
            var result = googleApiFetcher.DemoFetch();

        }
    }
}
