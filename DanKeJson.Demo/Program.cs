﻿using DanKeJson;

var test1 = JSONL.LineToData("C:\\Users\\15860\\Downloads\\lora_medical.jsonl",80);

var test2 = JSONL.AllLineToData("C:\\Users\\15860\\Downloads\\lora_medical.jsonl");

var test3 = JSONL.ListToJson(test2);

Console.WriteLine();


