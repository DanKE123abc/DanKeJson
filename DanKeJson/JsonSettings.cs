namespace DanKeJson
{
    /// <summary>
    /// JsonSettings : DanKeJson 的全局设置。
    /// </summary>
    public static class JsonSettings
    {
        /// <summary>默认最大嵌套层级。</summary>
        public const int DefaultMaxDepth = 256;

        private static int _maxDepth = DefaultMaxDepth;

        /// <summary>
        /// 解析与序列化允许的最大嵌套层级，默认 256。
        /// 超过该层级时抛出 <see cref="JsonDepthLimitException"/>，把原本不可捕获的
        /// StackOverflowException 变成可以处理的异常。
        /// 设为小于 1 的值表示不限制（不推荐：深层嵌套会直接终止进程）。
        /// </summary>
        public static int MaxDepth
        {
            get { return _maxDepth; }
            set { _maxDepth = value; }
        }
    }
}
