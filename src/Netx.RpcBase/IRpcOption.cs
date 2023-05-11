namespace Netx.RpcBase
{
    public interface IRpcOption<T>
        where T : OptionModel
    {
        /// <summary>
        /// 管道配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Config(T model);
    }
}
