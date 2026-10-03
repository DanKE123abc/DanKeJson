using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace DanKeJson.Tests
{
    /// <summary>JSON.ToData&lt;T&gt;(string) —— 文本到实体类的反序列化。</summary>
    public class JsonDeserializeTests
    {
        [Fact]
        public void ToData_FieldStyleEntity_IsPopulated()
        {
            UserModel user = JSON.ToData<UserModel>("{\"name\":\"DanKe\",\"age\":25,\"isVip\":true}");

            Assert.Equal("DanKe", user.name);
            Assert.Equal(25, user.age);
            Assert.True(user.isVip);
        }

        [Fact]
        public void ToData_PropertyStyleEntity_IsPopulated()
        {
            UserPropsModel user = JSON.ToData<UserPropsModel>("{\"name\":\"DanKe\",\"age\":25,\"isVip\":true}");

            Assert.Equal("DanKe", user.name);
            Assert.Equal(25, user.age);
            Assert.True(user.isVip);
        }

        [Fact]
        public void ToData_MixedFieldAndProperty_BothPopulated()
        {
            MixedModel model = JSON.ToData<MixedModel>("{\"fieldStyle\":\"f\",\"PropStyle\":\"p\"}");

            Assert.Equal("f", model.fieldStyle);
            Assert.Equal("p", model.PropStyle);
        }

        [Fact]
        public void ToData_JsonProperty_AppliesToFieldsAndProperties()
        {
            MappedModel model = JSON.ToData<MappedModel>("{\"user_name\":\"DanKe\",\"user_age\":25}");

            Assert.Equal("DanKe", model.UserName);
            Assert.Equal(25, model.UserAge);
        }

        [Fact]
        public void ToData_SameKeyForFieldAndProperty_BothAssigned()
        {
            DuplicateMappingModel model = JSON.ToData<DuplicateMappingModel>("{\"a\":\"v\"}");

            Assert.Equal("v", model.FieldA);
            Assert.Equal("v", model.a);
        }

        [Fact]
        public void ToData_NestedObjectAndCollections()
        {
            OrderModel order = JSON.ToData<OrderModel>(
                "{\"id\":1,\"items\":[{\"name\":\"apple\",\"price\":1.5},{\"name\":\"banana\",\"price\":2.0}],\"tags\":[\"a\",\"b\"]}");

            Assert.Equal(1, order.id);
            Assert.Equal(2, order.items.Count);
            Assert.Equal("apple", order.items[0].name);
            Assert.Equal(2.0, order.items[1].price, 10);
            Assert.Equal(new[] { "a", "b" }, order.tags);
        }

        [Fact]
        public void ToData_AllSupportedMemberTypes()
        {
            const string text = "{" +
                                "\"s\":\"str\",\"b\":true,\"by\":7,\"c\":\"x\"," +
                                "\"sb\":-8,\"sh\":-9,\"us\":10,\"i\":11,\"ui\":12,\"l\":13,\"ul\":14," +
                                "\"f\":1.5,\"d\":2.5,\"dec\":3.5," +
                                "\"dt\":\"2026-08-04T10:00:00Z\"," +
                                "\"dto\":\"2026-08-04T10:00:00+08:00\"," +
                                "\"ts\":\"01:30:00\"," +
                                "\"guid\":\"11111111-2222-3333-4444-555555555555\"," +
                                "\"color\":2," +
                                "\"raw\":{\"x\":[1,2]}" +
                                "}";

            AllTypesModel m = JSON.ToData<AllTypesModel>(text);

            Assert.Equal("str", m.s);
            Assert.True(m.b);
            Assert.Equal((byte)7, m.by);
            Assert.Equal('x', m.c);
            Assert.Equal((sbyte)-8, m.sb);
            Assert.Equal((short)-9, m.sh);
            Assert.Equal((ushort)10, m.us);
            Assert.Equal(11, m.i);
            Assert.Equal((uint)12, m.ui);
            Assert.Equal(13L, m.l);
            Assert.Equal((ulong)14, m.ul);
            Assert.Equal(1.5f, m.f);
            Assert.Equal(2.5d, m.d, 10);
            Assert.Equal(3.5m, m.dec);
            Assert.Equal(new DateTime(2026, 8, 4, 10, 0, 0, DateTimeKind.Utc), m.dt);
            Assert.Equal(new DateTimeOffset(2026, 8, 4, 10, 0, 0, TimeSpan.FromHours(8)), m.dto);
            Assert.Equal(TimeSpan.FromMinutes(90), m.ts);
            Assert.Equal(Guid.Parse("11111111-2222-3333-4444-555555555555"), m.guid);
            Assert.Equal(ColorModel.Blue, m.color);
            Assert.Equal(JsonData.Type.Object, m.raw.type);
            Assert.Equal(1, (int)m.raw["x"][0]);
        }

        [Fact]
        public void ToData_MissingKeys_KeepDefaults()
        {
            AllTypesModel m = JSON.ToData<AllTypesModel>("{}");

            Assert.Null(m.s);
            Assert.False(m.b);
            Assert.Equal(0, m.i);
            Assert.Equal(0d, m.d);
            Assert.Equal(default(DateTime), m.dt);
            Assert.Null(m.raw);
        }

        [Fact]
        public void ToData_UnknownKeys_AreIgnored()
        {
            UserModel user = JSON.ToData<UserModel>("{\"nope\":1,\"age\":7,\"nested\":{\"a\":1}}");

            Assert.Equal(7, user.age);
            Assert.Null(user.name);
        }

        [Fact]
        public void ToData_IntOverflow_FallsBackToDefault()
        {
            OverflowModel m = JSON.ToData<OverflowModel>("{\"age\":9999999999,\"big\":9999999999}");

            Assert.Equal(0, m.age);
            Assert.Equal(9999999999L, m.big);
        }

        [Fact]
        public void ToData_TypeMismatch_FallsBackToDefault()
        {
            AllTypesModel m = JSON.ToData<AllTypesModel>("{\"s\":1,\"i\":\"abc\",\"b\":\"not-bool\"}");

            Assert.Null(m.s);
            Assert.Equal(0, m.i);
            Assert.False(m.b);
        }

        [Fact]
        public void ToData_JsonDataMember_KeepsRawNode()
        {
            AllTypesModel fromObject = JSON.ToData<AllTypesModel>("{\"raw\":{\"x\":1}}");
            AllTypesModel fromNumber = JSON.ToData<AllTypesModel>("{\"raw\":12}");

            Assert.Equal(JsonData.Type.Object, fromObject.raw.type);
            Assert.Equal(JsonData.Type.Number, fromNumber.raw.type);
            Assert.Equal("12", fromNumber.raw.json);
        }

        [Fact]
        public void ToData_StructMember_IsPopulated()
        {
            StructHolderModel holder = JSON.ToData<StructHolderModel>("{\"p\":{\"x\":1,\"y\":2}}");

            Assert.Equal(1, holder.p.x);
            Assert.Equal(2, holder.p.y);
        }

        [Fact]
        public void ToData_SkipsReadonlyStaticAndSetterlessMembers()
        {
            SkipMembersModel model = JSON.ToData<SkipMembersModel>(
                "{\"locked\":\"x\",\"shared\":\"y\",\"open\":\"z\",\"GetOnly\":\"g\",\"GetSet\":\"t\"}");

            Assert.Equal("default", model.locked);
            Assert.Null(SkipMembersModel.shared);
            Assert.Equal("z", model.open);
            Assert.Equal("init", model.GetOnly);
            Assert.Equal("t", model.GetSet);
        }

        [Fact]
        public void ToData_EnumMember_FromNumber()
        {
            AllTypesModel m = JSON.ToData<AllTypesModel>("{\"color\":1}");

            Assert.Equal(ColorModel.Green, m.color);
        }

        [Fact]
        public void ToData_EnumMember_FromString()
        {
            AllTypesModel fromName = JSON.ToData<AllTypesModel>("{\"color\":\"Blue\"}");
            AllTypesModel fromLowerCase = JSON.ToData<AllTypesModel>("{\"color\":\"green\"}");
            AllTypesModel fromNumericString = JSON.ToData<AllTypesModel>("{\"color\":\"1\"}");
            AllTypesModel fromUnknownName = JSON.ToData<AllTypesModel>("{\"color\":\"Nope\"}");

            Assert.Equal(ColorModel.Blue, fromName.color);
            Assert.Equal(ColorModel.Green, fromLowerCase.color);
            Assert.Equal(ColorModel.Green, fromNumericString.color);
            Assert.Equal(ColorModel.Red, fromUnknownName.color);
        }

        [Fact]
        public void ToData_NullableValueTypes_ArePopulated()
        {
            ExtendedTypesModel m = JSON.ToData<ExtendedTypesModel>("{\"nullableInt\":7,\"nullableBool\":true}");

            Assert.Equal(7, m.nullableInt);
            Assert.True(m.nullableBool);
        }

        [Fact]
        public void ToData_NullableValueTypes_StayNullForJsonNull()
        {
            ExtendedTypesModel m = JSON.ToData<ExtendedTypesModel>("{\"nullableInt\":null,\"nullableBool\":null}");

            Assert.Null(m.nullableInt);
            Assert.Null(m.nullableBool);
        }

        [Fact]
        public void ToData_ByteAndCharMembers()
        {
            ExtendedTypesModel fromText = JSON.ToData<ExtendedTypesModel>("{\"by\":255,\"c\":\"x\"}");
            ExtendedTypesModel fromCode = JSON.ToData<ExtendedTypesModel>("{\"c\":97}");

            Assert.Equal((byte)255, fromText.by);
            Assert.Equal('x', fromText.c);
            Assert.Equal('a', fromCode.c);
        }

        [Fact]
        public void ToData_DictionaryMembers()
        {
            ExtendedTypesModel m = JSON.ToData<ExtendedTypesModel>(
                "{\"map\":{\"a\":1,\"b\":2}," +
                "\"items\":{\"x\":{\"name\":\"apple\",\"price\":1.5}}," +
                "\"byKey\":{\"7\":\"seven\"}}");

            Assert.Equal(2, m.map.Count);
            Assert.Equal(2, m.map["b"]);
            Assert.Equal("apple", m.items["x"].name);
            Assert.Equal(1.5, m.items["x"].price, 10);
            Assert.Equal("seven", m.byKey[7]);
        }

        [Fact]
        public void ToData_DictionaryMember_JsonNullKeepsNull()
        {
            ExtendedTypesModel m = JSON.ToData<ExtendedTypesModel>("{\"map\":null}");

            Assert.Null(m.map);
        }

        [Fact]
        public void ToData_TopLevelList()
        {
            List<int> numbers = JSON.ToData<List<int>>("[1,2,3]");
            List<ItemModel> items = JSON.ToData<List<ItemModel>>("[{\"name\":\"a\"},{\"name\":\"b\"}]");
            List<string> strings = JSON.ToData<List<string>>("[null,\"a\"]");

            Assert.Equal(new[] { 1, 2, 3 }, numbers);
            Assert.Equal("b", items[1].name);
            Assert.Null(strings[0]);
            Assert.Equal("a", strings[1]);
        }

        [Fact]
        public void ToData_NullJson_ReturnsNull()
        {
            Assert.Null(JSON.ToData<UserModel>("null"));
            Assert.Null(JSON.ToData<UserModel>(null));
        }

        /// <summary>JSON 是标量或数组时，反序列化到实体类会返回成员全为默认值的实例。</summary>
        [Fact]
        public void ToData_ScalarOrArrayJson_ReturnsDefaultInstance()
        {
            UserModel fromScalar = JSON.ToData<UserModel>("5");
            UserModel fromArray = JSON.ToData<UserModel>("[1,2]");

            Assert.NotNull(fromScalar);
            Assert.Null(fromScalar.name);
            Assert.NotNull(fromArray);
            Assert.Equal(0, fromArray.age);
        }

        [Fact]
        public void ToData_ListTargetWithObjectJson_ReturnsEmptyList()
        {
            List<int> list = JSON.ToData<List<int>>("{\"a\":1}");

            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public void ToData_IsCultureInvariant()
        {
            CultureInfo original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");

                AllTypesModel m = JSON.ToData<AllTypesModel>("{\"d\":1.5,\"dec\":2.25,\"dt\":\"2026-08-04T10:00:00Z\"}");

                Assert.Equal(1.5d, m.d, 10);
                Assert.Equal(2.25m, m.dec);
                Assert.Equal(new DateTime(2026, 8, 4, 10, 0, 0, DateTimeKind.Utc), m.dt);
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        [Fact]
        public void ToData_IgnoresItemKeyWhenClassHasIndexer()
        {
            IndexerModel m = JSON.ToData<IndexerModel>("{\"Item\":\"x\",\"Name\":\"y\"}");

            Assert.Equal("y", m.Name);
        }

        [Fact]
        public void ToData_ListOfJsonData_KeepsRawNodes()
        {
            JsonDataListModel m = JSON.ToData<JsonDataListModel>("{\"items\":[{\"a\":1},\"x\",5,null]}");

            Assert.Equal(4, m.items.Count);
            Assert.Equal(JsonData.Type.Object, m.items[0].type);
            Assert.Equal(1, (int)m.items[0]["a"]);
            Assert.Equal("x", (string)m.items[1]);
            Assert.Equal(5, (int)m.items[2]);
            Assert.Equal(JsonData.Type.None, m.items[3].type);
        }

        [Fact]
        public void ToData_TopLevelListOfJsonData()
        {
            List<JsonData> list = JSON.ToData<List<JsonData>>("[{\"a\":1}]");

            Assert.Single(list);
            Assert.Equal(JsonData.Type.Object, list[0].type);
            Assert.Equal(1, (int)list[0]["a"]);
        }

        [Fact]
        public void ToData_TopLevelDictionary()
        {
            Dictionary<string, int> map = JSON.ToData<Dictionary<string, int>>("{\"a\":1,\"b\":2}");

            Assert.Equal(2, map.Count);
            Assert.Equal(2, map["b"]);
        }

        [Fact]
        public void ToData_ListOfDictionary()
        {
            List<Dictionary<string, int>> list = JSON.ToData<List<Dictionary<string, int>>>("[{\"a\":1}]");

            Assert.Single(list);
            Assert.Equal(1, list[0]["a"]);
        }

        [Fact]
        public void ToData_PlainListUnaffected()
        {
            JsonDataListModel m = JSON.ToData<JsonDataListModel>("{\"typed\":[{\"name\":\"a\"}]}");

            Assert.Equal("a", m.typed[0].name);
        }

        [Fact]
        public void ToData_UninstantiableMembers_AreSkipped()
        {
            HostModel m = JSON.ToData<HostModel>(
                "{\"uri\":{\"x\":1},\"thing\":{\"name\":\"a\"},\"version\":{\"major\":1}," +
                "\"versions\":[{\"major\":1}],\"name\":\"ok\"}");

            Assert.Null(m.uri);
            Assert.Null(m.thing);
            Assert.Equal("ok", m.name);
            Assert.NotNull(m.version);
            Assert.Single(m.versions);
        }

        [Fact]
        public void ToData_ObjectMember_MapsJsonTypes()
        {
            Assert.Equal("s", JSON.ToData<DynamicModel>("{\"anything\":\"s\"}").anything);
            Assert.Equal(5L, JSON.ToData<DynamicModel>("{\"anything\":5}").anything);
            Assert.Equal(1.5d, JSON.ToData<DynamicModel>("{\"anything\":1.5}").anything);
            Assert.Equal(true, JSON.ToData<DynamicModel>("{\"anything\":true}").anything);
            Assert.Null(JSON.ToData<DynamicModel>("{\"anything\":null}").anything);

            List<object> list = (List<object>)JSON.ToData<DynamicModel>("{\"anything\":[1,\"a\"]}").anything;
            Assert.Equal(2, list.Count);
            Assert.Equal(1L, list[0]);
            Assert.Equal("a", list[1]);

            Dictionary<string, object> map =
                (Dictionary<string, object>)JSON.ToData<DynamicModel>("{\"anything\":{\"k\":1}}").anything;
            Assert.Equal(1L, map["k"]);
        }

        [Fact]
        public void ToData_InterfaceCollectionMembers()
        {
            InterfaceCollectionsModel m = JSON.ToData<InterfaceCollectionsModel>(
                "{\"numbers\":[1,2],\"items\":[{\"name\":\"a\"}],\"readOnlyNumbers\":[1,2,3]," +
                "\"map\":{\"a\":1},\"readOnlyMap\":{\"b\":2},\"rawList\":[1,\"a\"],\"rawMap\":{\"x\":[1]}}");

            Assert.Equal(new[] { 1, 2 }, m.numbers);
            Assert.Equal("a", m.items[0].name);
            Assert.Equal(3, m.readOnlyNumbers.Count);
            Assert.Equal(1, m.map["a"]);
            Assert.Equal(2, m.readOnlyMap["b"]);
            Assert.Equal(1L, m.rawList[0]);
            Assert.Equal(1L, ((List<object>)m.rawMap["x"])[0]);
        }

        [Fact]
        public void ToData_InterfaceMember_JsonNullStaysNull()
        {
            Assert.Null(JSON.ToData<InterfaceCollectionsModel>("{\"numbers\":null}").numbers);
        }

        [Fact]
        public void ToData_SetterException_IsNotWrapped()
        {
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
                () => JSON.ToData<ThrowingSetterModel>("{\"V\":\"x\"}"));

            Assert.Contains("setter boom", ex.Message);
        }
    }
}
