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
        public IEnumerable<Atm> Get()
        {
            var googleApiFethcer = new GoogleApiFetcher();
            var routes = googleApiFethcer.DemoFetch();


            var atms = Enumerable.Select(routes, route => new Atm
            {
                AtmPosition = route.Polyline
            });

            return atms;

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
