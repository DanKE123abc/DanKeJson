using System;
using System.Collections.Generic;
using DanKeJson;

namespace DanKeJson.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 创建一个对象用于序列化和反序列化
            var obj = new
            {
                StringValue = "Hello, World!",
                BoolValue = true,
                IntValue = 123,
                FloatValue = 45.67f,
                LongValue = 8910111213141516,
                ListValue = new List<int> { 1, 2, 3, 4, 5 }
            };

            // 序列化对象到JSON字符串
            var jsonString = JSON.ToJson(obj);
            Console.WriteLine("Serialized JSON:");
            Console.WriteLine(jsonString);

            // 反序列化JSON字符串到C#对象
            var deserializedObj = JSON.ToData<MyClass>(jsonString);

            // 打印反序列化后的对象
            Console.WriteLine("\nDeserialized Object:");
            Console.WriteLine(deserializedObj.StringValue);
            Console.WriteLine(deserializedObj.BoolValue);
            Console.WriteLine(deserializedObj.IntValue);
            Console.WriteLine(deserializedObj.FloatValue);
            Console.WriteLine(deserializedObj.LongValue);

            // 打印反序列化后的列表
            Console.WriteLine("\nList Value:");
            foreach (var item in deserializedObj.ListValue)
            {
                Console.WriteLine(item);
            }
        }
    }

    // 定义一个C#类，用于序列化和反序列化
    public class MyClass
    {
        public string StringValue { get; set; }
        public bool BoolValue { get; set; }
        public int IntValue { get; set; }
        public float FloatValue { get; set; }
        public long LongValue { get; set; }
        public List<int> ListValue { get; set; }
    }
}
