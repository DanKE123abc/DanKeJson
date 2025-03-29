using DanKeJson;

var text =
    "{\n  \"model\": \"gpt-3.5-turbo\",\n  \"messages\": [\n    {\"role\": \"system\", \"content\": \"你是一个助手\"},\n    {\"role\": \"user\", \"content\": \"你好！\"}\n  ],\n  \"temperature\": 0.7,\n  \"max_tokens\": 100,\n  \"stream\": false\n}";

var json = JSON.ToData<JsonRoot>(text);

Console.WriteLine();


public class JsonRoot
{
    [JsonProperty("model")]
    public string Model { get;set; }
    
    public List<MessagesItem> messages { get;set; }
    
    public double temperature { get;set; }
    
    public int max_tokens { get;set; }
    
    public bool stream { get;set; }
}

public class MessagesItem
{
    public string role { get;set; }
    
    public string content { get;set; }
}