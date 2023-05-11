namespace Netx.RpcBase
{
    public interface InterceptorHandler
    {
        object InvokeMember(object sender, int methodId, string name, params object[] args);
    }
}
