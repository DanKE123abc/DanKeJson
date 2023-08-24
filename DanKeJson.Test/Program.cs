using DanKeJson;

string tojson = "{\"Name\":\"DanKe\",\"Age\":16}";

JsonData djson = JSON.ToData(tojson);
int age = djson["Age"];
Console.WriteLine(age);