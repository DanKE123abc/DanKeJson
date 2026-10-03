using System;

namespace DanKeJson
{
    /// <summary>
    /// 解析或序列化时嵌套层级超过 <see cref="JsonSettings.MaxDepth"/> 所抛出的异常。
    /// </summary>
    public class JsonDepthLimitException : Exception
    {
        /// <summary>触发异常时生效的最大层级。</summary>
        public int MaxDepth { get; private set; }

        public JsonDepthLimitException(int maxDepth)
            : base("JSON nesting depth exceeds the limit of " + maxDepth +
                   " levels. Adjust JsonSettings.MaxDepth if deeper documents are expected.")
        {
            MaxDepth = maxDepth;
        }
    }
}
