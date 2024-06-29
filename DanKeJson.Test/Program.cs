using System;
using System.Collections.Generic;
using DanKeJson;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public List<string> Hobbies { get; set; }
}

public class JsonTest
{
    public static void Main()
    {
        // 将C#对象转换为JSON字符串
        Person person = new Person
        {
            Name = "张三",
            Age = 30,
            Hobbies = new List<string> { "阅读", "游泳", "旅游" }
        };

        string jsonString = JSON.ToJson(person);
        Console.WriteLine(jsonString);

        // 将JSON字符串转换为C#对象
        string json = "{\"Name\":\"张三\",\"Age\":30,\"Hobbies\":[\"阅读\",\"游泳\",\"旅游\"]}";
        Person newPerson = JSON.ToData<Person>(json);
        Console.WriteLine($"姓名：{newPerson.Name}, 年龄：{newPerson.Age}, 爱好：{string.Join(',', newPerson.Hobbies)}");
    }
}