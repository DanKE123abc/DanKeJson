using Xunit;

namespace DanKeJson.Tests
{
    /// <summary>JSON5 解析与序列化（注释、单引号、无引号键名、多余逗号、格式选项）。</summary>
    public class Json5Tests
    {
        [Fact]
        public void ToData_UnquotedKeysAndTrailingComma()
        {
            JsonData data = JSON5.ToData("{name:'DanKe',age:25,}");

            Assert.Equal("DanKe", (string)data["name"]);
            Assert.Equal(25, (int)data["age"]);
        }

        [Fact]
        public void ToData_DoubleQuotedKeysAndSingleQuotedValues()
        {
            JsonData data = JSON5.ToData("{\"a\": 'b'}");

            Assert.Equal("b", (string)data["a"]);
        }

        [Fact]
        public void ToData_LineComments()
        {
            JsonData data = JSON5.ToData("// leading\n{a:1, // trailing\n}");

            Assert.Equal(1, (int)data["a"]);
        }

        [Fact]
        public void ToData_BlockComments()
        {
            JsonData data = JSON5.ToData("{/* c1 */a:1/* c2 */,b:2}");

            Assert.Equal(1, (int)data["a"]);
            Assert.Equal(2, (int)data["b"]);
        }

        [Fact]
        public void ToData_LineCommentLikeTextInsideStringIsKept()
        {
            JsonData data = JSON5.ToData("{url:'http://x//y',u2:\"a//b\"}");

            Assert.Equal("http://x//y", (string)data["url"]);
            Assert.Equal("a//b", (string)data["u2"]);
        }

        [Fact]
        public void ToData_BlockCommentLikeTextInsideStringIsKept()
        {
            JsonData doubleQuoted = JSON5.ToData("{u2:\"a/*b*/c\"}");
            JsonData singleQuoted = JSON5.ToData("{u3:'a/*b*/c'}");

            Assert.Equal("a/*b*/c", (string)doubleQuoted["u2"]);
            Assert.Equal("a/*b*/c", (string)singleQuoted["u3"]);
        }

        [Fact]
        public void ToData_LeadingBom_IsIgnored()
        {
            JsonData data = JSON5.ToData("\uFEFF{a:1}");
            UserModel user = JSON5.ToData<UserModel>("\uFEFF{name:'DanKe'}");

            Assert.Equal(1, (int)data["a"]);
            Assert.Equal("DanKe", user.name);
        }

        [Fact]
        public void ToData_EscapedCharactersInQuotedKey()
        {
            Assert.Equal(1, (int)JSON5.ToData("{\"a\\\"b\":1}")["a\"b"]);
            Assert.Equal(3, (int)JSON5.ToData("{'a b':3}")["a b"]);
        }

        [Fact]
        public void ToData_EscapedSingleQuoteAndUnicode()
        {
            Assert.Equal("it's", (string)JSON5.ToData("{a:'it\\'s'}")["a"]);
            Assert.Equal("中文", (string)JSON5.ToData("{a:'\\u4e2d\\u6587'}")["a"]);
        }

        [Fact]
        public void ToData_TopLevelSingleQuotedString()
        {
            Assert.Equal("single", (string)JSON5.ToData("'single'"));
        }

        [Fact]
        public void ToData_PlusSignAndSpecialNumbers()
        {
            Assert.Equal(1, (int)JSON5.ToData("{a:+1}")["a"]);

            double infinity = JSON5.ToData("{a:Infinity}")["a"];
            double negativeInfinity = JSON5.ToData("{a:-Infinity}")["a"];
            double nan = JSON5.ToData("{a:NaN}")["a"];

            Assert.True(double.IsPositiveInfinity(infinity));
            Assert.True(double.IsNegativeInfinity(negativeInfinity));
            Assert.True(double.IsNaN(nan));
        }

        [Fact]
        public void ToData_TrailingCommaInArray()
        {
            JsonData data = JSON5.ToData("[1,2,]");

            Assert.Equal(2, data.array.Count);
        }

        [Fact]
        public void ToData_HexNumbers()
        {
            Assert.Equal(16, (int)JSON5.ToData("{a:0x10}")["a"]);
            Assert.Equal(16, (int)JSON5.ToData("{a:0X10}")["a"]);
            Assert.Equal(-16, (int)JSON5.ToData("{a:-0x10}")["a"]);
            Assert.Equal(255, (int)JSON5.ToData("{a:+0xff}")["a"]);
            Assert.Equal(0, (int)JSON5.ToData("{a:0}")["a"]);
        }

        [Fact]
        public void ToData_HexWithoutDigits_Fails()
        {
            Assert.Null(JSON5.ToData("{a:0x}"));
        }

        [Fact]
        public void ToData_JsonStaysStrictAboutHexAndBomHandlingIsShared()
        {
            Assert.Null(JSON.ToData("0x10"));
        }

        [Fact]
        public void ToData_NullInputs()
        {
            Assert.Null(JSON5.ToData((string)null));
            Assert.Null(JSON5.ToData<UserModel>(null));
        }

        [Fact]
        public void ToDataGeneric_WithCommentsAndUnquotedKeys()
        {
            UserModel user = JSON5.ToData<UserModel>("{ // c\n name: 'DanKe', age: 25, isVip: true, }");

            Assert.Equal("DanKe", user.name);
            Assert.Equal(25, user.age);
            Assert.True(user.isVip);
        }

        [Fact]
        public void ToJson_DefaultOptions_ArePlainJson()
        {
            JsonData data = JSON.ToData("{\"a\":\"x\",\"b\":[1,2]}");

            Assert.Equal("{\"a\":\"x\",\"b\":[1,2]}", JSON5.ToJson(data));
        }

        [Fact]
        public void ToJson_AllOptionsEnabled()
        {
            JsonData data = JSON.ToData("{\"name\":\"DanKe\",\"age\":25,\"a b\":1,\"list\":[1,2]}");
            var options = new Json5Options
            {
                KeyNameStyle = Json5Options.KeyNameType.WithoutQuotes,
                StringQuoteStyle = Json5Options.StringQuoteType.SingleQuote,
                AddTailingCommaForObject = true,
                AddTailingCommaForArray = true
            };

            Assert.Equal("{name:'DanKe',age:25,\"a b\":1,list:[1,2,],}", JSON5.ToJson(data, options));
        }

        [Fact]
        public void ToJson_NonIdentifierKeysKeepQuotes()
        {
            JsonData data = JSON.ToData("{\"a-b\":1,\"a_b\":2,\"中文\":3}");
            var options = new Json5Options { KeyNameStyle = Json5Options.KeyNameType.WithoutQuotes };

            Assert.Equal("{\"a-b\":1,a_b:2,中文:3}", JSON5.ToJson(data, options));
        }

        [Fact]
        public void ToJson_SingleQuoteEscapesInnerQuotes()
        {
            var options = new Json5Options { StringQuoteStyle = Json5Options.StringQuoteType.SingleQuote };
            JsonData data = JSON.ToData("{\"a\":\"it's\"}");

            // 键名引号由 KeyNameStyle 决定，这里只切换字符串引号
            Assert.Equal("{\"a\":'it\\'s'}", JSON5.ToJson(data, options));
        }

        [Fact]
        public void ToJson_EmptyContainersNeverGetTrailingComma()
        {
            var options = new Json5Options { AddTailingCommaForObject = true, AddTailingCommaForArray = true };

            Assert.Equal("{}", JSON5.ToJson(JSON.ToData("{}"), options));
            Assert.Equal("[]", JSON5.ToJson(JSON.ToData("[]"), options));
        }

        [Fact]
        public void ToJson_NullInputs_ReturnNull()
        {
            Assert.Null(JSON5.ToJson((JsonData)null));
            Assert.Null(JSON5.ToJson((object)null));
        }

        [Fact]
        public void ToJson_Object_RoundTripsThroughJson5()
        {
            var user = new UserModel { name = "DanKe", age = 25, isVip = true };
            var options = new Json5Options
            {
                KeyNameStyle = Json5Options.KeyNameType.WithoutQuotes,
                StringQuoteStyle = Json5Options.StringQuoteType.SingleQuote
            };

            string json5 = JSON5.ToJson(user, options);

            Assert.Equal("{name:'DanKe',age:25,isVip:true}", json5);
            UserModel restored = JSON5.ToData<UserModel>(json5);
            Assert.Equal("DanKe", restored.name);
            Assert.True(restored.isVip);
        }

        [Fact]
        public void ToData_Json5Escapes()
        {
            Assert.Equal("A", (string)JSON5.ToData("{a:'\\x41'}")["a"]);
            Assert.Equal("A", (string)JSON5.ToData("{a:\"\\x41\"}")["a"]);
            Assert.Equal('\v', ((string)JSON5.ToData("{a:'\\v'}")["a"])[0]);
            Assert.Equal('\0', ((string)JSON5.ToData("{a:'\\0'}")["a"])[0]);
            Assert.Equal(3, ((string)JSON5.ToData("{a:'\\01'}")["a"]).Length);
            Assert.Equal("it's", (string)JSON5.ToData("{a:\"it\\'s\"}")["a"]);
        }

        [Fact]
        public void ToData_StringContinuation()
        {
            Assert.Equal("line1line2", (string)JSON5.ToData("{a:'line1\\\nline2'}")["a"]);
            Assert.Equal("line1line2", (string)JSON5.ToData("{a:'line1\\\r\nline2'}")["a"]);
        }

        [Fact]
        public void ToData_EscapesInQuotedKeys()
        {
            Assert.Equal(1, (int)JSON5.ToData("{'\\x41':1}")["A"]);
        }

        [Fact]
        public void Json_KeepsUnknownEscapesVerbatim()
        {
            Assert.Equal("a\\x41b", (string)JSON.ToData("\"a\\x41b\""));
            Assert.Equal("a\\\nb", (string)JSON.ToData("\"a\\\nb\""));
        }

        [Fact]
        public void ToData_UnquotedKeyWithDollar()
        {
            var withoutQuotes = new Json5Options { KeyNameStyle = Json5Options.KeyNameType.WithoutQuotes };

            Assert.Equal(1, (int)JSON5.ToData("{$a:1}")["$a"]);
            Assert.Equal("{$a:1}", JSON5.ToJson(JSON.ToData("{\"$a\":1}"), withoutQuotes));
            Assert.Equal("{\"a-b\":1}", JSON5.ToJson(JSON.ToData("{\"a-b\":1}"), withoutQuotes));
        }
    }
}
