using System;
using System.Collections.Generic;
using System.Text;

namespace Netx.RestfulRpc
{
    /// <summary>
    /// 请求类型
    /// </summary>
    public enum RequestType
    {
        Get,
        Post,
    }

    /// <summary>
    /// 响应类型
    /// </summary>
    public enum ResponseType
    {
        Json,
        Xml
    }
}
