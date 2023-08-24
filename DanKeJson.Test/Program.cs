using DanKeJson;

JsonData json = new JsonData(JsonData.Type.Object);
json["Name"] = "DanKe";
json["Age"] = 16;
json["isHappy"] = true;
json["Data"] = new JsonData(JsonData.Type.Object);
json["Data"]["mail"] = "danke1024@foxmail.com";
json["Data"]["time"] = 2023;
json["Array"] = new JsonData(JsonData.Type.Array);
json["Array"].Add(111);
json["Array"].Add("sss");
json["Array"].Add("false");
JsonData json2 = new JsonData(JsonData.Type.Object);
json2["message"] = "Hello";
json["Array"].Add(json2);

string tojson = JSON.ToJson(json);
Console.WriteLine(tojson);

JsonData djson = JSON.ToData(tojson);
string message = djson["Array"][3]["message"];
Console.WriteLine(message);