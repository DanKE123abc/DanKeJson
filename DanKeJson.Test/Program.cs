using DanKeJson;

string json = "{\"Name\":\"John\",\"Age\":30,\"Height\":1.8,\"HasFriends\":true,\"Friends\":[\"Mary\",25,true]}";
Console.WriteLine(json);
Person test = JSON.ToData<Person>(json);
Console.WriteLine(test.Friends[2]);
Console.WriteLine(JSON.ToJson(test));


public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public float Height { get; set; }
    public bool HasFriends { get; set; }
    public List <object> Friends { get; set; }
}

