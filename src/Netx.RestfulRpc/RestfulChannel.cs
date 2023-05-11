using Netx.RpcBase;

namespace Netx.RestfulRpc
{
    public class RestfulChannel : RpcChannel
    {
        public RestfulChannel(OptionModel model) : base(model)
        {

        }

        protected override bool ConfigChannel(OptionModel model)
        {
            return true;
        }
    }
}
