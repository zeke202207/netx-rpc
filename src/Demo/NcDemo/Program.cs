using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;
using Netx.RestfulRpc;
using Netx.RestfulRpc.Attributes;
using Netx.RpcBase;
using Netx.RpcBase.Models;
using System.Diagnostics;
using System.Reflection;

namespace NcDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //单线程
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 10000; i++)
            {
                TestMedth();
                //Console.WriteLine($"{i}");
            }
            sw.Stop();
            Console.WriteLine($"耗时:{sw.Elapsed.TotalMilliseconds} ms");

            //多线程并发（每次创建新的连接）
            Stopwatch sw1 = new Stopwatch();
            sw1.Start();

            Parallel.For(0, 10000, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, i =>
            {
                TestMedth();
                //Console.WriteLine($"{i}");
            });
            sw1.Stop();
            Console.WriteLine($"耗时:{sw1.Elapsed.TotalMilliseconds} ms");

            //多线程并发（公用一个连接)
            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            TestMedth1();
            sw2.Stop();
            Console.WriteLine($"耗时:{sw2.Elapsed.TotalMilliseconds} ms");

            Console.ReadLine();
        }

        private static void TestMedth1()
        {
            using (var channle = RpcFactory.CreateRpcFactory<IFoo, RestfulConfigModel>(typeof(RestfulChannel), new RestfulRpcModel()
            {
                Id = "0",
                Config = new ConfigHandler()
            }))
            {
                var ifoo = channle.CreateRpcChannel<IFoo>(new RestfulInterceptorHandler());
                Parallel.For(0, 10000, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, i =>
                {
                    var result0 = ifoo.Foo1("hello zeke!", "hello zeke1!");
                    var result = ifoo.Foo("hello zeke!");
                    var result1 = ifoo.Bar(new Persion() { Id = "1", Name = "zeke" });
                });
            }
        }

        private static void TestMedth()
        {
            ////生成测试数据
            //List<Persion> list = new List<Persion>()
            //{
            //    new Persion(){ Id = "1" , Name ="zeke1" },
            //    new Persion(){ Id = "2" , Name ="zeke2" },
            //    new Persion(){ Id = "3" , Name ="zeke3" }
            // };
            //string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(list);

            using (var channle = RpcFactory.CreateRpcFactory<IFoo, RestfulConfigModel>(typeof(RestfulChannel), new RestfulRpcModel()
            {
                Id = "0",
                Config = new ConfigHandler()
            }))
            {
                var ifoo = channle.CreateRpcChannel<IFoo>(new RestfulInterceptorHandler());
                var result0 = ifoo.Foo1("hello zeke!", "hello zeke1!");
                var result = ifoo.Foo("hello zeke!");
                var result1 = ifoo.Bar(new Persion() { Id = "1", Name = "zeke" });
            }
        }
    }

    public interface IFoo
    {
        [RestfulRpc(RequestType.Post, ResponseType.Json, "api/WeatherForecast/Foo")]
        string Foo(string message);


        [RestfulRpc(RequestType.Get, ResponseType.Json, "api/WeatherForecast/Foo1?{message}&{message1}")]
        string Foo1(string message, string message1);

        [RestfulRpc(RequestType.Post, ResponseType.Json, "api/WeatherForecast/Bar")]
        List<Persion> Bar(Persion model);

        void Test();        
        //bool Foo1();
        DateTime FooDatetime();
        bool FooBool();
        TestEnum FooEnum();
        string? FooString();
        
    }

    public enum TestEnum
    {
        A,
        B,
        C
    }

    public class Persion
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ConfigHandler : IConfigHandler<RestfulConfigModel>
    {
        public RestfulConfigModel Config()
        {
            return new RestfulConfigModel() { Id = "1", BaseUrl = "http://localhost:5028" };
        }
    }
}