using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;

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

        [HttpPost(Name = "Bar2")]
        public ConnectResult Bar2(ConnectConfig config)
        {
            return new ConnectResult
            {
                ErrorMsg = "测试错误信息",
                PWD = "测试密码",
                SeviceName = "测试服务名称",
                ServiceUrl = "测试服务URL",
                Success = true,
                User = "测试用户名"
            };
        }
    }

    public class Persion
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ConnectConfig
    {
        private List<string> _arguments = null;

        /// <summary>
        /// 构造
        /// </summary>
        public ConnectConfig()
        {
            _arguments = new List<string>();
        }

        /// <summary>
        /// 自定义参数集
        /// 为扩展使用
        /// </summary>
        public List<string> AdditionalGenericArguments
        {
            get { return _arguments; }
            set { _arguments = value; }
        }
    }

    [DataContract]
    public class ConnectResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [DataMember]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 计算用户名
        /// </summary>
        [DataMember]
        public string User { get; set; }

        /// <summary>
        /// 计算密码
        /// </summary>
        [DataMember]
        public string PWD { get; set; }

        /// <summary>
        /// 返回主节点的URL，为动态创建WCF连接使用
        /// </summary>
        [DataMember]
        public string ServiceUrl { get; set; }

        /// <summary>
        /// 返回主节点的服务名称
        /// </summary>
        [DataMember]
        public string SeviceName { get; set; }
    }
}