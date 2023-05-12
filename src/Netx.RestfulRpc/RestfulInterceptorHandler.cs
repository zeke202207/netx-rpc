using Microsoft.CodeAnalysis.CSharp;
using Netx.RestfulRpc.Attributes;
using Netx.RestfulRpc.Contents;
using Netx.RpcBase;
using Netx.RpcBase.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Netx.RestfulRpc
{
    public class RestfulInterceptorHandler : InterceptorHandler
    {
        public object InvokeMember(object sender, int methodId, string name, params object[] args)
        {
            var sp = name.Split('+');
            var interfaceImplType = DnamicInterfaceProxy.Instance.GetCacheType(sp[0]);
            var type = GetMethodReturnType(interfaceImplType, sp[1]);
            var rpcModel = GetRpcModel(sender, interfaceImplType);
            if (null == rpcModel || null == type)
                return null;
            var config = rpcModel.Config?.Config();            
            var attribute = GetAttribute(interfaceImplType, sp[1]);
            var remoteCallResult = RemoteCall(config, interfaceImplType, attribute, args);
            var resolveStrategy = RestfulDI.Instance.ResolveStrategy(attribute.ResponseType);
            return resolveStrategy.Resolve(type, remoteCallResult);
        }

        /// <summary>
        /// 获取自定义解析器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="interfaceTypeFullName"></param>
        /// <returns></returns>
        private RestfulRpcModel GetRpcModel(object sender , Type interfaceImplType)
        {
            if (null == interfaceImplType)
                return null;
            var interfaceImplField = interfaceImplType.GetField("_model", BindingFlags.NonPublic | BindingFlags.Instance);
            if(null == interfaceImplField)
                return null;
            return interfaceImplField.GetValue(sender) as RestfulRpcModel;
        }

        /// <summary>
        /// 获取代理接口返回值类型
        /// </summary>
        /// <param name="interfaceImplType"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private Type GetMethodReturnType(Type interfaceImplType, string methodName)
        {
            if (null == interfaceImplType)
                return null;
            var method = interfaceImplType.GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            return method.ReturnType;
        }

        /// <summary>
        /// 真正的远程调用逻辑
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private string RemoteCall(RestfulConfigModel config, Type interfaceImplType, RestfulRpcAttribute attribute, params object[] args)
        {
            if(null == attribute)
                return null;
            var client = RestfulDI.Instance.HttpClient;
            client.BaseAddress = new Uri(config.BaseUrl);
            return HttpCall(client, attribute.RequestType, attribute.ApiRoute, args);
        }

        /// <summary>
        /// 获取接口的特性配置
        /// </summary>        /// <param name="interfaceImplType"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private RestfulRpcAttribute GetAttribute(Type interfaceImplType, string methodName)
        {
            if (null == interfaceImplType)
                return null;
            var intefaceType = interfaceImplType.GetInterfaces().FirstOrDefault();
            if(null == intefaceType)
                return null;
            var method = intefaceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (null == method)
                return null;
            return method.GetCustomAttributes<RestfulRpcAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// http调用
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="apiRoute"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private string HttpCall(HttpClient client, RequestType request, string apiRoute, params object[] args)
        {
            int index = 0;
            foreach(var route in ResolveApiRoute(apiRoute))
            {
                apiRoute = apiRoute.Replace(route.Content, $"{route.Content.Trim("{}".ToCharArray())}={args[index++].ToString()}" );
            }
            Task<HttpResponseMessage> response = null;
            switch(request)
            {
                case RequestType.Get:
                    response = client.GetAsync(apiRoute);
                    break;
                default:    
                case RequestType.Post:
                    if (args.Count() > 0)
                        response = client.PostAsync(apiRoute, new JsonContent(args[0].ToString()));
                    else
                        response = client.PostAsync(apiRoute, null);
                    break;
            }
            var result = response.Result;
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return string.Empty;
            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 解析路由模板
        /// </summary>
        /// <param name="apiRoute"></param>
        /// <returns></returns>
        private IEnumerable<TemplateModel> ResolveApiRoute(string apiRoute)
        {
            if(apiRoute.IndexOfAny("{".ToCharArray()) < 0)
                yield break;
            TemplateModel model = null;
            int index = 0;
            foreach (var c in apiRoute.ToCharArray())
            {
                if(c == Convert.ToChar("{"))
                    model = new TemplateModel(apiRoute) { StartIndex = index };
                if (c == Convert.ToChar("}"))
                {
                    model.EndIndex = index;
                    yield return model;
                }
                index++;
            }
        }
    }
}
