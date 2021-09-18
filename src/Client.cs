using System;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Runtime.InteropServices;
using Avalonia;

namespace Quest_Discord_Presence_Client
{
    public class Client
    {
        public string CONFIG_PATH {get; private set;}

        public static Client Instance {get; private set;} // Sorry for singleton - there isn't a way to pass parameters to an App in Avalonia :(

        public PresenceManager PresenceManager {get;}

        public Config Config {get; private set;} // Currently loaded config

        private Client(string[] args) {
            Instance = this;

            // Make sure only one instance of the client is running at a time
            KillExistingInstances();

            // Find the config path depending on platform
            CONFIG_PATH = FindConfigPath();

            LoadConfig();

            PresenceManager = new PresenceManager(this);
            // Enfore the -nogui CLI option
            if(!(args.Length > 0 && args[0] == "-nogui")) {
                InitializeUI(args); // Show the window for customizing the config
            }

            // Start fetching presence from the quest
            PresenceManager.QueryQuest();
        }

        static void Main(string[] args) {
            new Client(args);
        }

        private void InitializeUI(string[] args) {
            // Run the UI on another thread
            new Thread(() => {
                Console.WriteLine("Hello from UI thread");
                try {
                    AppBuilder.Configure<App>()
                        .UsePlatformDetect()
                        .LogToTrace(Avalonia.Logging.LogEventLevel.Debug)
                        .StartWithClassicDesktopLifetime(args);
                } catch (Exception ex) {
                    Console.Error.WriteLine("Error from the UI thread: " + ex.Message);
                }
            }).Start();
        }

        // Kills any existing instances of the app
        private void KillExistingInstances() {
            // Find existing instances
            string filePath = Assembly.GetEntryAssembly().Location;
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            Process[] existingInstances = Process.GetProcessesByName(fileName);

            // Yeet them
            foreach(Process instance in existingInstances) {
                if(instance.Id == Process.GetCurrentProcess().Id) {continue;} // Make sure that it's not this process

                Console.WriteLine("Killing existing instance");
                instance.Kill();
            }
        }

        public string FindConfigPath() { 
            Console.WriteLine("Attempting to use config in appdata");
            // Copy the config from the install directory to appdata
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Quest Discord Presence Client/";
            Directory.CreateDirectory(appDataPath);

            string appDataConfigPath = appDataPath + "config.json";
            if(!File.Exists(appDataConfigPath)) {
                Console.WriteLine("Copying config from resources to appdata . . .");
                SaveResource("config.json", appDataConfigPath);
            }

            return appDataConfigPath;
        }


        // Saves the resource resourceName to the file destinationPath
        private void SaveResource(string resourceName, string destinationPath)
        {
            Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Quest_Discord_Presence_Client.resources." + resourceName);
            FileStream destStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);

            resourceStream.CopyTo(destStream);
            destStream.Close();
        }

        // Load/save the config using JSON serialization
        public void LoadConfig() {
            Console.WriteLine("Loading config");
            string fileContents = File.ReadAllText(CONFIG_PATH);
            Config = JsonSerializer.Deserialize<Config>(fileContents);
        }

        public void SaveConfig() {
            Console.WriteLine("Saving config");
            // Keep it pretty printed for now
            JsonSerializerOptions options = new JsonSerializerOptions() {
                WriteIndented = true
            };

            string text = JsonSerializer.Serialize(Config, options);
            File.WriteAllText(CONFIG_PATH, text);
        }
    }
}
