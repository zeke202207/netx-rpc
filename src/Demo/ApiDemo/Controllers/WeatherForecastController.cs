using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet(Name = "Foo1")]
        public string Foo1(string message, string message1)
        {
            Console.WriteLine(DateTime.Now);
            return $"Foo1:{message}-{message1}";
        }

        [HttpPost(Name = "Foo")]
        public string Foo(string message)
        {
            return $"Foo:{message}";
        }

        [HttpPost(Name = "Bar")]
        public IEnumerable<Persion> Bar()
        {
            return Enumerable.Range(1, 5).Select(index => new Persion
            {
                Id = index.ToString(),
                Name = $"zeke-{index}"
            })
           .ToArray();
        }

        [HttpPost(Name = "Bar1")]
        public IEnumerable<Persion> Bar1(Persion persion)
        {
            return new List<Persion> { persion };
        }

        //[HttpPost(Name = "Bar2")]
        //public IEnumerable<Persion> Bar2(Persion persion, Persion persion1)
        //{
        //    return new List<Persion> { persion };
        //}
    }

    public class Persion
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}