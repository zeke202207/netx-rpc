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
        public static RpcChannel<T> CreateRpcFactory<IService, T>(Type channelType, RpcModel<T> model)
            where T : ConfigModel
            where IService : class
        {
            if (null == channelType)
                throw new ArgumentNullException($"{nameof(channelType)} can not be null");
            if (channelType.IsAssignableFrom(typeof(RpcChannel<T>)))
                throw new ArgumentException($"{nameof(channelType)} has to be a subclass of {nameof(RpcChannel<T>)} ");
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
