using System.Diagnostics;
using DanKeJson;

Person person;
string jsonText = @"{
                      ""name"": null
                  }";
person = JSON.ToData<Person>(jsonText);
JsonData json = JSON.ToData(jsonText);

Console.WriteLine((string)json["name"]);
Console.WriteLine(person.name);

public class Person
{
    public string name { get; set; }
}