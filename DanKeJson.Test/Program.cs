using DanKeJson;

string json = "{ \"Name\" : \"Ming\"," +
              " \"Age\" : 16," +
              " \"Height\" : 1.7," +
              " \"HasFriends\" : true," +
              " \"Friends\" : [\"Hong\",\"Long\",\"Cong\"]" +
              "}";
Console.WriteLine(json);
Person test = JSON.ToData<Person>(json);
Console.WriteLine(test.Friends[2]);


public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public float Height { get; set; }
    public bool HasFriends { get; set; }
    public List <string> Friends { get; set; }
}

