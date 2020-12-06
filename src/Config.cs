using System.Text.Json;
using System.IO;

public class Config {
    public string QuestAddress {get; set;}
    public int QuestPort {get; set;}
    public int UpdateInterval {get; set;} // In milliseconds

    // Deserializes the config file and loads it as a Config object
    public static Config load() {
        string fileContents = File.ReadAllText("config.json");
        return JsonSerializer.Deserialize<Config>(fileContents);
    }
}