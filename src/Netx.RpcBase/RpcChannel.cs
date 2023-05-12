using Netx.RpcBase.Models;
using System;

namespace Netx.RpcBase
{
    public abstract class RpcChannel<T> : IDisposable
        where T : ConfigModel
    {
        private readonly RpcModel<T> _option;
        private bool disposedValue;

        public RpcChannel(RpcModel<T> model)
        {
            _option = model;
        }

        /// <summary>
        /// 创建RPC管道
        /// </summary>
        /// <returns></returns>
        public IService CreateRpcChannel<IService>(InterceptorHandler handler)
            where IService : class
        {
            return DnamicInterfaceProxy.Instance.Resolve<IService,T>(handler, _option);
        }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~RpcChannel()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
