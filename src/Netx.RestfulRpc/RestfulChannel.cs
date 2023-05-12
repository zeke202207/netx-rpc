using Netx.RpcBase;
using Netx.RpcBase.Models;

namespace Netx.RestfulRpc
{
    public class RestfulChannel : RpcChannel<RestfulConfigModel>
    {
        public RestfulChannel(RestfulRpcModel model) 
            : base(model)
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //释放httpclient
        }
    }
}
