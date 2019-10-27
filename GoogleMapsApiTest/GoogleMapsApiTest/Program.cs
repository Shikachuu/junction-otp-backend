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
            //googleApiFetcher.DemoFetch();
            var encodedPolyline = "wow`HqbqsB}@u@QQwB_BCLBMhAv@DPFPFPBj@kFnRy@dDEd@wAbFcArDs@~BkAtDY~@m@xBa@pBaAhFCd@yCzLkDbN{EfReD`NeCtJaDrMeBzGe@tBuA|Fg@fB{@pCy@nCeApCs@fBk@zA{F`O]`AkBfF}@~AS@SE[Ec@CoEl@y@LmDvAoF~B_Bn@aAh@kAt@cDtBw@l@]RaCxG}@zCwBhGQh@QTe@TMBq@FQ@MBIBmBEcHIoDDyDIgBC_BEaBAiKOgCHm@Am@Cg@?e@?sCCMIgGYa@Cs@OICsAaA}@m@QOy@m@DOENqBuAyA{@y@e@EAGBcFmDuDiCAMSWYSGE";
            var result = googleApiFetcher.FetchAtms(encodedPolyline);

            //var result = googleApiFetcher.DemoFetch();

        }
    }
}
