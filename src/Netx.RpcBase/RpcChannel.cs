using System;

namespace Netx.RpcBase
{
    public abstract class RpcChannel
    {
        private readonly OptionModel _option;

        public RpcChannel(OptionModel model)
        {
            _option = model;
        }

        protected abstract bool ConfigChannel(OptionModel model);

        /// <summary>
        /// 创建RPC管道
        /// </summary>
        /// <returns></returns>
        public IService CreateRpcChannel<IService>(InterceptorHandler handler)
            where IService : class
        {
            if (!ConfigChannel(_option))
                throw new InvalidOperationException($"config channel error.{nameof(CreateRpcChannel)}");
            return DnamicInterfaceProxy.Instance.Resolve<IService>(handler);
        }
    }
}
