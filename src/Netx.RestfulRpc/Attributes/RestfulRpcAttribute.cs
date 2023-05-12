using Netx.RpcBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netx.RestfulRpc.Attributes
{
    public class RestfulRpcAttribute : RpcAttribute
    {
        /// <summary>
        /// 请求类型
        /// </summary>
        public RequestType RequestType { get; set; }

        /// <summary>
        /// 响应类型
        /// </summary>
        public ResponseType ResponseType { get; set; }

        /// <summary>
        /// api路由
        /// </summary>
        public string ApiRoute { get; set; }

        /// <summary>
        /// api接口特性
        /// </summary>
        /// <param name="request">请求类型</param>
        /// <param name="response">响应类型</param>
        /// <param name="apiRoute">请求api路由</param>
        public RestfulRpcAttribute(RequestType request, ResponseType response, string apiRoute)
        {
            ResponseType = response;
            RequestType = request;
            ApiRoute = apiRoute;
        }
    }
}
