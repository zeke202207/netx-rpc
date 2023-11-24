using Netx.RpcBase.Models;
using System;
using System.Collections.Concurrent;

namespace Netx.RpcBase
{
    /// <summary>
    /// RPC工厂
    /// </summary>
    public class RpcFactory
    {
        private static ConcurrentDictionary<Type,object> _cache = new ConcurrentDictionary<Type, object>();
        private static object _objLock = new object();

        /// <summary>
        /// 创建RPC管道实例
        /// </summary>
        /// <typeparam name="IService"></typeparam>
        /// <param name="channelType"></param>
        /// <returns>通信管道实例对象</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static RpcChannel<T> CreateRpcFactory<IService, T, TChannel>(RpcModel<T> model)
            where T : ConfigModel
            where IService : class
            where TChannel : RpcChannel<T>
        {
            Type channelType = typeof(TChannel);
            _cache.TryGetValue(channelType, out object instance);
            if(instance == null)
            {
                lock(_objLock)
                {
                    _cache.TryGetValue(channelType, out instance);
                    if (null == instance)
                    {
                        instance = Activator.CreateInstance(channelType, new object[] { model });
                        _cache.TryAdd(channelType, instance);
                    }
                }
            }
            return (RpcChannel<T>)instance;
        }
    }
}
