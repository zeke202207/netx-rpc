using Netx.RestfulRpc;
using Netx.RpcBase;

namespace NcDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var channle = RpcFactory.CreateRpcFactory<IFoo>(typeof(RestfulChannel), new RestfulOptionModel());
            var ifoo = channle.CreateRpcChannel<IFoo>(new RestfulInterceptorHandler());
            ifoo.Test();

            //var result = ifoo.Foo("zeke");

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