namespace Netx.RpcBase
{
    /// <summary>
    /// 拦截器接口
    /// </summary>
    public interface InterceptorHandler
    {
        object InvokeMember(object sender, int methodId, string name, params object[] args);
    }
}
