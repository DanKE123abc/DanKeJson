using DanKeJson;

Person person = new Person
{
    Name = "张三",
    Age = 30,
    Hobbies = new List<string> { "阅读", "旅行", "编程" },
    Address = new Address
    {
        City = "北京",
        District = "朝阳区",
        Street = "某街道"
    },
    addresses = new List<Address>
    {
        new Address
        {
            City = "北京",
            District = "朝阳区",
            Street = "某街道"
        },
        new Address
        {
            City = "上海",
            District = "浦东新区",
            Street = "另一街道"
        },
    },
    test = new List<List<int>>
    {
        new List<int>
            {1,2,3},
        new List<int>
            {4,5,6},
        new List<int>
            {7,8,9},
    }
};

string json = DanKeJson.JSON.ToJson(person);
Person newperson = DanKeJson.JSON.ToData<Person>(json);
string json5 = DanKeJson.JSON5.ToJson(newperson, new Json5Config
{
    AddCommaForObject = false,
    AddCommaForArray = false,
    KeyNameStyle = Json5Config.KeyNameType.WithoutQuotes,
    StringQuoteStyle = Json5Config.StringQuoteType.SingleQuote,
});

Console.WriteLine(json);


public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public List<string> Hobbies { get; set; }
    public Address Address { get; set; }
    
    public List<Address> addresses { get; set; }  
    
    public List<List<int>> test {get; set;}
}

public class Address
{
    public string City { get; set; }
    public string District { get; set; }
    public string Street { get; set; }
}



