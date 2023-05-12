using System;
using System.Collections.Generic;
using System.Text;

namespace Netx.RestfulRpc
{
    internal class BaseStrategy
    {
        /// <summary>
        /// 判断type是否为基础类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool IsFundamental(Type type)
        {
            return type.IsPrimitive || type.IsEnum || type.Equals(typeof(string)) || type.Equals(typeof(DateTime));
        }
    }
}
