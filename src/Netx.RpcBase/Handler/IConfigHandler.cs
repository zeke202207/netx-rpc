using System;
using System.Collections.Generic;
using System.Text;

namespace Netx.RpcBase
{
    public interface IConfigHandler<out T>
        where T : ConfigModel
    {
        T Config();
    }
}
