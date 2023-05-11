using Netx.RestfulRpc;
using Netx.RpcBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FwDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var channle = RpcFactory.CreateRpcFactory<IFoo>(typeof(RestfulChannel), new RestfulOptionModel());
            var ifoo = channle.CreateRpcChannel<IFoo>(new RestfulInterceptorHandler());
            var result = ifoo.Foo("zeke");
            var result1 = ifoo.Gets(new Persion() { Id = "3" });

            Console.WriteLine("Hello, World!");
            Console.ReadLine();
        }
    }

    public interface IFoo
    {
        void Test();
        string Foo(string message);
        List<Persion> Gets(Persion model);
    }

    public class Persion
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
