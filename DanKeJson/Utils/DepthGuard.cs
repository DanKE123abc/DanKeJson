using System;

namespace DanKeJson.Utils
{
    /// <summary>
    /// DepthGuard : 递归深度计数（按线程隔离）。
    /// 用于把深层嵌套导致的 StackOverflowException（不可捕获、直接终止进程）
    /// 转换成可捕获的 <see cref="JsonDepthLimitException"/>。
    /// </summary>
    internal static class DepthGuard
    {
        [ThreadStatic]
        private static int _depth;

        /// <summary>进入一层递归；超过 <see cref="JsonSettings.MaxDepth"/> 时抛出异常。</summary>
        public static void Enter()
        {
            _depth++;
            int maxDepth = JsonSettings.MaxDepth;
            if (maxDepth > 0 && _depth > maxDepth)
            {
                // 本层不会执行 Exit，这里先复位，避免污染同一线程上的后续调用
                _depth = 0;
                throw new JsonDepthLimitException(maxDepth);
            }
        }

        /// <summary>退出一层递归。</summary>
        public static void Exit()
        {
            if (_depth > 0)
            {
                _depth--;
            }
        }
    }
}
