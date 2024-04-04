using System.Diagnostics;
using DanKeJson;

Person persondata;
string jsonText = @"{
  ""id"": 1,
  ""name"": ""John Doe"",
  ""email"": ""john.doe@example.com"",
  ""address"": {
    ""street"": ""123 Main St"",
    ""city"": ""Anytown"",
    ""state"": ""CA"",
    ""postalCode"": 12346
  },
  ""phoneNumbers"": [
    {
      ""type"": ""home"",
      ""number"": ""555-1234""
    },
    {
      ""type"": ""office"",
      ""number"": ""555-5678""
    },
    {
      ""type"": null,
      ""number"": ""555-5678""
    }
  ],
  ""role"": ""developer"",
  ""skills"": [
    ""C#"",
    ""JavaScript"",
    ""SQL""
  ],
  ""active"": true
}";


persondata = JSON.ToData<Person>(jsonText);

JsonData jsondata = JSON.ToData(jsonText);
string json = JSON.ToJson(jsondata);
Console.WriteLine("");

public class Person
{
  public int id { get; set; }
  public string name { get; set; }
  public string email { get; set; }
  public Address address { get; set; }
  public List<PhoneNumber> phoneNumbers { get; set; }
  public string role { get; set; }
  public List<string> skills { get; set; }
  public bool active { get; set; }
}

public class Address
{
  public string street { get; set; }
  public string city { get; set; }
  public string state { get; set; }
  public int postalCode { get; set; }
}

public class PhoneNumber
{
  public string type { get; set; }
  public string number { get; set; }
}
