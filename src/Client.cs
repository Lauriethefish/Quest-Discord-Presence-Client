using System;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Logging.Serilog;

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
                AppBuilder.Configure<App>()
                    .UsePlatformDetect()
                    .LogToDebug()
                    .StartWithClassicDesktopLifetime(args);
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
            string installationPath = Assembly.GetEntryAssembly().Location;
            string configInInstallDirPath = installationPath + "/config.json";
            // If we can't find the above, check if there is a config in our working directory
            if(!File.Exists(configInInstallDirPath)) {
                configInInstallDirPath = "config.json";
            }
 
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Console.WriteLine("Attempting to use config in appdata - we're on windows");
                // If we're on windows, we should copy the config from program files to the app data folder if there isn't one already
                // The config used to be stored in program files, which was dumb, so we move it to app data
                // The config file is also in program files just after installing the app, so we need to copy it then as well
                string appDataFolder = Environment.GetEnvironmentVariable("APPDATA");
                string discordPresenceAppData = appDataFolder + "\\Quest Discord Presence Client";
                Directory.CreateDirectory(discordPresenceAppData);

                string appDataConfigPath = discordPresenceAppData + "\\config.json";
                if(!File.Exists(appDataConfigPath)) {
                    Console.WriteLine("Copying program files config to app data. ..");
                    File.Copy(configInInstallDirPath, appDataConfigPath);
                }

                return appDataConfigPath;
            }   else    {
                return configInInstallDirPath;
            }
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
