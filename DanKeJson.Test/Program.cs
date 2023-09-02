using System.Diagnostics;
using DanKeJson;
using LitJson;

int count = 10000;
Stopwatch watch;

watch = new Stopwatch();

string json1 = "{\"id\":10001,\"name\":\"test\"}";
string json2 = "[1,2,3,4,5,6,7,8,9,10]";
string json3 = "{\"id\":10000,\"username\":\"zhangyu\",\"password\":\"123456\",\"nickname\":\"冰封百度\",\"age\":20,\"gender\":1,\"phone\":12345678910,\"email\":\"zhangyu@xx.com\"}";
string json4 = "[\"test2\",[[\"key1\",    \"id\"],[\"key2\",    \"hp\"],[\"key3\",    \"mp\"],[\"key4\",    \"exp\"],[\"key5\",    \"money\"],[\"key6\",    \"point\"],[\"key7\",    \"age\"],[\"key8\",    \"sex\"]]]";

Console.WriteLine("——————————————Test1——————————————");
Console.WriteLine(json1);
DanKeJsonTest(json1);
LitJsonTest(json1);
Console.WriteLine("——————————————Test2——————————————");
Console.WriteLine(json2);
DanKeJsonTest(json2);
LitJsonTest(json2);
Console.WriteLine("——————————————Test3——————————————");
Console.WriteLine(json3);
DanKeJsonTest(json3);
LitJsonTest(json3);
Console.WriteLine("——————————————Test4——————————————");
Console.WriteLine(json4);
DanKeJsonTest(json4);
LitJsonTest(json4);

void DanKeJsonTest(string json)
{
    watch.Reset();
    watch.Start();
    for (int i = 0; i < count; i++)
    {
        DanKeJson.JsonData jData = JSON.ToData(json);
        string jsonText = JSON.ToJson(jData);
    }
    watch.Stop();
    Console.WriteLine("DanKeJson Serialization and Deserialization Time(ms):" + watch.ElapsedMilliseconds);
    Console.WriteLine("======================");
}

void LitJsonTest(string json)
{
    watch.Reset();
    watch.Start();
    for (int i = 0; i < count; i++)
    {
        LitJson.JsonData jData = JsonMapper.ToObject(json);
        string jsonText = JsonMapper.ToJson(jData);
    }
    watch.Stop();
    Console.WriteLine("LitJson Serialization and Deserialization Time(ms):" + watch.ElapsedMilliseconds);
    Console.WriteLine("======================");
}