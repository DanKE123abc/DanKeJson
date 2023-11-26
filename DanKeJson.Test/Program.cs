using DanKeJson;

Person person = JSON.ToData<Person>(@"{
    ""name"": ""John Doe"",
    ""happy"": false,
    ""age"": 30
}");

public class Person
{
    public string name { get; set; }
    public bool happy { get; set; }
    public int age { get; set; }
}

