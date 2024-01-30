using System.Diagnostics;
using DanKeJson;

Person persondata;
string jsonText = @"{
                      ""name"": ""DanKe"",
                      ""age"": 16,
                      ""num"": [1,2,3,4,5,6]
                  }";


persondata = JSON.ToData<Person>(jsonText);

JsonData jsondata = JSON.ToData(jsonText);

Console.WriteLine("");

public class Person
{
    public string name { get; set; }
    public int age { get; set; }
    public List<int> num { get; set; }
}