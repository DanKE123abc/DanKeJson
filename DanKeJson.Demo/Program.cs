using DanKeJson;

string jsonText = @"
        {
            ""notANumber"": NaN,
            ""positiveInfinity"": Infinity,
            ""positiveInfinityWithPlus"": +Infinity,
            ""negativeInfinity"": -Infinity,
            ""positiveOne"": +1,
            ""negativeOne"": -1,
            ""True"": true,
            ""False"": false
        }";

JsonData jsonData = JSON.ToData(jsonText);
double notANumber = jsonData["notANumber"];
double positiveInfinity = jsonData["positiveInfinity"];
double positiveInfinityWithPlus = jsonData["positiveInfinityWithPlus"];
double negativeInfinity = jsonData["negativeInfinity"];
double positiveOne= jsonData["positiveOne"];

JsonData newJsonData = new JsonData(JsonData.Type.Object);
newJsonData["notANumber"] = notANumber;
newJsonData["positiveInfinity"] = positiveInfinity;
newJsonData["positiveInfinityWithPlus"] = positiveInfinityWithPlus;
newJsonData["negativeInfinity"] = negativeInfinity;

Console.WriteLine();


