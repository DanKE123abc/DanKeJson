using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace DanKeJson.Tests
{
    /// <summary>JSON.ToJson(object) / JSON.ToJson(JsonData) —— 序列化。</summary>
    public class JsonSerializeTests
    {
        [Fact]
        public void ToJson_FieldStyleEntity_UsesMemberNames()
        {
            var user = new UserModel { name = "DanKe", age = 25, isVip = true };

            Assert.Equal("{\"name\":\"DanKe\",\"age\":25,\"isVip\":true}", JSON.ToJson(user));
        }

        [Fact]
        public void ToJson_PropertyStyleEntity_UsesMemberNames()
        {
            var user = new UserPropsModel { name = "DanKe", age = 25, isVip = true };

            Assert.Equal("{\"name\":\"DanKe\",\"age\":25,\"isVip\":true}", JSON.ToJson(user));
        }

        [Fact]
        public void ToJson_JsonProperty_OverridesName()
        {
            var model = new MappedModel { UserName = "DanKe", UserAge = 25 };

            Assert.Equal("{\"user_name\":\"DanKe\",\"user_age\":25}", JSON.ToJson(model));
        }

        [Fact]
        public void ToJson_NestedObjectAndCollections()
        {
            var order = new OrderModel
            {
                id = 1,
                items = new List<ItemModel>
                {
                    new ItemModel { name = "apple", price = 1.5 },
                    new ItemModel { name = "banana", price = 2.0 }
                },
                tags = new[] { "a", "b" }
            };

            Assert.Equal(
                "{\"id\":1,\"items\":[{\"name\":\"apple\",\"price\":1.5},{\"name\":\"banana\",\"price\":2}],\"tags\":[\"a\",\"b\"]}",
                JSON.ToJson(order));
        }

        [Fact]
        public void ToJson_NullMember_BecomesNull()
        {
            var node = new NodeModel { name = "root" };

            Assert.Equal("{\"name\":\"root\",\"child\":null}", JSON.ToJson(node));
        }

        [Fact]
        public void ToJson_EscapesStrings()
        {
            var model = new EscapingModel { text = "a\"b\\c\nd\te\bf\fg\u0001" };

            Assert.Equal("{\"text\":\"a\\\"b\\\\c\\nd\\te\\bf\\fg\\u0001\"}", JSON.ToJson(model));
        }

        [Fact]
        public void ToJson_StringEndingWithBackslashOrQuote()
        {
            Assert.Equal("{\"text\":\"a\\\\\"}", JSON.ToJson(new EscapingModel { text = "a\\" }));
            Assert.Equal("{\"text\":\"a\\\"\"}", JSON.ToJson(new EscapingModel { text = "a\"" }));
        }

        [Fact]
        public void ToJson_NullObjectOrJsonData_ReturnsNull()
        {
            Assert.Null(JSON.ToJson((object)null));
            Assert.Null(JSON.ToJson((JsonData)null));
        }

        [Fact]
        public void ToJson_Primitives()
        {
            Assert.Equal("1", JSON.ToJson(1));
            Assert.Equal("1.5", JSON.ToJson(1.5));
            Assert.Equal("true", JSON.ToJson(true));
            Assert.Equal("\"text\"", JSON.ToJson("text"));
        }

        [Fact]
        public void ToJson_ListsAndArrays()
        {
            Assert.Equal("[1,2]", JSON.ToJson(new List<int> { 1, 2 }));
            Assert.Equal("[\"a\",\"b\"]", JSON.ToJson(new[] { "a", "b" }));
            Assert.Equal("[1,null,2]", JSON.ToJson(new List<int?> { 1, null, 2 }));
        }

        [Fact]
        public void ToJson_Dictionary()
        {
            var dict = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };

            Assert.Equal("{\"a\":1,\"b\":2}", JSON.ToJson(dict));
        }

        [Fact]
        public void ToJson_EnumMember_IsNumber()
        {
            Assert.Equal("{\"color\":2}", JSON.ToJson(new { color = ColorModel.Blue }));
        }

        [Fact]
        public void ToJson_TopLevelEnum_IsNumber()
        {
            Assert.Equal("2", JSON.ToJson(ColorModel.Blue));
            Assert.Equal("0", JSON.ToJson(ColorModel.Red));
        }

        [Fact]
        public void ToJson_Char_IsString()
        {
            Assert.Equal("\"a\"", JSON.ToJson('a'));
            Assert.Equal("{\"c\":\"x\"}", JSON.ToJson(new { c = 'x' }));
        }

        [Fact]
        public void RoundTrip_EnumAndCharAndByte()
        {
            var model = new AllTypesModel { color = ColorModel.Blue, c = 'x', by = 200 };

            AllTypesModel restored = JSON.ToData<AllTypesModel>(JSON.ToJson(model));

            Assert.Equal(ColorModel.Blue, restored.color);
            Assert.Equal('x', restored.c);
            Assert.Equal((byte)200, restored.by);
        }

        [Fact]
        public void ToJson_DateTimeTypes_UseRoundTripFormat()
        {
            Assert.Equal("\"2026-08-04T12:00:00.0000000Z\"",
                JSON.ToJson(new DateTime(2026, 8, 4, 12, 0, 0, DateTimeKind.Utc)));
            Assert.Equal("\"2026-08-04T12:00:00.0000000+08:00\"",
                JSON.ToJson(new DateTimeOffset(2026, 8, 4, 12, 0, 0, TimeSpan.FromHours(8))));
            Assert.Equal("\"01:30:00\"", JSON.ToJson(TimeSpan.FromMinutes(90)));
            Assert.Equal("\"11111111-2222-3333-4444-555555555555\"",
                JSON.ToJson(Guid.Parse("11111111-2222-3333-4444-555555555555")));
        }

        [Fact]
        public void ToJson_IncludesPublicFieldsAndReadableProperties()
        {
            var model = new SerializeMembersModel();

            string json = JSON.ToJson(model);

            Assert.Contains("\"PublicField\":\"p\"", json);
            Assert.Contains("\"GetOnly\":\"init\"", json);
            Assert.Contains("\"GetSet\":\"init\"", json);
            Assert.DoesNotContain("StaticField", json);
            Assert.DoesNotContain("InternalField", json);
        }

        [Fact]
        public void ToJson_AnonymousType()
        {
            Assert.Equal("{\"x\":1,\"y\":\"s\"}", JSON.ToJson(new { x = 1, y = "s" }));
        }

        [Fact]
        public void ToJson_CircularReference_Throws()
        {
            var model = new SelfRefModel { name = "x" };
            model.self = model;

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => JSON.ToJson(model));
            Assert.Contains("circular reference", ex.Message);
        }

        [Fact]
        public void ToJson_SharedReferenceIsNotACycle()
        {
            var shared = new ItemModel { name = "shared" };
            var order = new OrderModel { id = 1, items = new List<ItemModel> { shared, shared } };

            Assert.Equal("{\"id\":1,\"items\":[{\"name\":\"shared\",\"price\":0},{\"name\":\"shared\",\"price\":0}],\"tags\":null}",
                JSON.ToJson(order));
        }

        [Fact]
        public void ToJson_JsonDataMember_IsEmittedVerbatim()
        {
            var model = new AllTypesModel { raw = JSON.ToData("{\"x\":[1,2]}") };

            Assert.Contains("\"raw\":{\"x\":[1,2]}", JSON.ToJson(model));
        }

        [Fact]
        public void ToJson_JsonData_AllTypes()
        {
            Assert.Equal("{\"a\":1}", JSON.ToJson(JSON.ToData("{\"a\":1}")));
            Assert.Equal("[1,2]", JSON.ToJson(JSON.ToData("[1,2]")));
            Assert.Equal("null", JSON.ToJson(JSON.ToData("null")));
            Assert.Equal("12", JSON.ToJson(JSON.ToData("12")));
            Assert.Equal("true", JSON.ToJson(JSON.ToData("true")));
            Assert.Equal("\"a\\\"b\"", JSON.ToJson(JSON.ToData("\"a\\\"b\"")));
        }

        [Fact]
        public void ToJson_IsCultureInvariant()
        {
            CultureInfo original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");

                Assert.Equal("{\"d\":1.5}", JSON.ToJson(new { d = 1.5 }));
                Assert.Equal("1.5", JSON.ToJson(1.5));
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        [Fact]
        public void RoundTrip_EntityToJsonToEntity()
        {
            var original = new OrderModel
            {
                id = 7,
                items = new List<ItemModel> { new ItemModel { name = "x", price = 0.25 } },
                tags = new[] { "t1" }
            };

            OrderModel restored = JSON.ToData<OrderModel>(JSON.ToJson(original));

            Assert.Equal(original.id, restored.id);
            Assert.Equal(original.items[0].name, restored.items[0].name);
            Assert.Equal(original.items[0].price, restored.items[0].price, 10);
            Assert.Equal(original.tags, restored.tags);
        }
    }
}
