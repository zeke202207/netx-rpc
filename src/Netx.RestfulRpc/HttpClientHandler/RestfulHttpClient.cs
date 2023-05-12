using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Netx.RestfulRpc
{
    internal class RestfulHttpClient : IRestfulHttpClient
    {
        public RestfulHttpClient(HttpClient httpClient) => HttpClient = httpClient;

        public HttpClient HttpClient { get; }
    }
}
