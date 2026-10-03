using Xunit;

namespace DanKeJson.Tests
{
    /// <summary>JSON.ToData(string) —— 文本到 JsonData 的解析行为。</summary>
    public class JsonParseTests
    {
        [Fact]
        public void ToData_NullText_ReturnsNull()
        {
            Assert.Null(JSON.ToData((string)null));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t\r\n")]
        public void ToData_EmptyOrWhitespace_ReturnsNull(string text)
        {
            Assert.Null(JSON.ToData(text));
        }

        [Theory]
        [InlineData("123", "123")]
        [InlineData("-12.5", "-12.5")]
        [InlineData("0", "0")]
        [InlineData("-0", "-0")]
        [InlineData("01", "01")]
        [InlineData("1e3", "1e3")]
        [InlineData("1.5e-3", "1.5e-3")]
        [InlineData("1E+3", "1E+3")]
        [InlineData("+5", "5")]
        public void ToData_Number_KeepsLiteral(string text, string expectedJson)
        {
            JsonData data = JSON.ToData(text);

            Assert.NotNull(data);
            Assert.Equal(JsonData.Type.Number, data.type);
            Assert.Equal(expectedJson, data.json);
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        public void ToData_Boolean(string text, bool expected)
        {
            JsonData data = JSON.ToData(text);

            Assert.Equal(JsonData.Type.Boolean, data.type);
            Assert.Equal(expected, (bool)data);
        }

        [Fact]
        public void ToData_NullLiteral_IsNoneType()
        {
            JsonData data = JSON.ToData("null");

            Assert.NotNull(data);
            Assert.Equal(JsonData.Type.None, data.type);
            Assert.Equal("null", data.json);
        }

        [Fact]
        public void ToData_String_UnescapesCommonSequences()
        {
            JsonData data = JSON.ToData("\"a\\nb\\tc\\\"d\\\\e\\/f\\bg\\fh\"");

            Assert.Equal(JsonData.Type.String, data.type);
            Assert.Equal("a\nb\tc\"d\\e/f\bg\fh", (string)data);
        }

        [Fact]
        public void ToData_String_UnescapesUnicode()
        {
            Assert.Equal("中文", (string)JSON.ToData("\"\\u4e2d\\u6587\""));
        }

        [Fact]
        public void ToData_String_UnescapesSurrogatePair()
        {
            Assert.Equal("😀", (string)JSON.ToData("\"\\ud83d\\ude00\""));
        }

        [Fact]
        public void ToData_String_UnknownEscapeIsKeptVerbatim()
        {
            Assert.Equal("a\\qb", (string)JSON.ToData("\"a\\qb\""));
        }

        [Fact]
        public void ToData_EmptyObjectAndArray()
        {
            JsonData obj = JSON.ToData("{}");
            JsonData array = JSON.ToData("[]");

            Assert.Equal(JsonData.Type.Object, obj.type);
            Assert.Empty(obj.map);
            Assert.Equal(JsonData.Type.Array, array.type);
            Assert.Empty(array.array);
        }

        [Fact]
        public void ToData_AllowsTrailingCommas()
        {
            JsonData obj = JSON.ToData("{\"a\":1,}");
            JsonData array = JSON.ToData("[1,]");

            Assert.True(obj.HasKey("a"));
            Assert.Single(array.array);
            Assert.Equal(1, (int)array[0]);
        }

        [Fact]
        public void ToData_HandlesWhitespaceEverywhere()
        {
            JsonData data = JSON.ToData("  { \"a\" : [ 1 , 2 ] }  ");

            Assert.NotNull(data);
            Assert.Equal("[1,2]", data["a"].json);
        }

        [Fact]
        public void ToData_NestedStructures()
        {
            JsonData data = JSON.ToData("{\"a\":{\"b\":[1,{\"c\":null}]}}");

            Assert.Equal(JsonData.Type.None, data["a"]["b"][1]["c"].type);
            Assert.Equal("{\"a\":{\"b\":[1,{\"c\":null}]}}", JSON.ToJson(data));
        }

        [Theory]
        [InlineData("{\"a\"}")]
        [InlineData("{\"a\":}")]
        [InlineData("{\"a\" 1}")]
        [InlineData("{\"a\":1,\"a\":2}")]
        [InlineData("{a:1}")]
        [InlineData("{'a':1}")]
        [InlineData("[,1]")]
        [InlineData("[1 2]")]
        [InlineData("[1,2")]
        [InlineData("{\"a\":1")]
        [InlineData("\"abc")]
        [InlineData("{")]
        [InlineData("[")]
        [InlineData("tru")]
        [InlineData("nul")]
        [InlineData("abc")]
        [InlineData("{} x")]
        [InlineData("{} {}")]
        [InlineData("1.")]
        [InlineData(".5")]
        [InlineData("-")]
        public void ToData_Malformed_ReturnsNull(string text)
        {
            Assert.Null(JSON.ToData(text));
        }

        [Fact]
        public void ToData_EscapedCharactersInKey()
        {
            JsonData quoted = JSON.ToData("{\"a\\\"b\":1}");
            JsonData unicode = JSON.ToData("{\"\\u4e2d\":1}");
            JsonData backslash = JSON.ToData("{\"a\\\\b\":1}");
            JsonData plain = JSON.ToData("{\"a\":2}");

            Assert.NotNull(quoted);
            Assert.True(quoted.HasKey("a\"b"));
            Assert.Equal(1, (int)quoted["a\"b"]);
            Assert.True(unicode.HasKey("中"));
            Assert.Equal(1, (int)unicode["中"]);
            Assert.True(backslash.HasKey("a\\b"));
            Assert.Equal(2, (int)plain["a"]);
        }

        [Fact]
        public void ToData_LeadingBom_IsIgnored()
        {
            JsonData data = JSON.ToData("\uFEFF{\"a\":1}");

            Assert.NotNull(data);
            Assert.Equal(1, (int)data["a"]);
        }

        /// <summary>无法识别的字面量会被当作 null 吸收，而不是让整段解析失败。</summary>
        [Fact]
        public void ToData_UnrecognizedLiteral_BecomesNoneNode()
        {
            JsonData inArray = JSON.ToData("[NaN]");
            JsonData inObject = JSON.ToData("{\"a\":undefined}");

            Assert.Equal(JsonData.Type.Array, inArray.type);
            Assert.Single(inArray.array);
            Assert.Equal(JsonData.Type.None, inArray[0].type);

            Assert.Equal(JsonData.Type.None, inObject["a"].type);
            Assert.Equal("[null]", inArray.json);
        }

        [Fact]
        public void ToData_EmptyKeyIsAccepted()
        {
            JsonData data = JSON.ToData("{\"\":1}");

            Assert.NotNull(data);
            Assert.True(data.HasKey(""));
        }

        [Fact]
        public void ToData_DeepNesting()
        {
            Assert.Equal("[[[[[1]]]]]", JSON.ToData("[[[[[1]]]]]").json);
        }

        [Fact]
        public void ToData_JsonDataJson_KeepsCompactForm()
        {
            JsonData data = JSON.ToData("{\n  \"a\": 1,\n  \"b\": [1, 2]\n}");

            Assert.Equal("{\"a\":1,\"b\":[1,2]}", data.json);
        }

        [Fact]
        public void ToJson_OfParsedJsonData_ReturnsCompactJson()
        {
            JsonData data = JSON.ToData("{\"a\":\"x\\ny\",\"b\":[true,false,null]}");

            Assert.Equal("{\"a\":\"x\\ny\",\"b\":[true,false,null]}", JSON.ToJson(data));
        }
    }
}
