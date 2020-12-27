using System;
using System.Text.Json;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using DiscordRPC;
using DiscordRPC.Message;

namespace Quest_Discord_Presence_Client
{
    class Program
    {
        static void Main(string[] args) {new Program();}

        private Config _config;

        private FileSystemWatcher _configWatcher;
        private DiscordRpcClient _client;
        public Program() {
            _config = Config.load();
            StartWatchingConfig();

            // Connect to Discord RPC using our application ID
            ReconnectClient(null, null);
            _client.OnConnectionFailed += ReconnectClient;

            Console.WriteLine("Connection made. Querying Quest . . .");
            while(true) {
                int timeBefore = DateTime.UtcNow.Millisecond;

                Status status;
                try {
                    status = GetStatus();
                    
                    _client.Invoke(); // Dispatch events
                    // Set the received presence
                    _client.SetPresence(status.ConvertToDiscord());
                    Console.WriteLine("Successfully fetched presence");
                }   catch(Exception ex) {                    
                    Console.Error.WriteLine("Exception occured while fetching the presence from the Quest (is your IP address correct and Beat Saber open?): " + ex.Message);
                    
                    // Disable the precence, since we aren't connected/Beat Saber is closed.
                    Console.WriteLine("Disabling presense due to failed connection.");
                    _client.ClearPresence();
                }

                // Sleep for the remaining time
                int timeElapsed = DateTime.UtcNow.Millisecond - timeBefore;
                Thread.Sleep(Math.Max(_config.UpdateInterval - timeElapsed, 0)); // Make sure we don't sleep for a negative amount of time!
            }
        }
        
        // Creates a new client in order to reconnect to discord
        private void ReconnectClient(object sender, ConnectionFailedMessage args) {
            Console.WriteLine("Connecting to Discord . . .");
            try {
                _client = new DiscordRpcClient("743131742759813160");
                _client.Initialize();
            }   catch(Exception ex)   {
                Console.WriteLine("Failed to connect to discord " + ex.Message);
            }
        }

        private Status GetStatus() {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the address and port from the config
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_config.QuestAddress), _config.QuestPort);
            socket.Connect(endPoint);

            // First find the JSON's length
            byte[] jsonLength = new byte[4];
            socket.Receive(jsonLength);

            int length = BitConverter.ToInt32(SwapEndianness(jsonLength));

            // Then read the status as a string
            byte[] statusBytes = new byte[length];
            socket.Receive(statusBytes);
            string statusString = Encoding.UTF8.GetString(statusBytes);

            return JsonSerializer.Deserialize<Status>(statusString); // Deserialize it as a Status object
        }

        private byte[] SwapEndianness(byte[] input) {
            List<byte> list = new List<byte>(input);
            list.Reverse();
            return list.ToArray();
        }

        // Starts a FileSystemWatcher to auto-reload the config when it changes
        public void StartWatchingConfig() {
            _configWatcher = new FileSystemWatcher();
            _configWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _configWatcher.Filter = Config.CONFIG_PATH;
            _configWatcher.Path = Directory.GetCurrentDirectory();
            _configWatcher.Changed += OnConfigChanged;
            _configWatcher.EnableRaisingEvents = true;
            Console.WriteLine("Listening for changes to the config");
        }

        public void OnConfigChanged(object sender, FileSystemEventArgs args) {
            Console.WriteLine("Config was changed: reloading");
            Thread.Sleep(1000);
            _config = Config.load();
        }
    }
}
