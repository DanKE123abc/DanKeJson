using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DanKeJson.Tests
{
    /// <summary>
    /// 嵌套层级上限：深层输入以前会让进程因 StackOverflowException 直接退出，
    /// 现在统一抛出可捕获的 <see cref="JsonDepthLimitException"/>。
    /// </summary>
    public class DepthLimitTests
    {
        private static string NestedArray(int depth)
        {
            return new string('[', depth) + "1" + new string(']', depth);
        }

        [Fact]
        public void Parse_WithinLimit_Works()
        {
            JsonData data = JSON.ToData(NestedArray(JsonSettings.DefaultMaxDepth));

            Assert.NotNull(data);
            Assert.Equal(JsonData.Type.Array, data.type);
        }

        [Fact]
        public void Parse_OverLimit_Throws()
        {
            Assert.Throws<JsonDepthLimitException>(() => JSON.ToData(NestedArray(JsonSettings.DefaultMaxDepth + 1)));
            Assert.Throws<JsonDepthLimitException>(() => JSON.ToData(NestedArray(5000)));
        }

        [Fact]
        public void Json5Parse_OverLimit_Throws()
        {
            Assert.Throws<JsonDepthLimitException>(() => JSON5.ToData(NestedArray(5000)));
        }

        [Fact]
        public void Parse_DeepObjects_Throws()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 5000; i++)
            {
                sb.Append("{\"a\":");
            }

            sb.Append("1");
            for (int i = 0; i < 5000; i++)
            {
                sb.Append('}');
            }

            Assert.Throws<JsonDepthLimitException>(() => JSON.ToData(sb.ToString()));
        }

        [Fact]
        public void Serialize_DeepObjectGraph_Throws()
        {
            var root = new ChainModel { v = 0 };
            ChainModel current = root;
            for (int i = 1; i < 5000; i++)
            {
                current.next = new ChainModel { v = i };
                current = current.next;
            }

            Assert.Throws<JsonDepthLimitException>(() => JSON.ToJson(root));
        }

        [Fact]
        public void Deserialize_DeepObjectGraph_Throws()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 5000; i++)
            {
                sb.Append("{\"v\":1,\"next\":");
            }

            sb.Append("null");
            for (int i = 0; i < 5000; i++)
            {
                sb.Append('}');
            }

            Assert.Throws<JsonDepthLimitException>(() => JSON.ToData<ChainModel>(sb.ToString()));
        }

        /// <summary>自引用列表以前会无限递归到栈溢出。</summary>
        [Fact]
        public void Serialize_SelfReferencingList_Throws()
        {
            var list = new List<object>();
            list.Add(list);

            Assert.Throws<JsonDepthLimitException>(() => JSON.ToJson(list));
        }

        [Fact]
        public void Guard_ResetsAfterFailure()
        {
            Assert.Throws<JsonDepthLimitException>(() => JSON.ToData(NestedArray(5000)));

            Assert.Equal(1, (int)JSON.ToData("{\"a\":1}")["a"]);
        }

        /// <summary>手工构造的深层 JsonData（绕过解析器）同样受深度上限保护。</summary>
        [Fact]
        public void Deserialize_DeepHandBuiltJsonData_Throws()
        {
            JsonData node = new JsonData(JsonData.Type.Number) { json = "1" };
            for (int i = 0; i < 2000; i++)
            {
                JsonData wrapper = new JsonData(JsonData.Type.Object);
                wrapper["a"] = node;
                node = wrapper;
            }

            JsonData root = new JsonData(JsonData.Type.Object);
            root["anything"] = node;

            Assert.Throws<JsonDepthLimitException>(
                () => DanKeJson.Json.Deserializer.FromJson(root, typeof(DynamicModel)));
        }

        [Fact]
        public void MaxDepth_IsConfigurable()
        {
            int original = JsonSettings.MaxDepth;
            try
            {
                JsonSettings.MaxDepth = 10;

                Assert.NotNull(JSON.ToData(NestedArray(10)));
                Assert.Throws<JsonDepthLimitException>(() => JSON.ToData(NestedArray(11)));
            }
            finally
            {
                JsonSettings.MaxDepth = original;
            }

            Assert.NotNull(JSON.ToData(NestedArray(JsonSettings.DefaultMaxDepth)));
        }
    }
}
