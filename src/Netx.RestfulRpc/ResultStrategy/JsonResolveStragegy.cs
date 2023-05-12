using System;
using System.Collections.Generic;
using System.Text;

namespace Netx.RestfulRpc
{
    /// <summary>
    /// Json格式结果解析策略
    /// </summary>
    internal class JsonResolveStragegy : BaseStrategy ,IStrategy
    {
        public object Resolve(Type responseContentType, string responseContent)
        {
            if (base.IsFundamental(responseContentType))
            {
                if (responseContentType.IsEnum)
                    return Enum.Parse(responseContentType, responseContent.ToString());
                return Convert.ChangeType(responseContent, responseContentType);
            }
            else
                return Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent?.ToString(), responseContentType);
        }
    }
}
