using System;
using System.Collections.Generic;
using System.Text;

namespace Netx.RestfulRpc
{
    /// <summary>
    /// 远端返回结果解析策略
    /// </summary>
    internal interface IStrategy
    {
        /// <summary>
        /// 解析远端结果
        /// </summary>
        /// <param name="responseContentType">返回值类型</param>
        /// <param name="responseContent">返回具体内容</param>
        /// <returns></returns>
        object Resolve(Type responseContentType, string responseContent);
    }
}
