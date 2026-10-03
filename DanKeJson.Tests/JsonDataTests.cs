using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace DanKeJson.Tests
{
    public class JsonDataTests
    {
        [Fact]
        public void Ctor_Object_InitializesMap()
        {
            var data = new JsonData(JsonData.Type.Object);

            Assert.Equal(JsonData.Type.Object, data.type);
            Assert.NotNull(data.map);
            Assert.Empty(data.map);
            Assert.Null(data.array);
        }

        [Fact]
        public void Ctor_Array_InitializesArray()
        {
            var data = new JsonData(JsonData.Type.Array);

            Assert.Equal(JsonData.Type.Array, data.type);
            Assert.NotNull(data.array);
            Assert.Empty(data.array);
            Assert.Null(data.map);
        }

        [Fact]
        public void Ctor_None_JsonIsNullLiteral()
        {
            var data = new JsonData(JsonData.Type.None);

            Assert.Equal("null", data.json);
        }

        [Fact]
        public void Json_Getter_RebuildsObjectAndArray()
        {
            var obj = new JsonData(JsonData.Type.Object);
            obj["a"] = (JsonData)1;
            obj["b"] = (JsonData)"x";

            Assert.Equal("{\"a\":1,\"b\":\"x\"}", obj.json);

            var arr = new JsonData(JsonData.Type.Array);
            arr.Add((JsonData)1);
            arr.Add(new JsonData(JsonData.Type.None));

            Assert.Equal("[1,null]", arr.json);
        }

        [Fact]
        public void Json_Getter_EscapesStringsInsideObjects()
        {
            var obj = new JsonData(JsonData.Type.Object);
            obj["text"] = (JsonData)"a\"b\n";

            Assert.Equal("{\"text\":\"a\\\"b\\n\"}", obj.json);
        }

        [Fact]
        public void Json_Setter_StoresRawValue()
        {
            var data = new JsonData(JsonData.Type.Number) { json = "42" };

            Assert.Equal("42", data.json);
        }

        [Fact]
        public void Indexer_Object_ReturnsValueOrNull()
        {
            JsonData data = JSON.ToData("{\"a\":1}");

            Assert.NotNull(data["a"]);
            Assert.Equal(1, (int)data["a"]);
            Assert.Null(data["missing"]);
        }

        [Fact]
        public void Indexer_Object_SetterAddsAndOverwrites()
        {
            var data = new JsonData(JsonData.Type.Object);

            data["k"] = (JsonData)"v";
            Assert.Equal("\"v\"", data["k"].json);

            data["k"] = (JsonData)"w";
            Assert.Equal("\"w\"", data["k"].json);
            Assert.Single(data.map);
        }

        [Fact]
        public void Indexer_Array_BoundsAreGuarded()
        {
            JsonData data = JSON.ToData("[10,20]");

            Assert.Equal(10, (int)data[0]);
            Assert.Equal(20, (int)data[1]);
            Assert.Null(data[2]);
            Assert.Null(data[-1]);
        }

        [Fact]
        public void Indexer_Array_SetterIsNoOpOutOfRange()
        {
            JsonData data = JSON.ToData("[1,2]");

            data[5] = (JsonData)9;
            data[-1] = (JsonData)9;

            Assert.Equal("[1,2]", data.json);
        }

        [Fact]
        public void Indexer_OnWrongType_ReturnsNull()
        {
            JsonData array = JSON.ToData("[1]");
            JsonData number = JSON.ToData("1");

            Assert.Null(array["k"]);
            Assert.Null(number[0]);
        }

        [Fact]
        public void HasKey_OnlyForObjects()
        {
            JsonData obj = JSON.ToData("{\"a\":1}");

            Assert.True(obj.HasKey("a"));
            Assert.False(obj.HasKey("b"));
            Assert.False(JSON.ToData("[1]").HasKey("a"));
            Assert.False(JSON.ToData("1").HasKey("a"));
        }

        [Fact]
        public void Add_AppendsToArrayAndIgnoresOthers()
        {
            JsonData array = new JsonData(JsonData.Type.Array);
            array.Add((JsonData)1);
            array.Add((JsonData)2);
            array.Add(null);

            Assert.Equal(2, array.array.Count);

            var obj = new JsonData(JsonData.Type.Object);
            obj.Add((JsonData)1);

            Assert.Null(obj.array);
        }

        [Fact]
        public void Implicit_String_RoundTrips()
        {
            JsonData data = "DanKe";
            Assert.Equal(JsonData.Type.String, data.type);
            Assert.Equal("\"DanKe\"", data.json);

            string value = data;
            Assert.Equal("DanKe", value);
        }

        [Fact]
        public void Implicit_String_OnWrongType_ReturnsNull()
        {
            string fromNumber = (JsonData)5;
            string fromNull = (JsonData)null;

            Assert.Null(fromNumber);
            Assert.Null(fromNull);
        }

        [Fact]
        public void Implicit_Bool_RoundTrips()
        {
            JsonData data = true;

            Assert.Equal(JsonData.Type.Boolean, data.type);
            Assert.Equal("true", data.json);
            Assert.True((bool)data);
            Assert.False((bool)(JsonData)false);
        }

        [Theory]
        [InlineData(42)]
        [InlineData(-42)]
        [InlineData(0)]
        public void Implicit_Int_RoundTrips(int value)
        {
            JsonData data = value;

            Assert.Equal(JsonData.Type.Number, data.type);
            Assert.Equal(value, (int)data);
        }

        [Fact]
        public void Implicit_AllNumericTypes_RoundTrip()
        {
            Assert.Equal(1L, (long)(JsonData)1L);
            Assert.Equal((sbyte)-2, (sbyte)(JsonData)(sbyte)-2);
            Assert.Equal((short)-3, (short)(JsonData)(short)-3);
            Assert.Equal((uint)4, (uint)(JsonData)(uint)4);
            Assert.Equal((ulong)5, (ulong)(JsonData)(ulong)5);
            Assert.Equal((ushort)6, (ushort)(JsonData)(ushort)6);
            Assert.Equal(1.5f, (float)(JsonData)1.5f);
            Assert.Equal(2.5d, (double)(JsonData)2.5d);
        }

        [Fact]
        public void Implicit_Double_KeepsInvariantFormat()
        {
            JsonData data = 1.5d;

            Assert.Equal("1.5", data.json);
        }

        [Fact]
        public void Implicit_FloatAndDouble_SpecialValues()
        {
            Assert.Equal("NaN", ((JsonData)double.NaN).json);
            Assert.Equal("Infinity", ((JsonData)double.PositiveInfinity).json);
            Assert.Equal("-Infinity", ((JsonData)double.NegativeInfinity).json);

            Assert.True(double.IsNaN((double)(JsonData)double.NaN));
            Assert.True(double.IsPositiveInfinity((double)(JsonData)double.PositiveInfinity));
            Assert.True(double.IsNegativeInfinity((double)(JsonData)double.NegativeInfinity));

            Assert.Equal("NaN", ((JsonData)float.NaN).json);
            Assert.True(float.IsNaN((float)(JsonData)float.NaN));
            Assert.True(float.IsPositiveInfinity((float)(JsonData)float.PositiveInfinity));
        }

        [Fact]
        public void Implicit_NumberOnWrongType_ReturnsDefault()
        {
            Assert.Equal(0, (int)(JsonData)"abc");
            Assert.Equal(0d, (double)(JsonData)null);
            Assert.False((bool)(JsonData)1);
        }

        [Fact]
        public void Implicit_UnparseableNumber_ReturnsDefault()
        {
            var data = new JsonData(JsonData.Type.Number) { json = "not-a-number" };

            Assert.Equal(0, (int)data);
            Assert.Equal(0d, (double)data);
        }

        [Fact]
        public void Implicit_Char_IsString()
        {
            JsonData data = 'a';

            Assert.Equal(JsonData.Type.String, data.type);
            Assert.Equal("\"a\"", data.json);
            Assert.Equal("a", (string)data);
        }

        [Fact]
        public void Implicit_NumericCasts_AreCultureInvariant()
        {
            CultureInfo original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");

                Assert.Equal(1.5d, (double)JSON.ToData("1.5"), 10);
                Assert.Equal(1.5f, (float)JSON.ToData("1.5"));
                Assert.Equal(15, (int)JSON.ToData("15"));
                Assert.Equal(1500L, (long)JSON.ToData("1500"));
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        [Fact]
        public void MapAndArray_ArePubliclyMutable()
        {
            var data = new JsonData(JsonData.Type.Object);
            data.map["k"] = (JsonData)1;

            Assert.Equal("{\"k\":1}", data.json);

            var array = new JsonData(JsonData.Type.Array);
            array.array.Add((JsonData)"x");

            Assert.Equal("[\"x\"]", array.json);
        }
    }
}
