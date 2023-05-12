using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Netx.RestfulRpc
{
    internal class RestfulDI
    {
        private static Lazy<RestfulDI> instance = new Lazy<RestfulDI>(() => new RestfulDI());
        private readonly ServiceProvider _serviceProvider;

        private RestfulDI()
        {
            var services = Init();
            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// 获取DI容器实例
        /// </summary>
        public static RestfulDI Instance
        {
            get
            {
                return instance.Value;
            }
        }

        /// <summary>
        /// 获取HttpClient实例
        /// </summary>
        public HttpClient HttpClient
        {
            get
            {
                return _serviceProvider.GetRequiredService<IRestfulHttpClient>().HttpClient;
            }
        }

        /// <summary>
        /// 获取注入的解析策略
        /// </summary>
        /// <param name="rType"></param>
        /// <returns></returns>
        public IStrategy ResolveStrategy(ResponseType rType)
        {
            var func = _serviceProvider.GetRequiredService<Func<ResponseType, IStrategy>>();
            return func.Invoke(rType);
        }

        /// <summary>
        /// 初始化ServiceCollection
        /// 注入httpclient
        /// </summary>
        private ServiceCollection Init()
        {
            var services = new ServiceCollection();
            services.AddHttpClient<IRestfulHttpClient, RestfulHttpClient>()
                    .ConfigurePrimaryHttpMessageHandler(() => 
                    new HttpClientHandler 
                    { 
                        UseDefaultCredentials = true,
                    });
            //策略注入
            services.AddSingleton<JsonResolveStragegy>();
            services.AddSingleton(provider =>
            {
                Func<ResponseType, IStrategy> func = t =>
                {
                    switch(t)
                    {
                        case ResponseType.Json:
                            return provider.GetService<JsonResolveStragegy>();
                        default:
                            throw new NotImplementedException("暂不支持其他解析方式");
                    }
                };
                return func;
            });
            return services;
        }
    }
}
