using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Netx.RestfulRpc
{
    internal interface IRestfulHttpClient
    {
        HttpClient HttpClient { get; }
    }
}
