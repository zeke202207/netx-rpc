using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Netx.RestfulRpc.Contents
{
    internal class JsonContent : StringContent
    {
        public JsonContent(string content) :
            base(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
        { }

    }
}
