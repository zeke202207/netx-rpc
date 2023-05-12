using System;
using System.Collections.Generic;
using System.Text;

namespace Netx.RpcBase.Models
{
    public abstract class RpcModel<T>
        where T : ConfigModel
    {
        public string Id { get; set; }

        public IConfigHandler<T> Config { get; set; }
    }
}
