using System;
using System.Collections.Generic;
using System.Text;

namespace Netx.RestfulRpc
{
    /// <summary>
    /// 模板实体
    /// </summary>
    internal class TemplateModel
    {
        private string template = string.Empty;

        public TemplateModel(string temp) 
        {
             template = temp;
        }

        /// <summary>
        /// 起始位置
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// 截止位置
        /// </summary>
        public int EndIndex { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content
        {
            get
            {
                return template.Substring(StartIndex, EndIndex - StartIndex + 1);
            }
        }
    }
}
