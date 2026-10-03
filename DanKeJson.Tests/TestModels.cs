using System;
using System.Collections.Generic;

namespace DanKeJson.Tests
{
    /// <summary>
    /// 测试用实体类集合。命名一律带 Model 后缀，避免与库中类型重名。
    /// </summary>

    /// <summary>公共字段风格（文档推荐，无需 { get; set; }）。</summary>
    public class UserModel
    {
        public string name;
        public int age;
        public bool isVip;
    }

    /// <summary>公共属性风格。</summary>
    public class UserPropsModel
    {
        public string name { get; set; }
        public int age { get; set; }
        public bool isVip { get; set; }
    }

    /// <summary>字段 + 属性混合。</summary>
    public class MixedModel
    {
        public string fieldStyle;
        public string PropStyle { get; set; }
    }

    /// <summary>自定义键名映射（字段与属性各一）。</summary>
    public class MappedModel
    {
        [JsonProperty("user_name")]
        public string UserName;

        [JsonProperty("user_age")]
        public int UserAge { get; set; }
    }

    /// <summary>同一 JSON 键同时对应一个字段和一个属性。</summary>
    public class DuplicateMappingModel
    {
        [JsonProperty("a")]
        public string FieldA;

        public string a { get; set; }
    }

    public class ItemModel
    {
        public string name;
        public double price;
    }

    /// <summary>嵌套对象 + List + 数组。</summary>
    public class OrderModel
    {
        public int id;
        public List<ItemModel> items;
        public string[] tags;
    }

    public enum ColorModel
    {
        Red = 0,
        Green = 1,
        Blue = 2
    }

    public struct PointModel
    {
        public int x;
        public int y;
    }

    public class StructHolderModel
    {
        public PointModel p;
    }

    /// <summary>反序列化支持的全部成员类型。</summary>
    public class AllTypesModel
    {
        public string s;
        public bool b;
        public byte by;
        public char c;
        public sbyte sb;
        public short sh;
        public ushort us;
        public int i;
        public uint ui;
        public long l;
        public ulong ul;
        public float f;
        public double d;
        public decimal dec;
        public DateTime dt;
        public DateTimeOffset dto;
        public TimeSpan ts;
        public Guid guid;
        public ColorModel color;
        public JsonData raw;
    }

    /// <summary>可空值类型 / byte / char / 字典成员。</summary>
    public class ExtendedTypesModel
    {
        public int? nullableInt;
        public bool? nullableBool;
        public byte by;
        public char c;
        public Dictionary<string, int> map;
        public Dictionary<string, ItemModel> items;
        public Dictionary<int, string> byKey;
    }

    public class SkipMembersModel
    {
        public readonly string locked = "default";
        public static string shared;
        public string open;

        public string GetOnly { get; } = "init";
        public string GetSet { get; set; } = "init";
    }

    public class OverflowModel
    {
        public int age;
        public long big;
    }

    public class NodeModel
    {
        public string name;
        public NodeModel child;
    }

    public class SelfRefModel
    {
        public string name;
        public SelfRefModel self;
    }

    public class SerializeMembersModel
    {
        public string PublicField = "p";
        public string GetOnly { get; } = "init";
        public string GetSet { get; set; } = "init";
        public static string StaticField = "s";
        internal string InternalField = "i";
    }

    /// <summary>用于检查转义输出的模型。</summary>
    public class EscapingModel
    {
        public string text { get; set; }
    }

    /// <summary>带公共索引器的实体类：索引器不应参与序列化与反序列化。</summary>
    public class IndexerModel
    {
        private readonly string[] _items = { "a", "b" };

        public string Name = "n";

        public string this[int index]
        {
            get { return _items[index]; }
            set { _items[index] = value; }
        }
    }

    /// <summary>JsonData 与普通实体的集合成员。</summary>
    public class JsonDataListModel
    {
        public List<JsonData> items;
        public List<ItemModel> typed;
    }

    /// <summary>深层链式结构，用于嵌套深度上限测试。</summary>
    public class ChainModel
    {
        public int v;
        public ChainModel next;
    }
}
