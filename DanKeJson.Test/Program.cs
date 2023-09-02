using DanKeJson;

string json = "{\"Name\":\"John\",\"Age\":30,\"Height\":1.8,\"HasFriends\":true ,\"Muq\":{\"Name\":\"MeiMei\"}}";
Console.WriteLine(json);
Person test = JSON.ToData<Person>(json);
Console.WriteLine(test.Name);
MaMa ma = new MaMa();
ma = test.Muq;
Console.WriteLine(ma.Name);


public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public float Height { get; set; }
    public bool HasFriends { get; set; }
    public MaMa Muq { get; set; }
}

public class MaMa
{
    public string Name { get; set; }
}
