using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleMapsApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace junctionx_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AtmController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;

        public AtmController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<RouteWithAtm> Get(string origin, string destination, string travelMode="transit", bool needsdeposit=false)
        {
            var googleApiFethcer = new GoogleApiFetcher();
            //var routes = googleApiFethcer.DemoFetch();
            // "http://100.98.11.34:5002/atm?origin=47.475828,19.099312&destination=47.510687,19.055810"
            var routes = googleApiFethcer.FetchAllRoutes(origin, destination, travelMode, needsdeposit);
            //var routes = new RouteWithAtm[]
            //{
            //    new RouteWithAtm
            //    {
            //        atm = new Atm
            //        {
            //            AtmPosition = "47.5147282, 19.099011",
            //            ExpectedWaitTimeInMinutes = 3,
            //            StreetName = "Szezám utca",
            //        },
            //        routeFromAtmToDestination = new Route
            //        {
            //            Polyline = "wow`HqbqsB}@u@QQwB_BCLBMhAv@DPFPFPBj@kFnRy@dDEd@wAbFcArDs@~BkAtDY~@m@xBa@pBaAhFCd@yCzLkDbN{EfReD`NeCtJaDrMeBzGe@tBuA|Fg@fB{@pCy@nCeApCs@fBk@zA{F`O]`AkBfF}@~AS@SE[Ec@CoEl@y@LmDvAoF~B_Bn@aAh@kAt@cDtBw@l@]RaCxG}@zCwBhGQh@QTe@TMBq@FQ@MBIBmBEcHIoDDyDIgBC_BEaBAiKOgCHm@Am@Cg@?e@?sCCMIgGYa@Cs@OICsAaA}@m@QOy@m@DOENqBuAyA{@y@e@EAGBcFmDuDiCAMSWYSGE",
            //            UserInstructionsForRoute = "Mennyé előre",
            //        }

            //    }, 
            //    new RouteWithAtm
            //    {
            //        atm = new Atm
            //        {
            //            AtmPosition = "47.2147282, 19.199011",
            //            ExpectedWaitTimeInMinutes = 10,
            //            StreetName = "Kapa utca",
            //        },
            //        routeFromAtmToDestination = new Route
            //        {
            //            Polyline = "wow`HqbqsB}@u@QQwB_BCLBMhAv@DPFPFPBj@kFnRy@dDEd@wAbFcArDs@~BkAtDY~@m@xBa@pBaAhFCd@yCzLkDbN{EfReD`NeCtJaDrMeBzGe@tBuA|Fg@fB{@pCy@nCeApCs@fBk@zA{F`O]`AkBfF}@~AS@SE[Ec@CoEl@y@LmDvAoF~B_Bn@aAh@kAt@cDtBw@l@]RaCxG}@zCwBhGQh@QTe@TMBq@FQ@MBIBmBEcHIoDDyDIgBC_BEaBAiKOgCHm@Am@Cg@?e@?sCCMIgGYa@Cs@OICsAaA}@m@QOy@m@DOENqBuAyA{@y@e@EAGBcFmDuDiCAMSWYSGE",
            //            UserInstructionsForRoute = "Mennyé hátra",
            //        }
            //    }
            //};

            return routes;

            //var atms = Enumerable.Select(routes, route => new Atm
            //{
            //    AtmPosition = route.
            //});

            //return atms;

            //var route = new Atm
            //{
            //    AtmPosition = "todo",
            //    StreetName = "todo",
            //    UserFriendlyRoute = "todo"
            //};
            //
            //
            //return new Atm[] { route };

            //var rng = new Random();
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //})
            //.ToArray();
        }
    }
}
