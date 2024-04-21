using System.Diagnostics;
using DanKeJson;

string jsonText =
    "{\n  \"users\": [\n    {\n      \"id\": 1,\n      \"name\": \"张三\",\n      \"email\": \"zhangsan@example.com\",\n      \"phone\": \"123-456-7890\",\n      \"address\": {\n        \"street\": \"中山路\",\n        \"city\": \"北京\",\n        \"postalCode\": \"100000\"\n      },\n      \"roles\": [\n        \"管理员\",\n        \"用户\"\n      ]\n    },\n    {\n      \"id\": 2,\n      \"name\": \"李四\",\n      \"email\": \"lisi@example.com\",\n      \"phone\": \"987-654-3210\",\n      \"address\": {\n        \"street\": \"南京路\",\n        \"city\": \"上海\",\n        \"postalCode\": \"200000\"\n      },\n      \"roles\": [\n        \"用户\"\n      ]\n    },\n    {\n      \"id\": 3,\n      \"name\": \"王五\",\n      \"email\": \"wangwu@example.com\",\n      \"phone\": \"555-123-4567\",\n      \"address\": {\n        \"street\": \"成都路\",\n        \"city\": \"成都\",\n        \"postalCode\": \"610000\"\n      },\n      \"roles\": [\n        \"管理员\",\n        \"编辑\"\n      ]\n    }\n  ]\n}";
JsonData data = JSON.ToData(jsonText);
Console.WriteLine("");