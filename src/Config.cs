using System.Text.Json;
using System.IO;
using System;

public class Config {
    private const string CONFIG_PATH = "config.json";

    public string QuestAddress {get; set;}
    public int QuestPort {get; set;}
    public int UpdateInterval {get; set;} // In milliseconds

    // Deserializes the config file and loads it as a Config object
    public static Config load() {
        Console.WriteLine("Loading config");
        string fileContents = File.ReadAllText(CONFIG_PATH);
        return JsonSerializer.Deserialize<Config>(fileContents);
    }
}