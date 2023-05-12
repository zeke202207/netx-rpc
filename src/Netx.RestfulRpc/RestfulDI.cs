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
            return services;
        }
    }
}
