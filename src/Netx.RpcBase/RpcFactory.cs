using System;

namespace Netx.RpcBase
{
    /// <summary>
    /// RPC工厂
    /// </summary>
    public class RpcFactory
    {
        /// <summary>
        /// 创建RPC管道实例
        /// </summary>
        /// <typeparam name="IService"></typeparam>
        /// <param name="channelType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static RpcChannel CreateRpcFactory<IService>(Type channelType, OptionModel model)
            where IService : class
        {
            if (null == channelType)
                throw new ArgumentNullException($"{nameof(channelType)} can not be null");
            if (channelType.IsAssignableFrom(typeof(RpcChannel)))
                throw new ArgumentException($"{nameof(channelType)} has to be a subclass of {nameof(RpcChannel)} ");
            return (RpcChannel)Activator.CreateInstance(channelType, new object[] { model });
        }
    }
}
